using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model
{
    public class SystemNotification : NotificationBase
    {
        public SystemNotificationCodeType? Code { get; set; }
        public SystemNotificationSeverityCodeType? Severity { get; set;}
    }
}