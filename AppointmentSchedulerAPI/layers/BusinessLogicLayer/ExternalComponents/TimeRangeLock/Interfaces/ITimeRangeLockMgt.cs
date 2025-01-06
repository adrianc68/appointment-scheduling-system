using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Interfaces
{
    public interface ITimeRangeLockMgt
    {
        OperationResult<DateTime, GenericError> BlockTimeRange(DateTimeRange range, Guid accountUuid);
        OperationResult<bool, GenericError> UnblockTimeRange(Guid accountUuid);
        OperationResult<DateTimeRange, GenericError> GetDateTimeRangeByAccountUuid(Guid accountUuid);
        OperationResult<Guid, GenericError> GetAccountUuidByDateTimeRange(DateTimeRange range);
    }
}