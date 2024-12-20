using System;
using System.Collections.Generic;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class AppointmentService
{
    public int? IdService { get; set; }

    public int? IdAppointment { get; set; }

    public virtual Appointment? IdAppointmentNavigation { get; set; }

    public virtual Service? IdServiceNavigation { get; set; }
}
