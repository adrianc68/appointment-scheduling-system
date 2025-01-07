using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Model
{
    public class SchedulingBlock
    {
        public Guid? ClientUuid { get; set; }
        public DateTimeRange? Range { get; set; }
        public DateTime LockEndTime { get; set; }
        public List<ServiceWithTime> Services { get; set; } = new();
        public Timer? Timer { get; set; }
    }
}