using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
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
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.APPOINTMENT_TIME_RANGE_LOCK_HAS_BEEN_LOCKED_BY_USER);
                }

                var conflictingServices = blockedTimeSlots
                    .Where(b => b.TotalServicesTimeRange!.StartDate == range.StartDate)
                    .SelectMany(b => b.SelectedServices)
                    .Where(service => selectedServices.Any(newService =>
                        newService.StartDate < service.EndDate &&
                        newService.EndDate > service.StartDate &&
                        (service.ServiceUuid == newService.ServiceUuid ||
                        service.AssistantUuid == newService.AssistantUuid)
                    ))
                    .ToList();

                if (conflictingServices.Any())
                {
                    var error = new GenericError($"Cannot block the specified time range. Another time range is already blocked.", []);
                    error.AddData("OverlappingTimeRanges:", conflictingServices);
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.APPOINTMENT_TIME_RANGE_LOCK_OCCUPPIED_BY_ANOTHER_USER);
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
                        StartDate = service.StartDate,
                        EndDate = service.EndDate,
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
                if (newRange.EndDate >= newRange.EndDate)
                {
                    var error = new GenericError($"Invalid range: {newRange}. StartTime must be earlier than EndTime.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.INVALID_RANGE_TIME);
                }

                var existingBlock = blockedTimeSlots.FirstOrDefault(b => b.ClientUuid == clientUuid);
                if (existingBlock == null)
                {
                    var error = new GenericError($"No existing block found for user {clientUuid}.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.APPOINTMENT_TIME_RANGE_LOCK_NOT_FOUND);
                }

                if (blockedTimeSlots.Any(b => b.ClientUuid != clientUuid &&
                                              b.TotalServicesTimeRange!.StartDate == newRange.StartDate &&
                                              newRange.EndDate < b.TotalServicesTimeRange.EndDate &&
                                              newRange.EndDate > b.TotalServicesTimeRange.EndDate))
                {
                    var error = new GenericError($"Cannot extend range. Another range overlaps.");
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.APPOINTMENT_TIME_RANGE_LOCK_OCCUPPIED_BY_ANOTHER_USER);
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
                    return OperationResult<bool, GenericError>.Failure(error, MessageCodeType.APPOINTMENT_TIME_RANGE_LOCK_NOT_FOUND);
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
                    return OperationResult<BlockedTimeSlot, GenericError>.Failure(error, MessageCodeType.APPOINTMENT_TIME_RANGE_LOCK_NOT_FOUND);
                }
                return OperationResult<BlockedTimeSlot, GenericError>.Success(block);
            }
        }

        public OperationResult<List<BlockedTimeSlot>, GenericError> GetBlockedTimeSlotsByDate(DateOnly date)
        {
            lock (scheduleLock)
            {
                var blocks = blockedTimeSlots
                    .Where(b => DateOnly.FromDateTime(b.TotalServicesTimeRange!.StartDate) == date)
                    .ToList();

                return OperationResult<List<BlockedTimeSlot>, GenericError>.Success(blocks);
            }
        }

    }
}