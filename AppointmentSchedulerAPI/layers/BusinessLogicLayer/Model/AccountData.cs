using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class AccountData
    {
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public RoleType? Role { get; set; }
        public AccountStatusType? Status { get; set; }
        private DateTime? _createdAt;
        public DateTime? CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }
    }
}