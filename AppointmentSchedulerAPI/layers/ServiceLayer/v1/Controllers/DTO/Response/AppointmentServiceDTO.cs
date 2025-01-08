using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class AppointmentServiceDTO
    {
        public AssistantScheduledServiceDTO? Assistant { get; set; }
        public ScheduledServiceDTO? Service { get; set; }

    }
}