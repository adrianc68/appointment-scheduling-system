namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class UnavailableTimeSlot
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int IdAvailabilityTimeSlot { get; set; }
    public virtual AvailabilityTimeSlot? AvailabilityTimeSlot { get; set; }
}
