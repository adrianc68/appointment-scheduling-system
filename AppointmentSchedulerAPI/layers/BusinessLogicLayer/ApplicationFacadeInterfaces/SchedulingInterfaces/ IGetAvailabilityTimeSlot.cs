using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IGetAvailabilityTimeSlot
    {
        Task<IEnumerable<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlots(DateOnly startDate, DateOnly endDate);
    }
}