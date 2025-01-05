using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IEditAvailabilityTimeSlot
    {
        Task<OperationResult<bool, GenericError>> EditAvailabilityTimeSlot(AvailabilityTimeSlot availabilityTimeSlot);
    }
}