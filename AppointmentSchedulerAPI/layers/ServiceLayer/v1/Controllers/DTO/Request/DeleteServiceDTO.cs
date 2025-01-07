using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.ValidationAttributes;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class DeleteServiceDTO
    {
        [Required(ErrorMessage = "Uuid is required.")]
        public required Guid Uuid { get; set; }
    }
}