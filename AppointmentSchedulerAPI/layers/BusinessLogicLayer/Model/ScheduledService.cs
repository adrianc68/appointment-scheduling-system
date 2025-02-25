namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class ScheduledService
    {
        public int? Id { get; set;}
        public Guid? Uuid { get; set;}
        public TimeOnly? ServiceStartTime { get; set; }
        public TimeOnly? ServiceEndTime { get; set; }
        public string? ServiceName { get; set; }
        public int? ServicesMinutes { get; set; }
        public double? ServicePrice { get; set; }
        public Appointment? Appointment { get; set;}
        public ServiceOffer? ServiceOffer { get; set;}
    }
}