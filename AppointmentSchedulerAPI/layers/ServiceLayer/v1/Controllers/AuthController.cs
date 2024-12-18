using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authentication;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService<JwtUserCredentials, JwtTokenResult> authenticationService;
        private readonly IHttpResponseService httpResponseService;

        public AuthController(IAuthenticationService<JwtUserCredentials, JwtTokenResult> authenticationService, IHttpResponseService httpResponseService)
        {
            this.authenticationService = authenticationService;
            this.httpResponseService = httpResponseService;

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] JwtUserCredentials credentials)
        {
            if (credentials == null)
            {
                return httpResponseService.BadRequest(ApiVersionEnum.V1, MessageCodeType.INVALID_CREDENTIALS);
            }

            JwtTokenResult? result = authenticationService.Authenticate(credentials);

            if (result == null)
            {
                return httpResponseService.Unauthorized(ApiVersionEnum.V1, MessageCodeType.UNAUTHORIZED);
            }
            return httpResponseService.OkResponse(result, ApiVersionEnum.V1);
        }


    }
}