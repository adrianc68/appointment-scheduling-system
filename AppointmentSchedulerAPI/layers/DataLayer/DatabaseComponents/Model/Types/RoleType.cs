using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types
{
    public enum RoleType
    {
        [PgName("ADMINISTRATOR")]
        ADMINISTRATOR,
        [PgName("CLIENT")]
        CLIENT,
        [PgName("ASSISTANT")]
        ASSISTANT
    }
}