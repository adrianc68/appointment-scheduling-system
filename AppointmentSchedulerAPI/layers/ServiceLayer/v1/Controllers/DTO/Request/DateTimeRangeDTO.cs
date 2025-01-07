using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class DateTimeRangeDTO
    {
        [Required(ErrorMessage = "StartTime is required.")]
        public required TimeOnly StartTime { get; set; }
        [Required(ErrorMessage = "EndTime is required.")]
        public required TimeOnly EndTime { get; set; }
        [Required(ErrorMessage = "Date is required.")]
        public required DateOnly Date { get; set; }
    }
}