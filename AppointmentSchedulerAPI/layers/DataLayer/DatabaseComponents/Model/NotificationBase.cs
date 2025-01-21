using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class NotificationBase
{
  public DateTime? CreatedAt { get; set; }
  public int? Id { get; set; }
  public Guid? Uuid { get; set; }
  public string? Message { get; set; }
  public NotificationType? Type { get; set; }
  public virtual IEnumerable<NotificationRecipient>? NotificationRecipients { get; set; }
  public virtual AppointmentNotification? AppointmentNotification { get; set; }
  public virtual SystemNotification? SystemNotification { get; set; }
  public virtual GeneralNotification? GeneralNotification { get; set; }
}
