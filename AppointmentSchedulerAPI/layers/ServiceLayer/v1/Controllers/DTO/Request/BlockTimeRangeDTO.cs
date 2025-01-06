using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request;

public class BlockTimeRangeDTO
{
    [Required(ErrorMessage = "Date is required.")]
    public DateOnly Date { get; set; }
    [Required(ErrorMessage = "StartTime is required")]
    public TimeOnly StartTime { get; set; }

    [Required(ErrorMessage = "SelectedServices cannot be null or empty.")]
    [MinLength(1, ErrorMessage = "At least one service must be selected.")]
    [UniqueUuidListValidation]
    public required List<SelectedServiceDTO> SelectedServices { get; set; }
    [Required(ErrorMessage = "AccountUuid is required.")]
    public Guid AccountUuid { get; set; }
}