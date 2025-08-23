namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class ScheduledService
    {
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }

        private DateTime? _serviceStartDate;
        public DateTime? ServiceStartDate
        {
            get => _serviceStartDate;
            set => _serviceStartDate = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }

        private DateTime? _serviceEndDate;
        public DateTime? ServiceEndDate
        {
            get => _serviceEndDate;
            set => _serviceEndDate = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }

        public string? ServiceName { get; set; }
        public int? ServicesMinutes { get; set; }
        public double? ServicePrice { get; set; }
        public Appointment? Appointment { get; set; }
        public ServiceOffer? ServiceOffer { get; set; }
    }

}