using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types
{
    public enum AvailabilityTimeSlotStatusType
    {
        [PgName("AVAILABLE")]
        AVAILABLE,
        [PgName("NOT_AVAILABLE")]
        NOT_AVAILABLE,
        [PgName("DELETED")]
        DELETED
    }
}