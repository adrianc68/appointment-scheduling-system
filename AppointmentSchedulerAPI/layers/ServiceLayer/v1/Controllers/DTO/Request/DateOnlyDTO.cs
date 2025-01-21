using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class DateOnlyDTO
    {
        [Required(ErrorMessage = "Date is required.")]
        public DateOnly Date { get; set; }
    }
}