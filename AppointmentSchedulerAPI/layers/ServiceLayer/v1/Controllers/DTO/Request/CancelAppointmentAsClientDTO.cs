namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CancelAppointmentAsClientDTO
    {
        public Guid AppointmentUuid { get; set; }
        // Get from the Authentication Service
        // $$$> THIS MUST BE REMOVED FROM HERE
        public Guid ClientUuid { get; set; }
    }
}