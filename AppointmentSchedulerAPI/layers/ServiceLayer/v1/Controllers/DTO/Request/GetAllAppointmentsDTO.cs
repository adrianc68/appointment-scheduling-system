namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class GetAllAppointmentsDTO
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}