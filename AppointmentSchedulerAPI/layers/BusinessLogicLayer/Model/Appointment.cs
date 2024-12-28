using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class Appointment
    {
        public int? Id { get; set; }
        public Guid? Uuid { get; set; }
        public TimeOnly? EndTime { get; set; }
        public TimeOnly? StartTime { get; set; }
        public DateOnly? Date { get; set; }
        public AppointmentStatusType Status { get; set; }
        public double? TotalCost { get; set; }
        public DateTime? CreatedAt { get; set;}
        public Client Client { get; set; } = new Client();
        public List<Service> AssistantService { get; set; }
    }
}