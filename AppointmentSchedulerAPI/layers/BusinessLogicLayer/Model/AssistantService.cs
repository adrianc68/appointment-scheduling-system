namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model
{
    public class AssistantService
    {
        public Guid Uuid { get; set; }
        public Assistant Assistant { get; set; } = new Assistant();
        public List<Service> Services { get; set;} = new List<Service>();
    }
}