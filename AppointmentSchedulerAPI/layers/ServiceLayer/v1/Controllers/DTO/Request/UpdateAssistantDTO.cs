namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class UpdateAssistantDTO
    {
        public required Guid Uuid { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
    }
}