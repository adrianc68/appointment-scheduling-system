using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class Appointment
{
    public DateTime StartDate { get; set; } 
    public DateTime EndDate { get; set; } 
    public double? TotalCost { get; set; }
    public Guid? Uuid { get; set; }
    public int? Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? IdClient { get; set; }
    public AppointmentStatusType? Status { get; set; }
    public virtual Client? Client { get; set; }
    public virtual IEnumerable<ScheduledService>? ScheduledServices { get; set; }
    public virtual IEnumerable<AppointmentNotification>? AppointmentNotifications { get; set; }
}
