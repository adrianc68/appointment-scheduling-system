namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ServiceOfferDTO
    {
        public string? AssistantName { get; set; }
        public Guid? AssistantUuid { get; set; }
        public string? ServiceName { get; set; }
        public double? ServicePrice { get; set; }
        public int? ServiceMinutes { get; set; }
        public Guid? ServiceUuid { get; set; }
    }
}