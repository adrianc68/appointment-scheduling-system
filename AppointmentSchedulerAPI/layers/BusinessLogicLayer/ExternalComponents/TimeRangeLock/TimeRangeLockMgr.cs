using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
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

        public OperationResult<DateTime, GenericError> BlockTimeRange(List<ServiceWithTime> selectedServices, DateTimeRange range, Guid clientUuid)
        {
            lock (scheduleLock)
            {
                if (schedulingBlocks.Any(b => b.ClientUuid == clientUuid))
                {
                    var error = new GenericError($"The user {clientUuid} has already blocked a time range.", []);
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.USER_ALREADY_HAS_BLOCKED_RANGE);
                }

                var conflictingServices = schedulingBlocks
                    .Where(b => b.Range!.Date == range.Date)
                    .SelectMany(b => b.Services)
                    .Where(service => selectedServices.Any(newService =>
                        newService.StartTime < service.EndTime &&
                        newService.EndTime > service.StartTime &&
                        (service.ServiceUuid == newService.ServiceUuid ||
                        service.AssistantUuid == newService.AssistantUuid)
                    ))
                    .ToList();

                if (conflictingServices.Any())
                {
                    var error = new GenericError($"Cannot block the specified time range. Another time range is already blocked.", []);
                    error.AddData("OverlappingTimeRanges:", conflictingServices);
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.SOMEONE_ELSE_IS_SCHEDULING_IN_RANGE_TIME);
                }

                int maxSecondsLock = int.Parse(envService.Get("MAX_SECONDS_SCHEDULING_LOCK"));
                DateTime lockEndTime = DateTime.Now.AddSeconds(maxSecondsLock);

                var timer = new Timer(_ =>
                {
                    UnblockTimeRange(clientUuid);
                }, null, TimeSpan.FromSeconds(maxSecondsLock), Timeout.InfiniteTimeSpan);

                schedulingBlocks.Add(new SchedulingBlock
                {
                    ClientUuid = clientUuid,
                    Timer = timer,
                    Range = range,
                    LockEndTime = lockEndTime,
                    Services = selectedServices.Select(service => new ServiceWithTime
                    {
                        StartTime = service.StartTime,
                        EndTime = service.EndTime,
                        ServiceUuid = service.ServiceUuid,
                        AssistantUuid = service.AssistantUuid
                    }).ToList()
                });

                return OperationResult<DateTime, GenericError>.Success(lockEndTime);
            }
        }


        public OperationResult<bool, GenericError> ExtendTimeRange(DateTimeRange newRange, Guid clientUuid)
        {
            lock (scheduleLock)
            {
                if (newRange.StartTime >= newRange.EndTime)
                {
                    var error = new GenericError($"Invalid range: {newRange}. StartTime must be earlier than EndTime.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.INVALID_RANGE_TIME);
                }

                var existingBlock = schedulingBlocks.FirstOrDefault(b => b.ClientUuid == clientUuid);
                if (existingBlock == null)
                {
                    var error = new GenericError($"No existing block found for user {clientUuid}.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.NO_DATE_TIME_RANGE_LOCK_FOUND);
                }

                if (schedulingBlocks.Any(b => b.ClientUuid != clientUuid &&
                                              b.Range!.Date == newRange.Date &&
                                              newRange.StartTime < b.Range.EndTime &&
                                              newRange.EndTime > b.Range.StartTime))
                {
                    var error = new GenericError($"Cannot extend range. Another range overlaps.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.SOMEONE_ELSE_IS_SCHEDULING_IN_RANGE_TIME);
                }
                return OperationResult<bool, GenericError>.Success(true);
            }
        }


        public OperationResult<bool, GenericError> UnblockTimeRange(Guid clientUuid)
        {
            lock (scheduleLock)
            {
                var block = schedulingBlocks.FirstOrDefault(b => b.ClientUuid == clientUuid);
                if (block == null)
                {
                    var error = new GenericError($"No blocked range found for user {clientUuid}");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.NO_DATE_TIME_RANGE_LOCK_FOUND);
                }

                block.Timer!.Dispose();
                schedulingBlocks.Remove(block);

                return OperationResult<bool, GenericError>.Success(true);
            }
        }

        public OperationResult<DateTimeRange, GenericError> GetDateTimeRangeByAccountUuid(Guid clientUuid)
        {
            lock (scheduleLock)
            {
                var block = schedulingBlocks.FirstOrDefault(b => b.ClientUuid == clientUuid);
                if (block == null)
                {
                    var error = new GenericError($"No blocked range found for user with UUID {clientUuid}.");
                    return OperationResult<DateTimeRange, GenericError>.Failure(error, MessageCodeType.NO_DATE_TIME_RANGE_LOCK_FOUND);
                }
                return OperationResult<DateTimeRange, GenericError>.Success(block.Range!);
            }
        }

        public OperationResult<List<SchedulingBlock>, GenericError> GetSchedulingBlockByDate(DateOnly date)
        {
            lock (scheduleLock)
            {
                var blocks = schedulingBlocks.Where(b => b.Range!.Date == date).ToList();
                return OperationResult<List<SchedulingBlock>, GenericError>.Success(blocks);
            }
        }

    }
}