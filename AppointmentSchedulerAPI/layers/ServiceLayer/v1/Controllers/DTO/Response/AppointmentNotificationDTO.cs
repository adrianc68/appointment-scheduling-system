using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AppointmentNotificationDTO : NotificationDTO
    {
        public required AppointmentNotificationCodeType Code { get; set;}
        public required AppointmentUuidDTO Appointment { get; set;}

    }
}