namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class ServiceUuidsDTO
    {
        public Guid AssistantUuid { get; set; }
        public List<Guid>? ServiceUuids { get; set; }
    }
}