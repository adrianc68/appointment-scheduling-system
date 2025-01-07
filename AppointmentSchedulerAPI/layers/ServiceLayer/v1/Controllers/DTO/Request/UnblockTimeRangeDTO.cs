using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

public class UnblockTimeRangeDTO
{
    [Required(ErrorMessage = "AccountUuid is required.")]
    public required Guid AccountUuid { get; set; }
}