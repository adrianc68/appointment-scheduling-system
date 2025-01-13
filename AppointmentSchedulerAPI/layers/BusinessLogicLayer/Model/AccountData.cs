using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class AccountData
    {
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? Username { get; set; }
        public RoleType? Role { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}