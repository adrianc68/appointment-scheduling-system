using System;
using System.Collections.Generic;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class Appointment
{
    public DateTime? Date { get; set; }
    public TimeOnly? EndTime { get; set; }
    public TimeOnly? StartTime { get; set; }
    public decimal? TotalCost { get; set; }
    public Guid? Uuid { get; set; }
    public int? Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? IdClient { get; set; }
    public int? IdAssistant { get; set; }
    public AppointmentStatusType? Status { get; set; }
    public virtual Assistant? Assistant { get; set; }
    public virtual Client? Client { get; set; }
    public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
}
