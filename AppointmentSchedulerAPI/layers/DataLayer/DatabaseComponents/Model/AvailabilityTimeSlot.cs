using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class AvailabilityTimeSlot
{
    public int Id { get; set; }
    public Guid Uuid { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public AvailabilityTimeSlotStatusType Status { get; set; }
    public int? IdAssistant { get; set; }
    public virtual Assistant? Assistant { get; set; }
    public virtual List<UnavailableTimeSlot>? UnavailableTimeSlots { get; set;}
}

