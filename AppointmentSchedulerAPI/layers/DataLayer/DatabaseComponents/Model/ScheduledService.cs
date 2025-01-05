namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model
{
    public class ScheduledService
    {
        public int? IdServiceOffer { get; set; }
        public int? IdAppointment { get; set; }
        public TimeOnly ServiceStartTime { get; set; }
        public TimeOnly ServiceEndTime { get; set; }
        public string? ServiceName { get; set; }
        public int? ServicesMinutes { get; set; }
        public double? ServicePrice { get; set; }

        public virtual Appointment Appointment { get; set; }
        public virtual ServiceOffer ServiceOffer { get; set; }

    }
}