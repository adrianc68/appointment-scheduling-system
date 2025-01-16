using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model
{
    public partial class NotificationRecipient
    {
        public int? IdUserAccount { get; set; }
        public int? IdNotificationBase { get; set; }
        public NotificationStatusType? Status { get; set; }
        public DateTime? ChangedAt { get; set; }
        public virtual NotificationBase? NotificationBase { get; set; }
        public virtual UserAccount? UserAccount { get; set; }

    }
}