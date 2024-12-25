namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO
{
    public class ScheduleAppointmentAsClientDTO
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}