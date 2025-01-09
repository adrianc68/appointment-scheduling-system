namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events
{
    public class ServiceEvent
    {
        public ServiceEventType EventType { get; set; }
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
        public Guid? ServiceUuid { get; set; }
        public int? ServiceId { get; set; }
    }
}