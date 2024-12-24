using System;
using System.Collections.Generic;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class Assistant
{
    public int IdUserAccount { get; set; }
    public AssistantStatusType Status { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<AssistantService> AssistantServices { get; set; } = new List<AssistantService>();
    public ICollection<AvailabilityTimeSlot> AvailabilityTimeSlots { get; set; } = new List<AvailabilityTimeSlot>();
    public virtual UserAccount UserAccount { get; set; } = null!;
}
