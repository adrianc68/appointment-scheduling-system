using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request;

public class BlockTimeRangeDTO
{
    [Required(ErrorMessage = "Date is required.")]
    public required DateOnly Date { get; set; }
    [Required(ErrorMessage = "StartTime is required")]
    public required TimeOnly StartTime { get; set; }
    [Required(ErrorMessage = "SelectedServices cannot be empty.")]
    [MinLength(1, ErrorMessage = "You must select at least one service.")]
    [UniqueGuidListValidation(ErrorMessage = "The SelectedServices must have unique UUIDs.")]
    public required List<Guid> SelectedServices { get; set; }
    [Required(ErrorMessage = "AccountUuid is required.")]
    public required Guid AccountUuid { get; set; }
}