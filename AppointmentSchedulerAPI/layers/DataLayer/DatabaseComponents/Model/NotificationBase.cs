using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class NotificationBase
{
  public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
  public int? Id { get; set; }
  public Guid? Uuid { get; set; }
  public NotificationStatusType? Status { get; set; } = NotificationStatusType.UNREAD;
  public string? Message { get; set; }
  public NotificationCodeType? Code { get; set; }
  public NotificationType? Type { get; set; }
  public int IdUserAccount { get; set; }
  public virtual UserAccount? UserAccount { get; set; }
  public virtual AppointmentNotification? AppointmentNotification { get; set; }
}
