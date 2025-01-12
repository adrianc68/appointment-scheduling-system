namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events
{
    public class AccountEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public AccountEventType EventType { get; set; }
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
        public int? AccountId { get; set; }
        public Guid AccountUuid { get; set; }
    }
}