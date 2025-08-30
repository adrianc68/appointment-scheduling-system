namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class UnavailableTimeSlotDTO
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }
}