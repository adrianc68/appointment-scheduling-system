using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.SchedulingInterfaces
{
    public interface IEditAvailabilityTimeSlot
    {
        bool EditAvailabilityTimeSlot(AvailabilityTimeSlot availabilityTimeSlot);
    }
}