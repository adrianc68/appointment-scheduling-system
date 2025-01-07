using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

public class UnblockTimeRangeDTO
{
    [Required(ErrorMessage = "ClientUuid is required.")]
    public required Guid ClientUuid { get; set; }
}