using System;
using System.Collections.Generic;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class AssistantService
{
    public int? IdAssistant { get; set; }

    public int? IdService { get; set; }

    public virtual Assistant? IdAssistantNavigation { get; set; }

    public virtual Service? IdServiceNavigation { get; set; }
}
