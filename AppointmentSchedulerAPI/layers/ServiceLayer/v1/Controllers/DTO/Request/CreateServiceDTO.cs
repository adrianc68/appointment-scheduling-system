namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CreateServiceDTO
    {
        public required string Description { get; set; }
        public required int Minutes { get; set; }
        public required string Name { get; set; }
        public required double Price { get; set; }
    }
}