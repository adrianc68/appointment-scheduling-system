namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AsisstantOfferDTO
    {
        public string? AssistantName { get; set; }
        public Guid AssistantUuid { get; set; }
        public TimeOnly StartTimeOfAssistantOfferingService { get; set; }
        public TimeOnly EndTimeOfAsisstantOfferingService { get; set; }
    }
}