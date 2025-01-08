namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ConflictingServiceOfferDTO
    {
        public AssistantConflictingAppointmentDTO? Assistant { get; set; } 
        public ConflictingAppointmentTimeRangeDTO? TimeRange { get; set;}
        public ConflictingServiceOfferDataDTO? ScheduledService { get; set;}
    }
}