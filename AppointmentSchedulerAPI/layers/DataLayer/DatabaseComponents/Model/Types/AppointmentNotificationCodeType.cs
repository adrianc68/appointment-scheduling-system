using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types
{
    public enum AppointmentNotificationCodeType
    {
        [PgName("APPOINTMENT_SCHEDULED")]
        APPOINTMENT_SCHEDULED,
        [PgName("APPOINTMENT_RESCHEDULED")]
        APPOINTMENT_RESCHEDULED,
        [PgName("APPOINTMENT_CANCELED")]
        APPOINTMENT_CANCELED,
        [PgName("APPOINTMENT_REMINDER")]
        APPOINTMENT_REMINDER,
        [PgName("APPOINTMENT_CONFIRMED")]
        APPOINTMENT_CONFIRMED
    }
}
