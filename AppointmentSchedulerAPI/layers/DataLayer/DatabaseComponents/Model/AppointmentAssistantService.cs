namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model
{
    public class AppointmentAssistantService
    {
        public int? IdAssistantService { get; set; }
        public int? IdAppointment { get; set; }

        public virtual Appointment Appointment { get; set; }
        public virtual AssistantService AssistantService { get; set; }

    }
}