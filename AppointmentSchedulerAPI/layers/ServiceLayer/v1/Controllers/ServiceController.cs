using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion(ApiVersionEnum.V1)]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;

        public ServiceController(IServiceInterfaces systemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
        }

        [HttpDelete("{idService}")]
        public IActionResult DeleteService(int idService)
        {
            var appointments = systemFacade.DeleteService(idService);
            return Ok(appointments);
        }

        [HttpGet]
        // [Authorize(Roles = UserRoleConstants.ASSISTANT + "," + UserRoleConstants.ADMINISTRATOR)]
        public IActionResult GetAllServices()
        {
            var services = new List<string> { "Service 1", "Service 2", "Service 3", "Service 4", "Service 5" };
            var data = new
            {
                services = services
            };
            return httpResponseService.OkResponse(data, ApiVersionEnum.V1);
            // return httpResponseService.InternalServerErrorResponse(new Exception("test"), ApiVersionEnum.V1);
        }

        // public IActionResult DisableService()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EnableService()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult DeleteService()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult RegisterService()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EditService()
        // {
        //     throw new NotImplementedException();
        // }


    }
}