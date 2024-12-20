using System;
using System.Collections.Generic;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class UserInformation
{
    public string? Name { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Filepath { get; set; }

    public int IdUser { get; set; }


    public virtual UserAccount IdUserNavigation { get; set; } = null!;
}
