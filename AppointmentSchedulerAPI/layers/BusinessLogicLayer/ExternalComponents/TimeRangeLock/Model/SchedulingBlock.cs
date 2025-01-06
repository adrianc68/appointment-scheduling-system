using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Model
{
    public class SchedulingBlock
    {
        public Guid AccountUuid { get; set; }
        public required DateTimeRange Range { get; set; }
        public required Timer Timer { get; set; }
    }
}