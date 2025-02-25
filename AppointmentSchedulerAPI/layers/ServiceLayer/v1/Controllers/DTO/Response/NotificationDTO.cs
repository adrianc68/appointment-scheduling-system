using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public abstract class NotificationDTO
    {
        public required DateTime CreatedAt { get; set; }
        public required Guid Uuid { get; set; }
        public required string Message { get; set; }
        public required NotificationType Type { get; set;}
    }
}