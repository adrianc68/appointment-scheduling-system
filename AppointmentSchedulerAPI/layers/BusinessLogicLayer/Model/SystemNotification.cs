using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class SystemNotification : NotificationBase
    {
        public SystemNotificationCodeType? Code { get; set; }
        public SystemNotificationSeverityCodeType? Severity { get; set;}
    }
}