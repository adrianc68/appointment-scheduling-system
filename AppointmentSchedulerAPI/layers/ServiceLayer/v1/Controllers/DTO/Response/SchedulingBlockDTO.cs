using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class SchedulingBlockDTO
    {
        public DateTimeRange? Range { get; set; }
        public List<ServiceWithTime>? Services { get; set;}
        public DateTime? LockEndtime { get; set;}
    }
}