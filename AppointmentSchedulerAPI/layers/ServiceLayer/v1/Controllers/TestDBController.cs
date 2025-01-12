using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiVersion("1")]
    public class TestDBController : ControllerBase
    {
        private readonly INotificationMgt notificationMgr;

        public TestDBController(INotificationMgt notificationMgr)
        {
            this.notificationMgr = notificationMgr;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {

            await notificationMgr.NotifyAllAsync(request.Message!);
            return Ok("Notification sent successfully.");
        }

        public class NotificationRequest
        {
            public string? Recipient { get; set; }
            public string? Message { get; set; }
        }


    }
}
