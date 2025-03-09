using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AccountDataDTO
    {
        public required Guid Uuid { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required RoleType Role { get; set; }
        public required AccountStatusType Status { get; set;}
        public required DateTime CreatedAt { get; set; }
    }
}