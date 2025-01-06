namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CreateAppointmentAsClientDTO
    {
        public DateOnly Date { get; set; }
        public Guid clientUuid { get; set; }
        [UniqueUuidListValidation]
        public required List<ServiceOfferWithStartTime> SelectedServices { get; set; }
    }
}