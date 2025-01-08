namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ServiceBlockedTimeSlotDTO
    {
        public Guid Uuid { get; set; }
        public TimeOnly StartTime { get; set;}
        public TimeOnly EndTime { get; set;}
    }
}