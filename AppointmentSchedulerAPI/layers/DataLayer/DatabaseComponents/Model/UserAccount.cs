using System;
using System.Collections.Generic;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class UserAccount
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Username { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? Id { get; set; }
    public Guid? Uuid { get; set; }
    public RoleType? Role {get; set;}

    public virtual UserInformation? UserInformation { get; set; }
    public virtual Assistant? Assistant { get; set; }
    public virtual Client? Client { get; set; }
}
