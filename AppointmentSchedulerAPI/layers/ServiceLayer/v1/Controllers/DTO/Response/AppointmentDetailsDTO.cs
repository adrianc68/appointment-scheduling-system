using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AppointmentDetailsDTO
    {
        public required Guid Uuid { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public AppointmentStatusType? Status { get; set; }
        public Double? TotalCost { get; set; }
        public required DateTime CreatedAt { get; set; }
        public ClientAppointmentDTO? Client { get; set; }
        // public required List<AsisstantOfferDTO> Assistants { get; set; } = [];
        // public List<ScheduledServiceDTO> SelectedServices { get; set; } = [];
        public List<AppointmentServiceDTO>? ScheduledServices { get; set; }

    }
}