using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class NotificationRecipient
    {
        public required NotificationRecipientData RecipientData { get; set; }
        public NotificationStatusType? Status { get; set; } = NotificationStatusType.UNREAD;
        // public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        private DateTime? _changedAt;
        public DateTime? ChangedAt
        {
            get => _changedAt;
            set => _changedAt = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not NotificationRecipient other)
            {
                return false;
            }

            return RecipientData.Equals(other.RecipientData);
        }

        public override int GetHashCode()
        {
            return RecipientData.GetHashCode();
        }

    }
}