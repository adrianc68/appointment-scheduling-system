namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class Assistant
{
    public int? IdUserAccount { get; set; }
    public virtual IEnumerable<ServiceOffer>? ServiceOffers { get; set; } 
    public virtual IEnumerable<AvailabilityTimeSlot>? AvailabilityTimeSlots { get; set; } 
    public virtual UserAccount? UserAccount { get; set; }
}
