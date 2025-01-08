using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;

public class AssistantServiceBlockedTimeSlotDataDTO
{
    public AssistantBlockedTimeSlotDTO? Assistant { get; set;}
    public ServiceBlockedTimeSlotDTO? Service { get; set;}
}