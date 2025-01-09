using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model
{
    public partial class Client
    {
        public int? IdUserAccount { get; set; }
        public ClientStatusType? Status { get; set; }
        public virtual IEnumerable<Appointment>? Appointments { get; set; } 
        public virtual UserAccount? UserAccount { get; set; }
    }
}
