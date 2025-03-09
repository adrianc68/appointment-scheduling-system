using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AppointmentNotificationDTO : NotificationDTO
    {
        public required AppointmentNotificationCodeType Code { get; set;}

        public required Guid AppointmentUuid { get; set;}

    }
}