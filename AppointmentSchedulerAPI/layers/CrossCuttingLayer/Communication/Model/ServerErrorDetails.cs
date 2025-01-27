namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model
{
    public class ServerErrorDetails
    {
        public required string Error { get; set; }
        public required string Message { get; set; }
        public string? Details { get; set; }
        public required string Identifier { get; set; }
    }
}