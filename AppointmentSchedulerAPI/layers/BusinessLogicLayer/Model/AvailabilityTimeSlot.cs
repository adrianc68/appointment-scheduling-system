namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class AvailabilityTimeSlot
    {
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? EndTime { get; set; }
        public TimeOnly? StartTime { get; set; }


        public AvailabilityTimeSlot() { }

    }
}