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


            // AppointmentNotification appointmentNotification = new AppointmentNotification
            // {
            //     Type = NotificationType.APPOINTMENT_NOTIFICATION,
            //     Message = $"La cita se ha cancelado.",
            //     Code = AppointmentNotificationCodeType.APPOINTMENT_CANCELED,
            //     Recipients = new List<NotificationRecipient>
            //     {
            //         new NotificationRecipient
            //         {
            //         Uuid = appointmentData!.Client!.Uuid!.Value,
            //         Id = appointmentData.Client.Id!.Value
            //         },
            //         new NotificationRecipient
            //         {
            //             Uuid = appointmentData!.
            //         }
            //     },
            //     Appointment = new BusinessLogicLayer.Model.Appointment
            //     {
            //         Id = appointmentData!.Id,
            //         Uuid = request.AppointmentUuid.Value
            //     }
            // };

            GeneralNotification generalNotification = new GeneralNotification
            {
                Type = NotificationType.GENERAL_NOTIFICATION,
                Message = "Estamos trabajando en una nueva versión para el sistema web!!!",
                Code = GeneralNotificationCodeType.GENERAL_SERVICE_UPDATE,
                Recipients = [],
            };


            await this.notificationMgr.CreateNotification(generalNotification, NotificationUsersToSendType.SEND_TO_EVERYONE);


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
