using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class ServiceOfferWithStartTime
    {
        [Required(ErrorMessage = "Uuid is required.")]
        public required Guid Uuid { get; set; }
        [Required(ErrorMessage = "StartTime is required.")]
        public required TimeOnly StartTime { get; set; }
    }
}