using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiVersion("1")]
    public class AssistantController : ControllerBase
    {
        private readonly IAssistantInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;
        private readonly AppointmentDbContext db;

        public AssistantController(IAssistantInterfaces systemFacade, IHttpResponseService httpResponseService, AppointmentDbContext db)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
            this.db = db;
        }

        // public IActionResult DisableAssistant()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EnableAssistant()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult DeleteAssistant()
        // {
        //     throw new NotImplementedException();
        // }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAssistant()
        {
            List<AssistantDTO> assistantDtos = [];
            try
            {
                var assistants = await systemFacade.GetAllAssistantsAsync();
                assistantDtos = assistants.Select(a => new AssistantDTO
                {
                    Uuid = a.Uuid,
                    Email = a.Email,
                    Name = a.Name,
                    PhoneNumber = a.PhoneNumber,
                    Username = a.Username,
                    Status = a.Status.ToString(),
                    CreatedAt = a.CreatedAt
                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1); ;
            }
            return httpResponseService.OkResponse(assistantDtos, ApiVersionEnum.V1);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAssistant([FromBody] CreateAssistantDTO assistantDTO)
        {
            Guid? guid;
            try
            {
                BusinessLogicLayer.Model.Assistant assistant = new()
                {
                    Name = assistantDTO.Name,
                    Email = assistantDTO.Email,
                    PhoneNumber = assistantDTO.PhoneNumber,
                    Password = assistantDTO.Password,
                    Username = assistantDTO.Username
                };
                OperationResult<Guid, GenericError> result = await systemFacade.RegisterAssistant(assistant);
                if (result.IsSuccessful)
                {
                    guid = result.Result;
                }
                else
                {
                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(guid, ApiVersionEnum.V1);
        }

        [HttpPost("service")]
        [AllowAnonymous]
        public async Task<IActionResult> AssignServiceToAssistant([FromBody] AssignServiceToAssistantDTO assignServiceDTO)
        {
            bool isAssigned = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.AssignServicesToAssistant(assignServiceDTO.assistantUuid, assignServiceDTO.servicesUuid);
                if (result.IsSuccessful)
                {
                    isAssigned = result.Result;
                }
                else
                {
                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(isAssigned, ApiVersionEnum.V1);

        }



    }
}
