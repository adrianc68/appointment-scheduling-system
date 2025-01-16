
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class GeneralNotification
{
    public int? IdNotificationBase { get; set; } 
    public GeneralNotificationCodeType? Code { get; set; }
    public virtual NotificationBase? NotificationBase { get; set; }

}
