using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Interfaces
{
    public interface ITimeRangeLockMgt
    {
        OperationResult<DateTime, GenericError> BlockTimeRange(List<ServiceWithTime> selectedServices, DateTimeRange range, Guid accountUuid);
        OperationResult<bool, GenericError> UnblockTimeRange(Guid clientUuid);
        OperationResult<bool, GenericError> ExtendTimeRange(DateTimeRange newRange, Guid clientUuid);
        OperationResult<DateTimeRange, GenericError> GetDateTimeRangeByAccountUuid(Guid clientUuid);
        OperationResult<List<SchedulingBlock>, GenericError> GetSchedulingBlockByDate(DateOnly date);
    }
}