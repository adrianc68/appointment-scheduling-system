using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CancelAppointmentAsClientDTO
    {
        [Required(ErrorMessage = "Uuid is required.")]
        public required Guid AppointmentUuid { get; set; }
    }
}