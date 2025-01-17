using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class SystemNotification : NotificationBase
    {
        public SystemNotification()
        {
            this.Type = NotificationType.SYSTEM_NOTIFICATION;
        }

        public required SystemNotificationCodeType? Code { get; set; }
        public required SystemNotificationSeverityCodeType? Severity { get; set;}
        
        
    }
}