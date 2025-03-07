namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model
{
    public enum MessageCodeType
    {
        // General use codes
        OK,
        ERROR,
        SUCCESS_OPERATION,
        DATA_FOUND,
        DATA_NOT_FOUND,
        DATA_UPDATED,
        SERVER_ERROR,
        REGISTER_ERROR,
        UPDATE_ERROR,
        INVALID_RANGE_TIME,
        UNAUTHORIZED,
        // Authentication codes
        AUTHENTICATION_FAILED_TO_GENERATE_JWT_TOKEN,
        AUTHENTICATION_INVALID_CREDENTIALS,
        AUTHENTICATION_CANNOT_REFRESH_TOKEN,
        AUTHENTICATION_UUID_VIOLATION,
        // Account codes
        ACCOUNT_USERNAME_ALREADY_REGISTERED,
        ACCOUNT_EMAIL_ALREADY_REGISTERED,
        ACCOUNT_PHONE_NUMBER_ALREADY_REGISTERED,
        ACCOUNT_NOT_FOUND,
        // Client codes
        CLIENT_NOT_FOUND,
        CLIENT_NOT_AVAILABLE,
        CLIENT_WAS_DELETED,
        CLIENT_IS_ALREADY_DISABLED,
        CLIENT_IS_ALREADY_ENABLED,
        CLIENT_IS_ALREADY_DELETED,
        // Assistant codes
        ASSISTANT_NOT_FOUND,
        ASSISTANT_IS_ALREADY_DISABLED,
        ASSISTANT_IS_ALREADY_ENABLED,
        ASSISTANT_IS_ALREADY_DELETED,
        ASSISTANT_WAS_DELETED,
        ASSISTANT_UNAVAILABLE,
        ASSISTANT_NOT_AVAILABLE_IN_TIME_RANGE,
        // Service codes
        SERVICE_IS_ALREADY_ENABLED,
        SERVICE_NAME_ALREADY_REGISTERED,
        SERVICE_NOT_FOUND,
        SERVICE_IS_ALREADY_DISABLED,
        SERVICE_IS_ALREADY_DELETED,
        SERVICE_OFFER_NOT_FOUND,
        SERVICE_IS_ALREADY_UNAVAILABLE,
        SERVICE_IS_ALREADY_AVAILABLE,
        SERVICE_WAS_DELETED,
        SERVICE_OFFER_UNAVAILABLE,
        SERVICE_OFFER_IS_ALREADY_ENABLED,
        SERVICE_OFFER_IS_ALREADY_DISABLED,
        SERVICE_OFFER_WAS_DELETED,
        SERVICE_UNAVAILABLE,
        SERVICES_ARE_NOT_CONTIGUOUS,
        SERVICE_ALREADY_ASSIGNED_TO_ASSISTANT,
        // Availability time slot codes
        AVAILABILITY_TIME_SLOT_NOT_AVAILABLE,
        AVAILABILITY_TIME_SLOT_NOT_FOUND,
        AVAILABILITY_TIME_SLOT_IS_ALREADY_DISABLED,
        AVAILABILITY_TIME_SLOT_IS_ALREADY_ENABLED,
        AVAILABILITY_TIME_SLOT_IS_ALREADY_DELETED,
        AVAILABILITY_TIME_SLOT_HAS_CONFLICTS,
        // Appointment codes
        APPOINTMENT_NOT_FOUND,
        APPOINTMENT_IS_ALREADY_CONFIRMED,
        APPOINTMENT_NEEDS_TO_BE_CONFIRMED,
        APPOINTMENT_IS_ALREADY_FINISHED,
        APPOINTMENT_IS_ALREDY_CANCELED,
        APPOINTMENT_SLOT_UNAVAILABLE,
        APPOINTMENT_BELONGS_TO_ANOTHER_USER,
        // Appointment block time range codes
        APPOINTMENT_TIME_RANGE_LOCK_NOT_FOUND,
        APPOINTMENT_TIME_RANGE_LOCK_HAS_BEEN_LOCKED_BY_USER,
        APPOINTMENT_TIME_RANGE_LOCK_MISTMATCH,
        APPOINTMENT_TIME_RANGE_LOCK_CANNOT_BE_EXTENDED,
        APPOINTMENT_TIME_RANGE_LOCK_OCCUPPIED_BY_ANOTHER_USER,
        // Appointment alternative codes
        APPOINTMENT_SCHEDULING_IN_THE_PAST_NOT_ALLOWED,
        APPOINTMENT_SCHEDULING_BEYOND_X_NOT_ALLOWED,
        APPOINTMENT_SERVICES_LIMIT_REACHED,
        APPOINTMENT_SCHEDULED_LIMIT_REACHED,
        APPOINTMENT_SELECTED_SERVICE_CONFLICT_WITH_TIME_SLOT,
        // Events codes (replace $<>$ to set data)
        EVENT_APPOINTMENT_HAS_BEEN_CANCELED,
        EVENT_APPOINTMENT_HAS_BEEN_RESCHEDULED
    }
}
