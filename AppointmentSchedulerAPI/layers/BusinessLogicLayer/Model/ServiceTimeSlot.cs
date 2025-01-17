namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class ServiceTimeSlot
    {
        public Guid ServiceUuid { get; set; }
        public Guid AssistantUuid { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}