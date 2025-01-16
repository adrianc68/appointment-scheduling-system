using NpgsqlTypes;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types
{
    public enum GeneralNotificationCodeType
    {
        [PgName("GENERAL_WELCOME")]
        GENERAL_WELCOME,
        [PgName("GENERAL_NEWS")]
        GENERAL_NEWS,
        [PgName("GENERAL_PROMOTION")]
        GENERAL_PROMOTION,
        [PgName("GENERAL_SERVICE_UPDATE")]
        GENERAL_SERVICE_UPDATE
    }
}
