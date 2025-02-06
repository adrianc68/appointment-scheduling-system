using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AppointmentDetailsDTO
    {
        public required Guid Uuid { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }
        public required DateOnly Date { get; set; }
        public AppointmentStatusType? Status { get; set; }
        public Double? TotalCost { get; set; }
        public required DateTime CreatedAt { get; set; }
        public ClientAppointmentDTO? Client {get;set;}
        public required List<AsisstantOfferDTO> Assistants { get; set;} = [];

    }
}