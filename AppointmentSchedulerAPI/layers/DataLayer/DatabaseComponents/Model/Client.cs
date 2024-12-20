using System;
using System.Collections.Generic;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class Client
{
    public Guid? Uuid { get; set; }

    public int IdUser { get; set; }

    public int Id { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual UserAccount IdUserNavigation { get; set; } = null!;
}
