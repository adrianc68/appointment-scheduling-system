using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class BlockedTimeSlot
    {
        public Guid? ClientUuid { get; set; }
        public DateTimeRange? TotalServicesTimeRange { get; set; }
        public DateTime LockExpirationTime { get; set; }
        public List<ServiceTimeSlot> SelectedServices { get; set; } = new();
        public Timer? LockTimer { get; set; }
    }
}