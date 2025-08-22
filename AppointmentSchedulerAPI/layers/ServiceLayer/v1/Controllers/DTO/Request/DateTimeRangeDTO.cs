using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class DateTimeRangeDTO
    {
        [Required(ErrorMessage = "StartTime is required.")]
        public required DateTime StartDate { get; set; }
        [Required(ErrorMessage = "EndTime is required.")]
        public required DateTime EndDate { get; set; }
    }
}