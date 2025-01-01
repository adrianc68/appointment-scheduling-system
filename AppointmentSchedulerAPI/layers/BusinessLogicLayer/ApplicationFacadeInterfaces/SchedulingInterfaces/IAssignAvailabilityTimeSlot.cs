using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IAssignAvailabilityTimeSlot
    {
        Task<OperationResult<Guid,GenericError>> RegisterAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot);
    }
}