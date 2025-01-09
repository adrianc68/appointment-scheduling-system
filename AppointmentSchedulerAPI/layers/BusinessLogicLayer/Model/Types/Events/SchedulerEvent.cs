namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events
{
    public class SchedulerEvent<T>
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
        public EventSource Source { get; set; }
        public string? Description { get; set; }
        public SchedulerEventType EventType { get; set; }
        public T? EventData { get; set; }
    }
}