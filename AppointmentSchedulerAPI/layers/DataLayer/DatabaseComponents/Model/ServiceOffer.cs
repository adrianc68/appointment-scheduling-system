using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model
{
    public partial class ServiceOffer
    {
        public int? IdAssistant { get; set; }

        public int? IdService { get; set; }
        public Guid? Uuid { get; set; }
        public int Id { get; set; }
        public ServiceOfferStatusType Status {get; set;}
        public virtual Assistant Assistant { get; set; }
        public virtual Service Service { get; set; }
        public virtual IEnumerable<ScheduledService> ScheduledServices { get; set; }

    }
}
