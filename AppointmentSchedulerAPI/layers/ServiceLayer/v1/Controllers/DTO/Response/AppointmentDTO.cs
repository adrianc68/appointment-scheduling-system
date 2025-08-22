namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AppointmentDTO
    {
        public Guid? Uuid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public Double? TotalCost { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}