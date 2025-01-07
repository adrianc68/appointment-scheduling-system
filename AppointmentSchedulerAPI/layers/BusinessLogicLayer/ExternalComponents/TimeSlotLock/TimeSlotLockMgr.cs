using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeSlotLock.Interfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeSlotLock.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeSlotLock
{
    public class TimeSlotLockMgr : ITimeSlotLockMgt
    {
        private static readonly List<BlockedTimeSlot> blockedTimeSlots = new();
        private readonly EnvironmentVariableService envService;
        private static readonly Lock scheduleLock = new();

        public TimeSlotLockMgr(EnvironmentVariableService envService)
        {
            this.envService = envService;
        }

        public OperationResult<DateTime, GenericError> BlockTimeSlot(List<ServiceTimeSlot> selectedServices, DateTimeRange range, Guid clientUuid)
        {
            lock (scheduleLock)
            {
                if (blockedTimeSlots.Any(b => b.ClientUuid == clientUuid))
                {
                    var error = new GenericError($"The user {clientUuid} has already blocked a time range.", []);
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.USER_ALREADY_HAS_BLOCKED_RANGE);
                }

                var conflictingServices = blockedTimeSlots
                    .Where(b => b.TotalServicesTimeRange!.Date == range.Date)
                    .SelectMany(b => b.SelectedServices)
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
                    UnblockTimeSlot(clientUuid);
                }, null, TimeSpan.FromSeconds(maxSecondsLock), Timeout.InfiniteTimeSpan);

                blockedTimeSlots.Add(new BlockedTimeSlot
                {
                    ClientUuid = clientUuid,
                    LockTimer = timer,
                    TotalServicesTimeRange = range,
                    LockExpirationTime = lockEndTime,
                    SelectedServices = selectedServices.Select(service => new ServiceTimeSlot
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


        public OperationResult<bool, GenericError> ExtendBlockedTimeSlot(DateTimeRange newRange, Guid clientUuid)
        {
            lock (scheduleLock)
            {
                if (newRange.StartTime >= newRange.EndTime)
                {
                    var error = new GenericError($"Invalid range: {newRange}. StartTime must be earlier than EndTime.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.INVALID_RANGE_TIME);
                }

                var existingBlock = blockedTimeSlots.FirstOrDefault(b => b.ClientUuid == clientUuid);
                if (existingBlock == null)
                {
                    var error = new GenericError($"No existing block found for user {clientUuid}.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.NO_DATE_TIME_RANGE_LOCK_FOUND);
                }

                if (blockedTimeSlots.Any(b => b.ClientUuid != clientUuid &&
                                              b.TotalServicesTimeRange!.Date == newRange.Date &&
                                              newRange.StartTime < b.TotalServicesTimeRange.EndTime &&
                                              newRange.EndTime > b.TotalServicesTimeRange.StartTime))
                {
                    var error = new GenericError($"Cannot extend range. Another range overlaps.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.SOMEONE_ELSE_IS_SCHEDULING_IN_RANGE_TIME);
                }
                return OperationResult<bool, GenericError>.Success(true);
            }
        }


        public OperationResult<bool, GenericError> UnblockTimeSlot(Guid clientUuid)
        {
            lock (scheduleLock)
            {
                var block = blockedTimeSlots.FirstOrDefault(b => b.ClientUuid == clientUuid);
                if (block == null)
                {
                    var error = new GenericError($"No blocked range found for user {clientUuid}");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.NO_DATE_TIME_RANGE_LOCK_FOUND);
                }

                block.LockTimer!.Dispose();
                blockedTimeSlots.Remove(block);

                return OperationResult<bool, GenericError>.Success(true);
            }
        }

        public OperationResult<BlockedTimeSlot, GenericError> GetBlockedTimeSlotByClientUuid(Guid clientUuid)
        {
            lock (scheduleLock)
            {
                var block = blockedTimeSlots.FirstOrDefault(b => b.ClientUuid == clientUuid);
                if (block == null)
                {
                    var error = new GenericError($"No blocked range found for user with UUID {clientUuid}.");
                    return OperationResult<BlockedTimeSlot, GenericError>.Failure(error, MessageCodeType.NO_DATE_TIME_RANGE_LOCK_FOUND);
                }
                return OperationResult<BlockedTimeSlot, GenericError>.Success(block);
            }
        }

        public OperationResult<List<BlockedTimeSlot>, GenericError> GetBlockedTimeSlotsByDate(DateOnly date)
        {
            lock (scheduleLock)
            {
                var blocks = blockedTimeSlots.Where(b => b.TotalServicesTimeRange!.Date == date).ToList();
                return OperationResult<List<BlockedTimeSlot>, GenericError>.Success(blocks);
            }
        }

    }
}