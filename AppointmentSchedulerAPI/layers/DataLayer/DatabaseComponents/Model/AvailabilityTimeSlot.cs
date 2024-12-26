namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class AvailabilityTimeSlot
{
    public int? Id { get; set; }
    public Guid? Uuid { get; set; }
    public DateOnly? Date { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? IdAssistant { get; set; }
    public virtual Assistant? Assistant { get; set; } = null!;
}
