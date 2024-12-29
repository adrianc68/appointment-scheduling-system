using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class SchedulerMgr : ISchedulerMgt
    {
        private readonly ISchedulerRepository schedulerRepository;

        public SchedulerMgr(ISchedulerRepository SchedulerRepository)
        {
            this.schedulerRepository = SchedulerRepository;
        }

        public async Task<IEnumerable<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlots(DateOnly startDate, DateOnly endDate)
        {
            return (List<AvailabilityTimeSlot>)await schedulerRepository.GetAvailabilityTimeSlotsAsync(startDate, endDate);
        }

        public async Task<List<AssistantService>> GetAvailableServicesAsync(DateOnly date)
        {
            return (List<AssistantService>)await schedulerRepository.GetAvailableServicesAsync(date);

        }

        public async Task<Guid?> RegisterAvailabilityTimeSlot(AvailabilityTimeSlot availabilityTimeSlot, Guid assistantUuid)
        {
            availabilityTimeSlot.Uuid = Guid.CreateVersion7();
            bool isRegistered = await schedulerRepository.AddAvailabilityTimeSlotAsync(availabilityTimeSlot, assistantUuid);
            if (!isRegistered)
            {
                return null;
            }
            return availabilityTimeSlot.Uuid.Value;
        }

        public async Task<OperationResult<Guid>> ScheduleAppointment(Appointment appointment)
        {
            appointment.Uuid = Guid.CreateVersion7();
            appointment.Status = Model.Types.AppointmentStatusType.SCHEDULED;
            appointment.TotalCost = 500;
            appointment.EndTime = TimeOnly.Parse("12:00:00");
            appointment.Client.Id = 2;


            // 1. Check if time slot is available for appointment
            // using date, endTime, startTime



            bool isRegistered = await schedulerRepository.AddAppointmentAsync(appointment);
            if (isRegistered)
            {
                return new OperationResult<Guid>
                {
                    IsSuccessful = true,
                    Data = appointment.Uuid.Value,
                    Code = MessageCodeType.SUCCESS_OPERATION

                };
            }
            return new OperationResult<Guid>
            {
                IsSuccessful = true,
                Code = MessageCodeType.REGISTER_ERROR
            };
        }

        // public bool ScheduleAppointment(DateTimeRange range, List<Service> services, Client client)
        // {
        //     throw new NotImplementedException();
        // }
    }
}