namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class AssignServiceToAssistantDTO
    {
        public required Guid AssistantUuid { get; set; }
        public required List<Guid> uuidServices { get; set; }
    }
}