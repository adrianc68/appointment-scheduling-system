namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO
{
    public class AssistantDTO
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Username { get; set; }
        public string? Status { get; set; }
        public Guid? Uuid { get; set; }
    }
}