using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authorization.Attributes;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [AllowedRoles(RoleType.ASSISTANT, RoleType.CLIENT, RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            var claims = ClaimsPOCO.GetUserClaims(User);
            // await notificationMgr.NotifyAllAsync("Mensaje enviado para uno");

            await notificationMgr.NotifyToUserAsync(request.Recipient!, request.Message!);
            return Ok("Notification sent successfully.");
        }

        public class NotificationRequest
        {
            public string? Recipient { get; set; }
            public string? Message { get; set; }
        }


    }
}
