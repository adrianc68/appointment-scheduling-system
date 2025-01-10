using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CreateAvailabilityTimeSlotDTO
    {
        [Required(ErrorMessage = "Date is required.")]
        public required DateOnly Date { get; set; }
        [Required(ErrorMessage = "StartTime is required.")]
        public required TimeOnly StartTime { get; set; }
        [Required(ErrorMessage = "EndTime is required.")]
        public required TimeOnly EndTime { get; set; }
        [Required(ErrorMessage = "AssistantUuid is required.")]
        public required Guid AssistantUuid { get; set; }

        [ValidTimeSlot]
        public List<UnavailableTimeSlotDTO>? UnavailableTimeSlots { get; set;}
    }
}