namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ConflictingServiceOfferDTO
    {
        public string? AssistantName { get; set;}
        public Guid? AssistantUuid { get; set; }
        public Guid? ConflictingServiceOfferUuid { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}