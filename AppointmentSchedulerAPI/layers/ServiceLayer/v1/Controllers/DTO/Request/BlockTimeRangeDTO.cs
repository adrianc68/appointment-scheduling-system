using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request;

public class BlockTimeRangeDTO
{
    [Required(ErrorMessage = "SelectedServices cannot be empty.")]
    [MinLength(1, ErrorMessage = "You must select at least one service.")]
    [UniqueUuidListValidation(ErrorMessage = "The SelectedServices must have unique UUIDs.")]
    public required List<ServiceOfferWithStartTime> SelectedServices { get; set; }


    [Required(ErrorMessage = "ClientUuid is required.")]
    public required Guid ClientUuid { get; set; }
}