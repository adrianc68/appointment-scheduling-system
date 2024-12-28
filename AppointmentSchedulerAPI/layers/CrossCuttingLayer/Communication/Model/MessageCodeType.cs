namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model
{
    public enum MessageCodeType
    {
        SUCCESS_OPERATION,
        DATA_FOUND,
        DATA_UPDATED,
        SERVER_ERROR,
        INVALID_CREDENTIALS,
        UNAUTHORIZED,
        USERNAME_ALREADY_REGISTERED,
        NULL_VALUE_IS_PRESENT,
        EMAIL_ALREADY_REGISTERED,
        PHONE_NUMBER_ALREADY_REGISTERED,
        REGISTER_ERROR
    }
}
