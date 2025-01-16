using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model
{
    public abstract class NotificationBase
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        public NotificationStatusType? Status { get; set; } = NotificationStatusType.UNREAD;
        public required string Message { get; set; }
        public NotificationCodeType Code { get; set;}
        public NotificationType Type { get; set;}
        public AccountData? Recipient { get; set; }


    }
}