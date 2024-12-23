namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model
{
    public partial class Client
    {
        public int IdUserAccount { get; set; }
        public ClientStatusType Status { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual UserAccount UserAccount { get; set; } = null!;
    }
}
