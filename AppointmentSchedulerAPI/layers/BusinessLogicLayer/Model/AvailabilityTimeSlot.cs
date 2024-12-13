namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class AvailabilityTimeSlot
    {
        public int Id { get; set; }
        public Date Date { get; set; }
        public Time EndTime { get; set; }
        public Time StartTime { get; set; }
        public PriorityType Priority { get; set; }

        public AvailabilityTimeSlot() { }

    }
}