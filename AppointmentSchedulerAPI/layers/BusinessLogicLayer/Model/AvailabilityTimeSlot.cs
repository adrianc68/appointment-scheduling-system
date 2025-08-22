using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class AvailabilityTimeSlot
    {
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AvailabilityTimeSlotStatusType Status { get; set;}
        public List<UnavailableTimeSlot>? UnavailableTimeSlots { get; set; } = [];

        public List<Service>? Services { get; set; }
        public Assistant? Assistant { get; set; }

        public AvailabilityTimeSlot() { }

    }
}