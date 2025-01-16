using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types
{
    public enum NotificationCodeType
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
        APPOINTMENT_CONFIRMED,
        [PgName("SYSTEM_MAINTENANCE")]
        SYSTEM_MAINTENANCE,
        [PgName("NEW_FEATURE_ANNOUNCEMENT")]
        NEW_FEATURE_ANNOUNCEMENT,
        [PgName("GENERAL_ALERT")]
        GENERAL_ALERT,
        [PgName("PAYMENT_PROCESSED")]
        PAYMENT_PROCESSED,
        [PgName("PAYMENT_FAILED")]
        PAYMENT_FAILED,
        [PgName("INVOICE_GENERATED")]
        INVOICE_GENERATED,

        [PgName("PROFILE_UPDATED")]
        PROFILE_UPDATED,
        [PgName("PASSWORD_CHANGE_REQUESTED")]
        PASSWORD_CHANGE_REQUESTED,
        [PgName("USER_ACCOUNT_DEACTIVATED")]
        USER_ACCOUNT_DEACTIVATED,
        [PgName("ERROR_NOTIFICATION")]
        ERROR_NOTIFICATION,
    }
}