using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class Appointment
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan StartTime { get; set; }
        public DateTime Date { get; set; }
        public AppointmentStatusType Status { get; set; }
        public double TotalCost { get; set; }

        public Appointment() { }
    }
}