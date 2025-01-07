using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class GetAllAppointmentsDTO
    {
        [Required(ErrorMessage = "StartDate is required.")]
        public required DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "EndDate is required.")]
        public required DateOnly EndDate { get; set; }
    }
}