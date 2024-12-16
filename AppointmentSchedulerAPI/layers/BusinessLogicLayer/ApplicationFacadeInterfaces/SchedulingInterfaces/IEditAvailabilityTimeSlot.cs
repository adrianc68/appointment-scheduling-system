using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IEditAvailabilityTimeSlot
    {
        bool EditAvailabilityTimeSlot(AvailabilityTimeSlot availabilityTimeSlot);
    }
}