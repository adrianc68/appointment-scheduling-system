using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AvailabilityTimeSlotDTO
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required Guid Uuid { get; set; }
        public required List<UnavailableTimeSlotDTO> UnavailableTimeSlots {get; set;} = [];
        public required AvailabilityTimeSlotStatusType Status { get; set; }
        public required AssistantAvailabilityTimeSlotDTO Assistant { get; set; }
    }
}