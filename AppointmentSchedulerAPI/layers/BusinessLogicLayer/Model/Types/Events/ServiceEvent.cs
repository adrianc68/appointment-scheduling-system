namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events
{
    public class ServiceEvent
    {
        public ServiceEventType EventType { get; set; }
        public Guid ServiceUuid { get; set; }
    }
}