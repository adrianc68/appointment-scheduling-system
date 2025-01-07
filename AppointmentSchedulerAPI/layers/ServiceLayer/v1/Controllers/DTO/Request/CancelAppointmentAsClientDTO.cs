using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CancelAppointmentAsClientDTO
    {
        [Required(ErrorMessage = "Uuid is required.")]
        public required Guid AppointmentUuid { get; set; }
        // Get from the Authentication Service
        // $$$> THIS MUST BE REMOVED FROM HERE
        public required Guid ClientUuid { get; set; }
    }
}