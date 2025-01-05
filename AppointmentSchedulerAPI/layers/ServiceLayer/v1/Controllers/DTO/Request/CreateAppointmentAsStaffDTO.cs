namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CreateAppointmentAsStaffDTO
    {
        public DateOnly Date { get; set; }
        public Guid clientUuid { get; set; }
        public required List<ServiceOfferWithStartTime> SelectedServices { get; set; }
    }
}