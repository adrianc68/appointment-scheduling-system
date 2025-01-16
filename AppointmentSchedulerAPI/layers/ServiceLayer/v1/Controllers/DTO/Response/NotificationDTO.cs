using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class NotificationDTO
    {
        public required DateTime CreatedAt { get; set; }
        public required Guid Uuid { get; set; }
        public required NotificationStatusType Status { get; set; } 
        public required string Message { get; set; }
        public required NotificationType Type { get; set;}
    }
}