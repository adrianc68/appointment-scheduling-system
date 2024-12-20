using System;
using System.Collections.Generic;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class AvailabilityTimeSlot
{
    public int Id { get; set; }

    public Guid? Uuid { get; set; }

    public string? Date { get; set; }

    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int IdAssistant { get; set; }

    public virtual Assistant IdAssistantNavigation { get; set; } = null!;
}
