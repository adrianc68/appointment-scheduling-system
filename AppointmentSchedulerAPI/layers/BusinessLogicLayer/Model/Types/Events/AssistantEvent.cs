namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events
{
    public class AssistantEvent
    {
        public Guid AssistantUuid { get; set; }
        public AssistantEventType EventType { get; set; }
    }
}