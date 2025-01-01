namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class DateTimeRangeDTO
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set;}
        public DateOnly Date { get; set;}
    }
}