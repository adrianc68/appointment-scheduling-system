namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model
{
    public partial class AppointmentAssistant
    {
        public int? IdAssistant { get; set; }

        public int? IdAppointment { get; set; }

        public virtual Appointment? Appointment { get; set; }

        public virtual Assistant? Assistant { get; set; }
    }
}
