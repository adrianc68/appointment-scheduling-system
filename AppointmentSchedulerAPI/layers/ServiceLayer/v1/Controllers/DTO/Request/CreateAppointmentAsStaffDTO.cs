namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CreateAppointmentAsStaffDTO
    {
        public DateOnly Date { get; set; }
        public Guid ClientUuid { get; set; }
        [UniqueUuidListValidation]
        public required List<ServiceOfferWithStartTime> SelectedServices { get; set; }
    }
}