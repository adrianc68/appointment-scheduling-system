using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IDeleteAvailabilityTimeSlot
    {
        Task<OperationResult<bool, GenericError>> DeleteAvailabilityTimeSlotAsync(Guid uuid);
    }
}