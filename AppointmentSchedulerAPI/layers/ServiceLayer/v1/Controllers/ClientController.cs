using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class ClientController : ControllerBase
    {
        private readonly ISchedulingInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;

        public ClientController(ISchedulingInterfaces systemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
        }

        // public IActionResult DisableClient()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EnableClient()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult DeleteClient()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult GetClientStatusType()
        // {
        //     throw new NotImplementedException();
        // }


        // public IActionResult RegisterClient()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EditClient()
        // {
        //     throw new NotImplementedException();
        // }



    }
}
