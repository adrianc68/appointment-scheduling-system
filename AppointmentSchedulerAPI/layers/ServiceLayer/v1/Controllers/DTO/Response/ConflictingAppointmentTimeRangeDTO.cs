namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ConflictingAppointmentTimeRangeDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}