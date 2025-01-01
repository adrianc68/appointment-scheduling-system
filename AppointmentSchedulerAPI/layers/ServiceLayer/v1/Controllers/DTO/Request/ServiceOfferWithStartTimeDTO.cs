namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class ServiceOfferWithStartTime
    {
        public Guid Uuid { get; set; }
        public TimeOnly StartTime { get; set; }
    }
}