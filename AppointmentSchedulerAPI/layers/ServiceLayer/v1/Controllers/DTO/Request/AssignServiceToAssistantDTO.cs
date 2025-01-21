using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class AssignServiceToAssistantDTO
    {
        [Required(ErrorMessage = "AssistantUuid is required.")]
        public required Guid AssistantUuid { get; set; }
        [Required(ErrorMessage = "SelecteServices cannot be empty.")]
        [MinLength(1, ErrorMessage = "You must select at least one service.")]
        [UniqueUuidListValidation(ErrorMessage = "The selected services must have unique UUIDs.")]
        public required List<Guid> SelectedServices { get; set; }
    }
}