using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class Appointment
    {
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
        public AppointmentStatusType? Status { get; set; }
        public double? TotalCost { get; set; }
        private DateTime? _createdAt;
        public DateTime? CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }
        public Client? Client { get; set; } = new Client();
        public List<ScheduledService>? ScheduledServices { get; set; }
    }
}