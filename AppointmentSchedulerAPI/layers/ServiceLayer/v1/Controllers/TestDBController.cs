using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.AccountMgr.Interfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
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
        private readonly IAccountMgt accountMgr;
        private readonly ISchedulerMgt schedulerMgr;

        public TestDBController(INotificationMgt notificationMgr, IAccountMgt accountMgr, ISchedulerMgt schedulerMgr)
        {
            this.notificationMgr = notificationMgr;
            this.accountMgr = accountMgr;
            this.schedulerMgr = schedulerMgr;
        }

        [HttpPost("send")]
        [Authorize]
        [AllowedRoles(RoleType.ASSISTANT, RoleType.CLIENT, RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            var claims = ClaimsPOCO.GetUserClaims(User);
            var appointmentData = await schedulerMgr.GetAppointmentByUuidAsync(request.AppointmentUuid!.Value);
            var accountData = await accountMgr.GetAccountIdByUuid(claims.Uuid);

            System.Console.WriteLine("********");
            System.Console.WriteLine(appointmentData);
            System.Console.WriteLine("********");
            System.Console.WriteLine(accountData);
            System.Console.WriteLine("********");

            AppointmentNotification appointmentNotification = new AppointmentNotification
            {
                Type = NotificationType.APPOINTMENT_NOTIFICATION,
                Message = $"La cita se ha cancelado.",
                Code = NotificationCodeType.APPOINTMENT_CANCELED,
                Recipient = new BusinessLogicLayer.Model.AccountData
                {
                    Uuid = appointmentData!.Client!.Uuid,
                    Id = appointmentData.Client.Id
                    },
                Appointment = new BusinessLogicLayer.Model.Appointment
                {
                    Id = appointmentData!.Id,
                    Uuid = request.AppointmentUuid.Value
                }
            };

            PropToString.PrintData(appointmentNotification);
            PropToString.PrintData(appointmentNotification.Recipient);
            PropToString.PrintData(appointmentNotification.Appointment);

            await this.notificationMgr.CreateNotification(appointmentNotification);
      

            // await notificationMgr.NotifyAllAsync("Mensaje enviado para uno");

            // await notificationMgr.NotifyToUserAsync(request.Recipient!, request.Message!);
            return Ok("Notification sent successfully.");
        }

        public class NotificationRequest
        {
            public string? Recipient { get; set; }
            public string? Message { get; set; }
            public Guid? AppointmentUuid { get; set; }
        }


    }
}
