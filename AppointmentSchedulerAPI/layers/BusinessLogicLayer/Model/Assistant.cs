using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class Assistant
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? Username { get; set; }
        public DateTime? CreatedAt { get; set; }
        public AccountStatusType? Status { get; set; }
        public Guid? Uuid { get; set; }

        public List<Service>? Services { get; set; }

        public Assistant() { }

    }
}