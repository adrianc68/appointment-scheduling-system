using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class AvailabilityTimeSlot
    {
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        private DateTime _createdAt;
        public DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = DateTime.SpecifyKind(value, DateTimeKind.Utc) ;

        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public AvailabilityTimeSlotStatusType Status { get; set; }

        public List<UnavailableTimeSlot>? UnavailableTimeSlots { get; set; } = new List<UnavailableTimeSlot>();
        public List<Service>? Services { get; set; } = new List<Service>();

        public Assistant? Assistant { get; set; }

        public AvailabilityTimeSlot() { }
    }
}