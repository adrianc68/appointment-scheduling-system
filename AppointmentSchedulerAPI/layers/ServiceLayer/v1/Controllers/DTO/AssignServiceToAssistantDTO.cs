namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO
{
    public class AssignServiceToAssistantDTO
    {
        public required Guid assistantUuid { get; set;}
        public required List<Guid?> servicesUuid {get; set;}
    }
}