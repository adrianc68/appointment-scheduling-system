using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        [HttpPost("register")]
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
                guid = await systemFacade.RegisterAssistant(assistant);
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(guid, ApiVersionEnum.V1);
        }


        
    }
}
