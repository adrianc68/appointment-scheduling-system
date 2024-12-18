namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement.ExceptionHandlerService
{
    public class ExceptionHandlerService : IExceptionHandlerService
    {
        private readonly ILogger<ExceptionHandlerService> logger;

        public ExceptionHandlerService(ILogger<ExceptionHandlerService> logger)
        {
            this.logger = logger;
        }

        public void LogException(Exception ex, string context)
        {
            var identifier = Guid.NewGuid().ToString();
            logger.LogError(ex, "Error Identifier: {Identifier}, Context: {Context}", identifier, context);
        }

        string IExceptionHandlerService.HandleException(Exception ex, string version)
        {
            string identifier = Guid.NewGuid().ToString();
            logger.LogError(ex, "Error Identifier: {Identifier}", identifier);
            return identifier;
        }

    }
}