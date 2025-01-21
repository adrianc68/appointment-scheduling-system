using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IEnableAvailabilityTimeSlot
    {
        Task<OperationResult<bool, GenericError>> EnableAvailabilityTimeSlotAsync(Guid uuid);
    }
}