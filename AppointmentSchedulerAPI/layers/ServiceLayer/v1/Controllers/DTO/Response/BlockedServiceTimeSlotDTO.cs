using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeSlotLock.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class BlockedServiceTimeSlotDTO
    {
        public DateTimeRange? TotalServicesTimeRange { get; set; }
        public List<ServiceTimeSlot>? ServicesSelected { get; set;}
        public DateTime? LockExpirationTime { get; set;}
    }
}