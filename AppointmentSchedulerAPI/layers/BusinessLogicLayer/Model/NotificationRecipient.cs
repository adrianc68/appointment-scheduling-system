using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class NotificationRecipient
    {
        public Guid Uuid { get; set; }
        public int Id { get; set; }
        public NotificationStatusType Status { get; set;}
        public DateTime? ChangedAt { get; set; } = DateTime.Now;
    }
}