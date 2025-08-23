using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class Service
    {
        public int? Id { get; set; }
        public string? Description { get; set; }
        public int? Minutes { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public Guid? Uuid { get; set; }
        public ServiceStatusType? Status { get; set; }
        private DateTime? _createdAt;
        public DateTime? CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }
        public List<ServiceOffer>? ServiceOffers { get; set; }
    }
}