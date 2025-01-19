using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;
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


            AppointmentNotification notificationApp = new AppointmentNotification
            {
                Type = NotificationType.APPOINTMENT_NOTIFICATION,
                Message = $"La cita se ha cancelado.",
                Code = AppointmentNotificationCodeType.APPOINTMENT_CANCELED,
                Options = new NotificationOptions
                {
                    Channels = new List<NotificationChannelType>
                    {
                        NotificationChannelType.WEB_APPLICATION
                    }
                },
                Appointment = new BusinessLogicLayer.Model.Appointment
                {
                    Id = appointmentData!.Id,
                    Uuid = request.AppointmentUuid.Value
                },
                Recipients = []
            };

            GeneralNotification notificationGen = new GeneralNotification
            {
                Type = NotificationType.GENERAL_NOTIFICATION,
                Message = "Estamos trabajando en una nueva versión para el sistema web!!!",
                Code = GeneralNotificationCodeType.GENERAL_SERVICE_UPDATE,
                Options = new NotificationOptions
                {
                    Channels = new List<NotificationChannelType>
                    {
                        NotificationChannelType.WEB_APPLICATION
                    }
                },
                Recipients = [],
            };

            SystemNotification notificationSys = new SystemNotification
            {
                Message = "EL servidor estara en mantenimiento a las 18:00 PM MXN",
                Code = SystemNotificationCodeType.SYSTEM_MAINTENANCE,
                Options = new NotificationOptions
                {
                    Channels = new List<NotificationChannelType>
                    {
                        NotificationChannelType.WEB_APPLICATION
                    }
                },
                Recipients = [],
                Severity = SystemNotificationSeverityCodeType.INFO
            };


            // await this.notificationMgr.CreateNotification(notification, NotificationUsersToSendType.SEND_TO_EVERYONE);
            await this.notificationMgr.CreateNotification(notificationApp);
            await this.notificationMgr.CreateNotification(notificationGen);
            await this.notificationMgr.CreateNotification(notificationSys);


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
