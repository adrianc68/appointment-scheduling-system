using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public abstract class Notification
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        public NotificationType Type { get; set; }
        public required string Message { get; set; }
        public required NotificationOptions Options { get; set; }
        public required HashSet<NotificationRecipient> Recipients { get; set; }
    }
}