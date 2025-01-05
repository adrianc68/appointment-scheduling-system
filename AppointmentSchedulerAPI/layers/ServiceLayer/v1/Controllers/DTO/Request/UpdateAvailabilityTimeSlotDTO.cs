namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class UpdateAvailabilityTimeSlotDTO
    {
        public Guid? Uuid { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? EndTime { get; set; }
        public TimeOnly? StartTime { get; set; }
    }
}