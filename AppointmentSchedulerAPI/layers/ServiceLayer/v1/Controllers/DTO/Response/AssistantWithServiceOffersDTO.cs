namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AssistantWithServicesOfferDTO
    {
        public required AssistantServiceOfferDTO Assistant { get; set; }
        public required List<ServiceOfferDTO> ServiceOffer { get; set; }
    }
}