namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types
{
    public enum NotificationCodeType
    {
        APPOINTMENT_SCHEDULED,
        APPOINTMENT_TRESCHEDULED,
        APPOINTMENT_CANCELED,
        APPOINTMENT_REMINDER,
        APPOINTMENT_CONFIRMED,

        SYSTEM_MAINTENANCE,
        NEW_FEATURE_ANNOUNCEMENT,
        GENERAL_ALERT,

        PAYMENT_PROCESSED,
        PAYMENT_FAILED,
        INVOICE_GENERATED,

        PROFILE_UPDATED,
        PASSWORD_CHANGE_REQUESTED,
        USER_ACCOUNT_DEACTIVATED,

        ERROR_NOTIFICATION,
    }
}