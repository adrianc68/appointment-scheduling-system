namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class NotificationRecipientData
    {
        public required string Email { get; set; }
        public required int UserAccountId { get; set; }
        public required Guid UserAccountUuid { get; set; }
        public required string PhoneNumber { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is not NotificationRecipientData other)
            {
                return false;
            }

            return UserAccountId == other.UserAccountId && UserAccountUuid == other.UserAccountUuid;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserAccountId, UserAccountUuid);

        }

    }
}