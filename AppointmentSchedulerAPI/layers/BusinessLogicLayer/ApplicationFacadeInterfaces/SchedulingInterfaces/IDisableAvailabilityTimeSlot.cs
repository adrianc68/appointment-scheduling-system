using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IDisableAvailabilityTimeSlot
    {
        Task<OperationResult<bool, GenericError>> DisableAvailabilityTimeSlotAsync(Guid uuid);
    }
}