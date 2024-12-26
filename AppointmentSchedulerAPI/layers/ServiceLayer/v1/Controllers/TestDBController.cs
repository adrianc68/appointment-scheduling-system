using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiVersion("1")]
    public class TestDBController : ControllerBase
    {
        private readonly ISchedulingInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;
        private readonly IAssistantRepository assistantRepository;
        private readonly IClientRepository clientRepository;
        private readonly IServiceRepository serviceRepository;
        private readonly ISchedulerRepository schedulerRepository;

        public TestDBController(ISchedulingInterfaces systemFacade, IHttpResponseService httpResponseService, IAssistantRepository assistantRepository, IClientRepository clientRepository, IServiceRepository serviceRepository, ISchedulerRepository schedulerRepository)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
            this.assistantRepository = assistantRepository;
            this.clientRepository = clientRepository;
            this.serviceRepository = serviceRepository;
            this.schedulerRepository = schedulerRepository;
        }


        [HttpGet("test")]
        [AllowAnonymous]
        public async Task<IActionResult> TestDBMethod()
        {
            object result;

            try
            {
                
                Guid guid = Guid.Parse("01940146-3d5f-756b-b03c-0776ffa7a7bf");

                // Assistant uid
                // Guid guid = Guid.Parse("019401d1-1634-7daf-afb7-d8d6066899b6");
                // List<Service>? assistant = await assistantRepository.GetServicesAssignedToAssistantByUuidAsync(guid);
                // result = assistant;
                // PropToString.PrintListData(assistant);
                result = await serviceRepository.GetServiceByUuidAsync(guid);

            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1); ;
            }
            return httpResponseService.OkResponse(result, ApiVersionEnum.V1);
        }

      
    }
}
