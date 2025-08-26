namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ScheduledServiceDTO
    {
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public double? Price { get; set; }
        public int? Minutes { get; set; }
        public string? Name { get; set; }
        public Guid Uuid { get; set; }
    }
}