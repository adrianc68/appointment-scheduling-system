using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class ServiceOfferDTO
    {        
        public string? Name { get; set; }
        public double? Price { get; set; }
        public int? Minutes { get; set; }
        public string? Description { get; set; }
        public Guid? Uuid { get; set; }
        public Guid? ServiceUuid { get; set; }
        public ServiceStatusType? ServiceStatus { get; set;}
        public ServiceOfferStatusType Status { get; set;}
    }
}