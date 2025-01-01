using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class ServiceOffer
    {
        public Assistant? Assistant { get; set; }
        public Service? Service { get; set; }
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}