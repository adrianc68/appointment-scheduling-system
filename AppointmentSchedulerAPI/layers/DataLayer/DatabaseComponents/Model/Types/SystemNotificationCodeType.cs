using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types
{
    public enum SystemNotificationCodeType
    {
        [PgName("SYSTEM_MAINTENANCE")]
        SYSTEM_MAINTENANCE,
        [PgName("SYSTEM_ERROR")]
        SYSTEM_ERROR,
        [PgName("SYSTEM_UPDATE")]
        SYSTEM_UPDATE,
        [PgName("SYSTEM_BACKUP")]
        SYSTEM_BACKUP,
        [PgName("SYSTEM_SECURITY")]
        SYSTEM_SECURITY,
        [PgName("SYSTEM_THRESHOLD")]
        SYSTEM_THRESHOLD,
        [PgName("SYSTEM_CONFIG_CHANGE")]
        SYSTEM_CONFIG_CHANGE
    }
}
