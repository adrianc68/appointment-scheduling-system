namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO
{
    public class AssistantServiceDTO
    {
        public required AssistantDTO Assistant { get; set; }
        public List<ServiceDTO>? Services { get; set; }
    }
}