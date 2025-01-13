using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CreateAppointmentAsClientDTO
    {
        [Required(ErrorMessage = "Date is required.")]
        public required DateOnly Date { get; set; }
        [Required(ErrorMessage = "SelectedServices cannot be empty.")]
        [MinLength(1, ErrorMessage = "You must select at least one service.")]
        [UniqueUuidListValidation(ErrorMessage = "The selected services must have unique UUIDs.")]
        public required List<ServiceOfferWithStartTime> SelectedServices { get; set; }
    }
}