using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class AssistantController : ControllerBase
    {
        private readonly ISchedulingInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;

        public AssistantController(ISchedulingInterfaces systemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
        }


    }
}
