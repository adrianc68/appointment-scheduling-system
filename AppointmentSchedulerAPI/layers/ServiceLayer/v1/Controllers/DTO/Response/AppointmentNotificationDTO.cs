namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AppointmentNotificationDto : NotificationDto
    {
        public Guid AppointmentUuid { get; set; }
    }
}