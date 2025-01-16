using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class ServiceOffer
    {
        public Assistant? Assistant { get; set; }
        public Service? Service { get; set; }
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        public ServiceOfferStatusType? Status { get; set; }
        public List<ScheduledService>? ScheduledServices { get; set; }
    }
}