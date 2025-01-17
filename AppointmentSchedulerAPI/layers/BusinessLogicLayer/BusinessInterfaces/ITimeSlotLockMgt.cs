using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface ITimeSlotLockMgt
    {
        OperationResult<DateTime, GenericError> BlockTimeSlot(List<ServiceTimeSlot> selectedServices, DateTimeRange range, Guid clientUuid);
        OperationResult<bool, GenericError> UnblockTimeSlot(Guid clientUuid);
        OperationResult<bool, GenericError> ExtendBlockedTimeSlot(DateTimeRange newRange, Guid clientUuid);
        OperationResult<BlockedTimeSlot, GenericError> GetBlockedTimeSlotByClientUuid(Guid clientUuid);
        OperationResult<List<BlockedTimeSlot>, GenericError> GetBlockedTimeSlotsByDate(DateOnly date);
    }
}