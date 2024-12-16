namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class Scheduler
    {
        public int Id { get; set; }
        public int AvailableSlots { get; set; }
        public int UnavailableSlots { get; set; }
        public DateTime LastUpdate { get; set; }

        public Scheduler() { }
    }
}