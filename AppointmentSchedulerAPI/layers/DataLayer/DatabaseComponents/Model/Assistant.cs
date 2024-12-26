using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class Assistant
{
    public int? IdUserAccount { get; set; }
    public AssistantStatusType Status { get; set; }

    public virtual IEnumerable<AppointmentAssistant> AppointmentAssistants { get; set; } = new List<AppointmentAssistant>();
    public virtual IEnumerable<AssistantService> AssistantServices { get; set; } = new List<AssistantService>();
    public virtual IEnumerable<AvailabilityTimeSlot> AvailabilityTimeSlots { get; set; } = new List<AvailabilityTimeSlot>();
    public virtual UserAccount UserAccount { get; set; }
}
