using System;
using System.Collections.Generic;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class UserAccount
{
    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Username { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int Id { get; set; }

    // public string? Role {get; set;}
    public virtual ICollection<Assistant> Assistants { get; set; } = new List<Assistant>();

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
