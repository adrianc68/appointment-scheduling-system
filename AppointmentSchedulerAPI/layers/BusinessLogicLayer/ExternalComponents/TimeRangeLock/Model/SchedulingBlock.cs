using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Model
{
    public class SchedulingBlock
    {
        public required Guid ClientUuid { get; set; }
        public DateTimeRange? Range { get; set; }
        public List<ServiceWithTime> Services { get; set; } = new();
        public required Timer Timer { get; set; }
    }
}