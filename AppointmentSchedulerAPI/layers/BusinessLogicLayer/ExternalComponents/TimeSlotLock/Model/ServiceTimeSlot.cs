namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeSlotLock.Model
{
    public class ServiceTimeSlot
    {
        public Guid ServiceUuid { get; set; }
        public Guid AssistantUuid { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}