using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class SchedulingController : ControllerBase
    {
        private readonly ISchedulingInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;

        public SchedulingController(ISchedulingInterfaces systemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
        }

        // public IActionResult ChangeAppointmentStatus()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult GetAppointmentDetails()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult GetAppointment()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult GetAvailableServices()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult ScheduleAppointmentAsClient()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult ScheduleAppointmentAsStaff()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult RegisterAvailabilityTimeSlot()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EditAppointment()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult FinalizeAppointment()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult CancelAppointClientSelf()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult CancelAppointmentStaffAssisted()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult DeleteAvailabilityTimeSlot()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EditAvailabilityTimeSlot()
        // {
        //     throw new NotImplementedException();
        // }
    }
}
