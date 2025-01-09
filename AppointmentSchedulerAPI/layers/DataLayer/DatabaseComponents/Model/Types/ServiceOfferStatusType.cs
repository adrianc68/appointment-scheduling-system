using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types
{
    public enum ServiceOfferStatusType
    {
        [PgName("AVAILABLE")]
        AVAILABLE,
        [PgName("NOT_AVAILABLE")]
        NOT_AVAILABLE,
        [PgName("DELETED")]
        DELETED
    }
}