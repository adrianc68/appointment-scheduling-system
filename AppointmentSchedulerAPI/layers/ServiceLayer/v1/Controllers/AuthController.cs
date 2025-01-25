using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AccountInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;

        public AuthController(IAccountInterfaces systemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;

        }

        [HttpPost("login/")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginJwt([FromBody] LoginAccountAndPasswordDTO dto)
        {
            JwtTokenDTO token;
            try
            {
                OperationResult<JwtTokenResult, GenericError> result = await systemFacade.LoginWithEmailOrUsernameOrPhoneNumberJwtTokenAsync(dto.Account, dto.Password);
                if (result.IsSuccessful)
                {
                    token = new JwtTokenDTO
                    {
                        Token = result.Result!.Token!,
                        Expiration = result.Result.Expiration!.Value
                    };
                }
                else
                {
                    return httpResponseService.Unauthorized(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }

            return httpResponseService.OkResponse(token, ApiVersionEnum.V1);
        }


        [HttpPost("refresh/")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshJwt()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return httpResponseService.BadRequest(ApiVersionEnum.V1, "Token is missing.");
            }
            JwtTokenResult tokenResult;
            try
            {
                OperationResult<JwtTokenResult, GenericError> result = await systemFacade.RefreshToken(token);
                if (result.IsSuccessful)
                {
                    tokenResult = result.Result!;
                }
                else
                {
                    return httpResponseService.Unauthorized(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(tokenResult, ApiVersionEnum.V1);
        }


    }
}