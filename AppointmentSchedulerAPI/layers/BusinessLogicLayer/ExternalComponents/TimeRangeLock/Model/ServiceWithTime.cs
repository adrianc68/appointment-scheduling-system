namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Model
{
    public class ServiceWithTime
    {
        public Guid ServiceUuid { get; set; }
        public Guid AssistantUuid { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}