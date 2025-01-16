using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model
{
    public class NotificationRecipient
    {
        public Guid Uuid { get; set; }
        public int Id { get; set; }
        public NotificationStatusType Status { get; set;}
        public DateTime? ChangedAt { get; set; } = DateTime.Now;
    }
}