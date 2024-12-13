using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.SchedulingInterfaces
{
    public interface IDeleteAvailabilityTimeSlot
    {
        bool DeleteAvailabilityTimeSlot(int idAvailabilityTimeSlot);
    }
}