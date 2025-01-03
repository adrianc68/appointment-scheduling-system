using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class ServiceOffer
    {
        public Assistant? Assistant { get; set; }
        public Service? Service { get; set; }
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        public TimeOnly? ServiceStartTime { get; set; }
        public TimeOnly? ServiceEndTime { get; set; }
        public string? ServiceName { get; set; }
        public int? ServicesMinutes { get; set; }
        public double? ServicePrice { get; set; }

        public ServiceOfferStatusType? Status { get; set; }
    }
}