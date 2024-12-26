namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ServiceDTO
    {

        public string? Description { get; set; }
        public int? Minutes { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public Guid? Uuid { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}