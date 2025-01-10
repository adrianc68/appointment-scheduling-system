namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class UnavailableTimeSlot
{
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int IdAvailabilityTimeSlot { get; set;}
    public virtual AvailabilityTimeSlot? AvailabilityTimeSlot { get; set; }
}
