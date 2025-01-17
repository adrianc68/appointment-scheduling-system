using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class GeneralNotification : NotificationBase
    {
        public GeneralNotificationCodeType? Code { get; set; }
    }
}