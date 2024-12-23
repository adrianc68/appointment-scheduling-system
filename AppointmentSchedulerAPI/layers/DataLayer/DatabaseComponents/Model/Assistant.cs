using System;
using System.Collections.Generic;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class Assistant
{
    public Guid? Uuid { get; set; }

    public int IdUser { get; set; }

    public int Id { get; set; }
    public AssistantStatusType Status { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<AssistantService> AssistantServices { get; set; } = new List<AssistantService>();
    public ICollection<AvailabilityTimeSlot> AvailabilityTimeSlots { get; set; } = new List<AvailabilityTimeSlot>();
    public virtual UserAccount UserAccount { get; set; } = null!;
}
