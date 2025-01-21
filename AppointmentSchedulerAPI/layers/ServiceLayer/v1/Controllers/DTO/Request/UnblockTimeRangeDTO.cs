using System.ComponentModel.DataAnnotations;

public class UnblockTimeRangeDTO
{
    [Required(ErrorMessage = "ClientUuid is required.")]
    public required Guid ClientUuid { get; set; }
}