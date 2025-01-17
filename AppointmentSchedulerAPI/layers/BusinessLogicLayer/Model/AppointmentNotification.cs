using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class AppointmentNotification : NotificationBase
    {
        public AppointmentNotification()
        {
            this.Type = NotificationType.APPOINTMENT_NOTIFICATION;
        }

        public AppointmentNotificationCodeType? Code { get; set; }
        public virtual Appointment? Appointment { get; set; }
    }
}