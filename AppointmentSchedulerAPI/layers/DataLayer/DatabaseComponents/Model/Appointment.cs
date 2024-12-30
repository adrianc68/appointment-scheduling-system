using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class Appointment
{
    public DateOnly? Date { get; set; }
    public TimeOnly? EndTime { get; set; }
    public TimeOnly? StartTime { get; set; }
    public double? TotalCost { get; set; }
    public Guid? Uuid { get; set; }
    public int? Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? IdClient { get; set; }
    public AppointmentStatusType? Status { get; set; }
    public virtual Client Client { get; set; }
    public virtual IEnumerable<AppointmentServiceOffer> AppointmentServiceOffers { get; set; }
}
