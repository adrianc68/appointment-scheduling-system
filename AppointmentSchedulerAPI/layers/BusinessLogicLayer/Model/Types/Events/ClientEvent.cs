namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events
{
    public class ClientEvent
    {
        public ClientEventType EventType { get; set; }
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
        public int? ClientId { get; set; }
        public Guid? ClientUuid { get; set; }
    }
}