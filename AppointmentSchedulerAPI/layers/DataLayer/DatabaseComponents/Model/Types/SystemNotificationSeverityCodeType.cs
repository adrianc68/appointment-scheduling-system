using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types
{
    public enum SystemNotificationSeverityCodeType
    {
        [PgName("INFO")]
        INFO,
        [PgName("WARNING")]
        WARNING,
        [PgName("CRITICAL")]
        CRITICAL,

    }
}
