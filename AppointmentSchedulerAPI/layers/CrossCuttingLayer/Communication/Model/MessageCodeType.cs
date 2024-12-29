namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model
{
    public enum MessageCodeType
    {
        OK,
        SUCCESS_OPERATION,
        DATA_FOUND,
        DATA_NOT_FOUND,
        DATA_UPDATED,
        SERVER_ERROR,
        INVALID_CREDENTIALS,
        UNAUTHORIZED,
        USERNAME_ALREADY_REGISTERED,
        NULL_VALUE_IS_PRESENT,
        EMAIL_ALREADY_REGISTERED,
        PHONE_NUMBER_ALREADY_REGISTERED,
        REGISTER_ERROR,
        SERVICE_NAME_ALREADY_REGISTERED,
        SERVICE_NOT_FOUND,
        TIME_SLOT_NOT_AVAILABLE
    }
}
