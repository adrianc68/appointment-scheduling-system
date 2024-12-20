using System;
using System.Collections.Generic;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class Appointment
{
    public DateTime? Date { get; set; }

    public TimeOnly? EndTime { get; set; }

    public TimeOnly? StartTime { get; set; }

    public decimal? TotalCost { get; set; }

    public Guid? Uuid { get; set; }

    public int Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? IdClient { get; set; }

    public int? IdAssistant { get; set; }

    public virtual Assistant? IdAssistantNavigation { get; set; }

    public virtual Client? IdClientNavigation { get; set; }
}
