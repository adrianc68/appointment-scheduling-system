using System.Security.Claims;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
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
        [AllowedRoles(BusinessLogicLayer.Model.Types.RoleType.ASSISTANT)]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            var claims = ClaimsPOCO.GetUserClaims(User);
            System.Console.WriteLine(claims.Role);
            System.Console.WriteLine(claims.Email);
            System.Console.WriteLine(claims.Username);
            System.Console.WriteLine(claims.Uuid);
 
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
