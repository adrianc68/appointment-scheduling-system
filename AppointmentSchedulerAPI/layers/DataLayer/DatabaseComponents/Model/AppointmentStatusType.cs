using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model
{
    public enum AppointmentStatusType
    {
        [PgName("SCHEDULED")]
        SCHEDULED,
        [PgName("CONFIRMED")]
        CONFIRMED,
        [PgName("CANCELED")]
        CANCELED,
        [PgName("FINISHED")]
        FINISHED
    }
}

