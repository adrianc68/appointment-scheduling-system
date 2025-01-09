namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events
{
    public class AssistantEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public AssistantEventType EventType { get; set; }
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
        public int? AssistantId { get; set; }
        public Guid AssistantUuid { get; set; }
    }
}