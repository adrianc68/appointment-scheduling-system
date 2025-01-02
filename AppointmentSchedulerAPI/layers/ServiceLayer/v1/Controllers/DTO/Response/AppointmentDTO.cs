namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AppointmentDTO
    {
        public Guid? Uuid { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public DateOnly? Date { get; set; }
        public string? Status { get; set; }
        public Double? TotalCost { get; set; }
        public DateTime CreatedAt { get; set;}
        public ClientDTO? Client {get;set;}
        public List<AsisstantOfferDTO>? AssistantOffer { get; set;}

    }
}