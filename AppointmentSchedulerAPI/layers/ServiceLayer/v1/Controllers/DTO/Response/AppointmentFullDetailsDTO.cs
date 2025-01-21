namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AppointmentFullDetailsDTO
    {
        public AppointmentDTO? Appointment { get; set;}
        public ClientAppointmentDTO? Client {get;set;}
        public List<AppointmentServiceDTO>? ScheduledServices { get; set;}

    }
}