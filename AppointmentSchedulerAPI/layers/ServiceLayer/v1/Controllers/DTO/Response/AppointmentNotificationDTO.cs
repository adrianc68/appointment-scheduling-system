namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AppointmentNotificationDto : NotificationDTO
    {
        public Guid AppointmentUuid { get; set; }
    }
}