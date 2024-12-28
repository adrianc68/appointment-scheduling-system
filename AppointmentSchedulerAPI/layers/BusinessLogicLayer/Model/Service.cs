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
        public DateTime? CreatedAt { get; set; }

        public List<AssistantService>? AssistantServices { get; set; }
        public List<Appointment>? Appointments { get; set; }
    }
}