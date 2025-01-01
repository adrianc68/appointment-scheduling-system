namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class ServiceWithStartTime 
    {
        public Guid Uuid { get; set; }
        public TimeOnly StartTime { get; set; }
    }

    public class CreateAppointmentAsClientDTO
    {
        public DateOnly Date { get; set; }
        public Guid ClientUuid { get; set; }
        public List<ServiceWithStartTime> SelectedServices { get; set; }
    }
}