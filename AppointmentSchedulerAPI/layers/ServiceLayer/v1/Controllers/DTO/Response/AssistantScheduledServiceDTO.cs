namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AssistantScheduledServiceDTO
    {
        public string? Name { get; set; }
        public Guid? Uuid { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}