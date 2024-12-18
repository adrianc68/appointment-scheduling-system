namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement.ExceptionHandlerService
{
    public interface IExceptionHandlerService
    {
        void LogException(Exception ex, string context);
        string HandleException(Exception ex, string version);
    }
}