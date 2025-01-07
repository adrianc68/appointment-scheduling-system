using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CancelAppointmentAsStaffDTO
    {
        [Required(ErrorMessage = "AppointmentUuid is required.")]
        public required Guid AppointmentUuid { get; set; }
    }
}