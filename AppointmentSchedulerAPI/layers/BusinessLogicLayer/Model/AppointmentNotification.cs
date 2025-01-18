using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class AppointmentNotification : Notification
    {
        public AppointmentNotification()
        {
            this.Type = NotificationType.APPOINTMENT_NOTIFICATION;
        }

        public required AppointmentNotificationCodeType Code { get; set; }
        public required virtual Appointment Appointment { get; set; }
    }
}