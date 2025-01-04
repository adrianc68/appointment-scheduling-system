using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
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

        public async Task<List<Appointment>> GetScheduledOrConfirmedAppointmentsAsync(DateOnly startDate, DateOnly endDate)
        {
            return (List<Appointment>)await schedulerRepository.GetScheduledOrConfirmedAppoinmentsAsync(startDate, endDate);
        }

        public async Task<List<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate)
        {
            return (List<AvailabilityTimeSlot>)await schedulerRepository.GetAvailabilityTimeSlotsAsync(startDate, endDate);
        }

        public async Task<List<Appointment>> GetAllAppoinments(DateOnly startDate, DateOnly endDate)
        {
            return (List<Appointment>)await schedulerRepository.GetAllAppoinments(startDate, endDate);
        }

        public async Task<List<ServiceOffer>> GetAvailableServicesAsync(DateOnly date)
        {
            return (List<ServiceOffer>)await schedulerRepository.GetAvailableServicesAsync(date);
        }

        public async Task<List<ServiceOffer>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range)
        {
            return (List<ServiceOffer>)await schedulerRepository.GetConflictingServicesByDateTimeRangeAsync(range);
        }

        public async Task<bool> HasAssistantConflictingAppoinmentsAsync(DateTimeRange range, int idAssistant)
        {
            bool HasAssistantConflictingAppoinments = await schedulerRepository.HasAssistantConflictingAppoinmentsAsync(range, idAssistant);
            return HasAssistantConflictingAppoinments;
        }

        public async Task<bool> IsAssistantAvailableInAvailabilityTimeSlotsAsync(DateTimeRange range, int idAssistant)
        {
            bool isAssistantAvailableInAvailabilityTimeSlots = await schedulerRepository.IsAssistantAvailableInAvailabilityTimeSlotsAsync(range, idAssistant);
            return isAssistantAvailableInAvailabilityTimeSlots;
        }

        public async Task<bool> IsAppointmentTimeSlotAvailableAsync(DateTimeRange range)
        {
            bool isTimeSlotAvailable = await schedulerRepository.IsAppointmentTimeSlotAvailableAsync(range);
            return isTimeSlotAvailable;
        }

        public async Task<bool> IsAvailabilityTimeSlotAvailableAsync(DateTimeRange range, int idAssistant)
        {
            bool isAvailabilityTimeSlotRegistered = await schedulerRepository.IsAvailabilityTimeSlotRegisteredAsync(range, idAssistant);
            return isAvailabilityTimeSlotRegistered;
        }

        public async Task<Guid?> RegisterAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot)
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

        public async Task<Guid?> ScheduleAppointmentAsync(Appointment appointment)
        {
            appointment.Uuid = Guid.CreateVersion7();

            bool isRegistered = await schedulerRepository.AddAppointmentAsync(appointment);
            if (!isRegistered)
            {
                appointment.Uuid = null;
                return null;
            }
            return appointment.Uuid;
        }

        public async Task<Appointment?> GetAppointmentByUuidAsync(Guid uuid)
        {
            Appointment? appointment = await schedulerRepository.GetAppointmentByUuidAsync(uuid);
            return appointment;
        }

        public async Task<bool> ChangeAppointmentStatusTypeAsync(int idAppointment, AppointmentStatusType status)
        {
            bool isAppointmentStatusChanged = await schedulerRepository.ChangeAppointmentStatusTypeAsync(idAppointment, status);
            return isAppointmentStatusChanged;
        }

        public async Task<Appointment?> GetAppointmentDetailsByUuidAsync(Guid uuid)
        {
            Appointment? appointment = await schedulerRepository.GetAppointmentFullDetailsByUuidAsync(uuid);
            return appointment;
        }

        public async Task<bool> ChangeServiceOfferStatusTypeAsync(int idServiceOffer, ServiceOfferStatusType status)
        {
            bool isUpdated = await schedulerRepository.ChangeServiceOfferStatusTypeAsync(idServiceOffer, status);
            return isUpdated;
        }

        public async Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid)
        {
            ServiceOffer? serviceOffer = await schedulerRepository.GetServiceOfferByUuidAsync(uuid);
            return serviceOffer;
        }
    }
}