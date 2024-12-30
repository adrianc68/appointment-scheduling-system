using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
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

        public async Task<bool> IsAppointmentTimeSlotAvailable(DateTimeRange range)
        {
            bool isTimeSlotAvailable = await schedulerRepository.IsAppointmentTimeSlotAvailableAsync(range);
            return isTimeSlotAvailable;
        }

        public async Task<bool> IsAssistantAvailableInTimeRange(DateTimeRange range, int idAssistant)
        {
            bool isAssistantAvailableInTimeRange = await schedulerRepository.IsAssistantAvailableInTimeRange(range, idAssistant);
            return isAssistantAvailableInTimeRange;
        }

        public async Task<bool> IsAvailabilityTimeSlotAvailable(DateTimeRange range, int idAssistant)
        {
            bool isAvailabilityTimeSlotRegistered = await schedulerRepository.IsAvailabilityTimeSlotRegisteredAsync(range, idAssistant);
            return isAvailabilityTimeSlotRegistered;
        }

        public async Task<Guid?> RegisterAvailabilityTimeSlot(AvailabilityTimeSlot availabilityTimeSlot)
        {
            availabilityTimeSlot.Uuid = Guid.CreateVersion7();
            bool isRegistered = await schedulerRepository.AddAvailabilityTimeSlotAsync(availabilityTimeSlot);
            if (!isRegistered)
            {
                availabilityTimeSlot.Uuid = null;
                return null;
            }
            return availabilityTimeSlot.Uuid.Value;
        }

        public async Task<Guid?> ScheduleAppointment(Appointment appointment)
        {
            appointment.Uuid = Guid.CreateVersion7();

            bool isRegistered = await schedulerRepository.AddAppointmentAsync(appointment);
            if (isRegistered)
            {
                appointment.Uuid = null;
                return null;
            }
            return appointment.Uuid;
        }
    }
}