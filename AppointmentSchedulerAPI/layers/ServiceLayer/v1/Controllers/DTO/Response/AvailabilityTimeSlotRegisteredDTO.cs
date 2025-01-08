namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AvailabilityTimeSlotRegisteredDTO
    {
        public AssistantAvailabilityTimeSlotDTO? Assistant { get; set; }
        public AvailabilityTimeSlotDTO? AvailabilityTimeSlot { get; set; }
    }
}