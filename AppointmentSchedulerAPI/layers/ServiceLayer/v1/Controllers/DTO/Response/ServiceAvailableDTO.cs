namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ServiceAvailableDTO
    {
        public AssistantServiceOfferDTO? Assistant { get; set; }
        public ServiceOfferDTO? Service { get; set; }
    }
}