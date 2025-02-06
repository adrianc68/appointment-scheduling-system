namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AsisstantOfferDTO
    {
        // public AssistantOccupiedServiceOfferDTO? Assistant { get; set; }
        public required string Name { get; set; }
        public required Guid Uuid { get; set; }
        public ServiceOfferRangeDTO? OccupiedTimeRange { get; set; }
    }
}