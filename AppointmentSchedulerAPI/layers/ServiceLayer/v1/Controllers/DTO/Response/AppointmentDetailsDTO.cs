namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AppointmentDetailsDTO
    {
        public AppointmentDTO? Appointment { get; set;}
        public ClientAppointmentDTO? Client {get;set;}
        public List<AsisstantOfferDTO>? Assistants { get; set;}

    }
}