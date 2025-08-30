namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class ServiceTimeSlot
    {
        public Guid ServiceUuid { get; set; }
        public Guid AssistantUuid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}