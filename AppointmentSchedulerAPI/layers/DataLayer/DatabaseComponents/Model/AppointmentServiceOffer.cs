namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model
{
    public class AppointmentServiceOffer
    {
        public int? IdServiceOffer { get; set; }
        public int? IdAppointment { get; set; }
        public TimeOnly StartTime { get; set;}
        public TimeOnly EndTime { get; set;}

        public virtual Appointment Appointment { get; set; }
        public virtual ServiceOffer ServiceOffer { get; set; }

    }
}