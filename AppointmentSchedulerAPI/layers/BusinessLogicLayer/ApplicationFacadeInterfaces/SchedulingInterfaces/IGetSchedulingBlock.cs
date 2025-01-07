using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeSlotLock.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IGetSchedulingBlock
    {
        OperationResult<List<BlockedTimeSlot>, GenericError> GetSchedulingBlockRanges(DateOnly date);
    }
}