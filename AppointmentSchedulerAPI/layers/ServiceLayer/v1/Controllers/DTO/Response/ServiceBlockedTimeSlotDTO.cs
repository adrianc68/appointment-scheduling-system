namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ServiceBlockedTimeSlotDTO
    {
        public Guid Uuid { get; set; }
        public DateTime StartDate { get; set;}
        public DateTime EndDate { get; set;}
    }
}