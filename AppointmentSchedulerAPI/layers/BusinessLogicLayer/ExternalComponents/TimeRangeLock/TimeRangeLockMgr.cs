using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Interfaces
{
    public class TimeRangeLockMgr : ITimeRangeLockMgt
    {
        private static readonly List<SchedulingBlock> schedulingBlocks = new();

        private readonly EnvironmentVariableService envService;

        private static readonly Lock scheduleLock = new();

        public TimeRangeLockMgr(EnvironmentVariableService envService)
        {
            this.envService = envService;
        }

        public OperationResult<DateTime, GenericError> BlockTimeRange(DateTimeRange range, Guid accountUuid)
        {
            lock (scheduleLock)
            {
                if (schedulingBlocks.Any(b => b.AccountUuid == accountUuid))
                {
                    var error = new GenericError($"The user {accountUuid} has already blocked a time range.", []);
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.USER_ALREADY_HAS_BLOCKED_RANGE);
                }

                if (range.StartTime >= range.EndTime)
                {
                    var error = new GenericError($"Invalid range: {range}. StartTime must be earlier than EndTime.");
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.INVALID_RANGE_TIME);
                }

                if (schedulingBlocks.Any(b => b.Range.Date == range.Date &&
                                              range.StartTime < b.Range.EndTime &&
                                              range.EndTime > b.Range.StartTime))
                {
                    var error = new GenericError($"Cannot block range. Another range overlaps.");
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.SOMEONE_ELSE_IS_SCHEDULING_IN_RANGE_TIME);
                }

                int maxSecondsLock = int.Parse(envService.Get("MAX_SECONDS_SCHEDULING_LOCK"));
                DateTime lockEndTime = DateTime.Now.AddSeconds(maxSecondsLock);

                var timer = new Timer(_ =>
                {
                    UnblockTimeRange(accountUuid);
                }, null, TimeSpan.FromSeconds(maxSecondsLock), Timeout.InfiniteTimeSpan);

                schedulingBlocks.Add(new SchedulingBlock
                {
                    AccountUuid = accountUuid,
                    Range = range,
                    Timer = timer
                });

                return OperationResult<DateTime, GenericError>.Success(lockEndTime);
            }
        }


        public OperationResult<bool, GenericError> ExtendTimeRange(DateTimeRange newRange, Guid accountUuid)
        {
            lock (scheduleLock)
            {
                if (newRange.StartTime >= newRange.EndTime)
                {
                    var error = new GenericError($"Invalid range: {newRange}. StartTime must be earlier than EndTime.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.INVALID_RANGE_TIME);
                }

                var existingBlock = schedulingBlocks.FirstOrDefault(b => b.AccountUuid == accountUuid);
                if (existingBlock == null)
                {
                    var error = new GenericError($"No existing block found for user {accountUuid}.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.NO_DATE_TIME_RANGE_LOCK_FOUND);
                }

                if (schedulingBlocks.Any(b => b.AccountUuid != accountUuid &&
                                              b.Range.Date == newRange.Date &&
                                              newRange.StartTime < b.Range.EndTime &&
                                              newRange.EndTime > b.Range.StartTime))
                {
                    var error = new GenericError($"Cannot extend range. Another range overlaps.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.SOMEONE_ELSE_IS_SCHEDULING_IN_RANGE_TIME);
                }
                return OperationResult<bool, GenericError>.Success(true);
            }
        }


        public OperationResult<bool, GenericError> UnblockTimeRange(Guid accountUuid)
        {
            lock (scheduleLock)
            {
                var block = schedulingBlocks.FirstOrDefault(b => b.AccountUuid == accountUuid);
                if (block == null)
                {
                    var error = new GenericError($"No blocked range found for user {accountUuid}");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.NO_DATE_TIME_RANGE_LOCK_FOUND);
                }

                block.Timer.Dispose();
                schedulingBlocks.Remove(block);

                return OperationResult<bool, GenericError>.Success(true);
            }
        }

        public OperationResult<DateTimeRange, GenericError> GetDateTimeRangeByAccountUuid(Guid accountUuid)
        {
            lock (scheduleLock)
            {
                var block = schedulingBlocks.FirstOrDefault(b => b.AccountUuid == accountUuid);
                if (block == null)
                {
                    var error = new GenericError($"No blocked range found for user with UUID {accountUuid}.");
                    return OperationResult<DateTimeRange, GenericError>.Failure(error, MessageCodeType.NO_DATE_TIME_RANGE_LOCK_FOUND);
                }
                return OperationResult<DateTimeRange, GenericError>.Success(block.Range);
            }
        }

        public OperationResult<Guid, GenericError> GetAccountUuidByDateTimeRange(DateTimeRange range)
        {
            lock (scheduleLock)
            {
                var block = schedulingBlocks.FirstOrDefault(b => b.Range.Equals(range));
                if (block == null)
                {
                    var error = new GenericError($"No user found for the provided range: {range}");
                    return OperationResult<Guid, GenericError>.Failure(error, MessageCodeType.NO_DATE_TIME_RANGE_LOCK_FOUND);
                }
                return OperationResult<Guid, GenericError>.Success(block.AccountUuid);
            }
        }

    }
}