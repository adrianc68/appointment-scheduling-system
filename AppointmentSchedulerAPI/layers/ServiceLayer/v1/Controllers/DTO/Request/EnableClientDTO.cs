using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class EnableClientDTO
    {
        [Required(ErrorMessage = "Uuid is required.")]
        public required Guid Uuid { get; set; }
    }
}