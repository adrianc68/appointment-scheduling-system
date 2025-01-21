
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class SystemNotification
{
    public int? IdNotificationBase { get; set; } 
    public SystemNotificationCodeType? Code { get; set; }
    public SystemNotificationSeverityCodeType? Severity { get; set; }

    public virtual NotificationBase? NotificationBase { get; set; }

}
