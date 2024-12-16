using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IAssignAvailabilityTimeSlot
    {
        bool RegisterAvailabilityTimeSlot(DateTimeRange range, int idAssistant);
    }
}