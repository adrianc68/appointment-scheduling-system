using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.SchedulingInterfaces
{
    public interface IAssignAvailabilityTimeSlot
    {
        bool RegisterAvailabilityTimeSlot(DateTimeRange range, int idAssistant);
    }
}