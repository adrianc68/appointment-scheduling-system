namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class AvailabilityTimeSlot
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan StartTime { get; set; }

        public AvailabilityTimeSlot() { }

    }
}