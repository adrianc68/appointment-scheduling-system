namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ServiceAvailableDTO
    {
        public required string Name { get; set; }
        public required double Price { get; set; }
        public required int Minutes { get; set; }
        public required string Description { get; set; }
        public required Guid Uuid { get; set; }
        public AssistantServiceOfferDTO? Assistant { get; set; }
    }
}