using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types
{
    public enum NotificationStatusType
    {
        [PgName("READ")]
        READ,
        [PgName("UNREAD")]
        UNREAD,
    }
}