using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types
{
    public enum AssistantStatusType
    {
        [PgName("ENABLED")]
        ENABLED,
        [PgName("DISABLED")]
        DISABLED,
        [PgName("DELETED")]
        DELETED,
    }
}