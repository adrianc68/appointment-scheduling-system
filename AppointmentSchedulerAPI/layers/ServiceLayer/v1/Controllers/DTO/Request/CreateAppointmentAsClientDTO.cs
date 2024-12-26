namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CreateAppointmentAsClientDTO
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public Guid ClientUuid { get; set; }
        public List<ServiceUuidsDTO>? AssistantServices {get; set;}
    }
}