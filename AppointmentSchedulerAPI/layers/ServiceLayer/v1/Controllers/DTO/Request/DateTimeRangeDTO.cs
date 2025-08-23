using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class DateTimeRangeDTO
    {
        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "EndDate is required.")]
        public DateTime? EndDate { get; set; }
    }
}