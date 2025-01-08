using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeSlotLock.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class BlockedServiceTimeSlotDTO
    {
        public DateTimeRange? BlockedRange { get; set; }
        public List<AssistantServiceBlockedTimeSlotDataDTO>? BlockedServices { get; set;}
        public DateTime? LockExpirationTime { get; set;}
    }
}