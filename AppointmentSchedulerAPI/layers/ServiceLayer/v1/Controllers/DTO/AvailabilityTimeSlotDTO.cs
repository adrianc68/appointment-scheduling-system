namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO
{
    public class AvailabilityTimeSlotDTO
    {
        public DateOnly? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public Guid? Uuid { get; set; }
        public Guid? AssistantUuid { get; set; }
    }
}