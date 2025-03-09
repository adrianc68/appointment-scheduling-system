namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ServiceOfferDetailsDTO
    {
        public required ServiceOfferDTO ServiceOffer { get; set; }
        public required AssistantServiceOfferDTO Assistant { get; set; }
    }
}