using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class GeneralNotification : Notification
    {
        public GeneralNotification()
        {
            this.Type = NotificationType.GENERAL_NOTIFICATION;
        }

        public required GeneralNotificationCodeType Code { get; set; }
    }
}