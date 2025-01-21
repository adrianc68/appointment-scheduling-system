using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types
{
    public enum NotificationType
    {
        [PgName("APPOINTMENT_NOTIFICATION")]
        APPOINTMENT_NOTIFICATION,
        [PgName("PAYMENT_NOTIFICATION")]
        PAYMENT_NOTIFICATION,
        [PgName("SYSTEM_NOTIFICATION")]
        SYSTEM_NOTIFICATION,
        [PgName("GENERAL_NOTIFICATION")]
        GENERAL_NOTIFICATION,
    }
}