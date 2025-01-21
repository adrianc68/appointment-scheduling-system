using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class UpdateAvailabilityTimeSlotDTO
    {
        [Required(ErrorMessage = "Uuid is required.")]
        public required Guid Uuid { get; set; }
        [Required(ErrorMessage = "Date is required.")]
        public required DateOnly Date { get; set; }
        [Required(ErrorMessage = "EndTime is required.")]
        public required TimeOnly EndTime { get; set; }
        [Required(ErrorMessage = "StartTime is required.")]
        public required TimeOnly StartTime { get; set; }
        [ValidTimeSlot]
        public List<UnavailableTimeSlotDTO>? UnavailableTimeSlots { get; set; }
    }
}