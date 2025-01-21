
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class AppointmentNotification
{
    public int? IdAppointment { get; set; }
    public int? IdNotificationBase { get; set; } 
    public AppointmentNotificationCodeType? Code { get; set; }
    public virtual Appointment? Appointment { get; set; }
    public virtual NotificationBase? NotificationBase { get; set; }

}
