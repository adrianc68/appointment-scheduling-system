using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public abstract class NotificationBase
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        public NotificationStatusType? Status { get; set; } = NotificationStatusType.UNREAD;
        public string? Message { get; set; }
        public NotificationType Type { get; set;}
        public required List<NotificationRecipient> Recipients { get; set; }
    }
}