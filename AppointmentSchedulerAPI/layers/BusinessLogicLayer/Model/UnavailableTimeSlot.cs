namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class UnavailableTimeSlot
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}