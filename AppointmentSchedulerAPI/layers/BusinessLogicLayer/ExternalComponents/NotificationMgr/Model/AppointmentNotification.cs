using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model
{
    public class AppointmentNotification : NotificationBase
    {
        public virtual Appointment? Appointment { get; set; }
    }
}