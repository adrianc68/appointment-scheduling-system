using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class SchedulerMgr : ISchedulerMgt, ISchedulerEvent, IClientObserver, IServiceObserver, IAssistantObserver, ISchedulerObserver
    {
        private readonly ISchedulerRepository schedulerRepository;
        private static readonly List<ISchedulerObserver> observers = new();

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

        public async Task<List<ScheduledService>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range)
        {
            return (List<ScheduledService>)await schedulerRepository.GetConflictingServicesByDateTimeRangeAsync(range);
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
            foreach (var scheduledService in appointment.ScheduledServices!)
            {
                scheduledService.Uuid = Guid.CreateVersion7();
            }
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

        public async Task<AvailabilityTimeSlot?> GetAvailabilityTimeSlotByUuidAsync(Guid uuid)
        {
            AvailabilityTimeSlot? result = await schedulerRepository.GetAvailabilityTimeSlotByUuidAsync(uuid);
            return result;
        }

        public async Task<bool> ChangeAvailabilityStatusTypeAsync(int idAvailabilityTimeSlot, AvailabilityTimeSlotStatusType status)
        {
            bool isStatusChanged = await schedulerRepository.ChangeAvailabilityStatusTypeAsync(idAvailabilityTimeSlot, status);
            return isStatusChanged;
        }

        public async Task<bool> UpdateAvailabilityTimeSlot(AvailabilityTimeSlot availabilityTimeSlot)
        {
            bool isUpdated = await schedulerRepository.UpdateAvailabilityTimeSlot(availabilityTimeSlot);
            return isUpdated;
        }

        public async Task<bool> HasAvailabilityTimeSlotConflictingSlotsAsync(DateTimeRange range, int idAvailabilityTimeSlot, int idAssistant)
        {
            bool hasConflict = await schedulerRepository.HasAvailabilityTimeSlotConflictingSlotsAsync(range, idAvailabilityTimeSlot, idAssistant);
            return hasConflict;
        }

        public async Task<int> GetAppointmentsScheduledCountByClientId(int idClient)
        {
            int totalAppoinemnts = await schedulerRepository.GetAppointmentsScheduledCountByClientUuid(idClient);
            return totalAppoinemnts;
        }

        public async Task<List<DateTimeRange>> GetAppointmentDateTimeRangeConflictsByRange(DateTimeRange range)
        {
            List<DateTimeRange> ranges = (List<DateTimeRange>)await schedulerRepository.GetAppointmentDateTimeRangeConflictsByRange(range);
            return ranges;
        }

        public async Task<bool> CancelScheduledOrConfirmedAppointmentsOfClientById(int idAssistant)
        {
            return await schedulerRepository.CancelScheduledOrConfirmedAppointmentsOfClientById(idAssistant);
        }

        public async Task<bool> ChangeAllServiceOfferStatusByServiceId(int idService, ServiceOfferStatusType status)
        {
            return await schedulerRepository.ChangeAllServiceOfferStatusByServiceId(idService, status);
        }

        public async Task<bool> ChangeAllServiceOfferStatusByAssistantId(int idAssistant, ServiceOfferStatusType status)
        {
            return await schedulerRepository.ChangeAllServiceOfferStatusByAssistantId(idAssistant, status);
        }

        public void UpdateOnClientChanged(ClientEvent clientEvent)
        {
            if (clientEvent.EventType == ClientEventType.DISABLED || clientEvent.EventType == ClientEventType.DELETED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                _ = this.CancelScheduledOrConfirmedAppointmentsOfClientById(clientEvent.ClientId!.Value);
            }
        }

        public async Task<bool> CancelScheduledOrConfirmedAppointmentsOfAssistantById(int idAssistant)
        {
            return await schedulerRepository.CancelScheduledOrConfirmedAppointmentsOfAssistantById(idAssistant);
        }

        public void NotifySuscribers<T>(SchedulerEvent<T> eventType)
        {
            foreach (var observer in observers)
            {
                observer.UpdateOnSchedulerEvent(eventType);
            }
        }

        public void Suscribe(ISchedulerObserver schedulerObserver)
        {
            if (!observers.Contains(schedulerObserver))
            {
                observers.Add(schedulerObserver);
            }
        }

        public void Unsuscribe(ISchedulerObserver schedulerObserver)
        {
            if (observers.Contains(schedulerObserver))
            {
                observers.Remove(schedulerObserver);
            }
        }

        public void UpdateOnServiceChanged(ServiceEvent serviceEvent)
        {
            if (serviceEvent.EventType == ServiceEventType.DISABLED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                _ = this.ChangeAllServiceOfferStatusByServiceId(serviceEvent.ServiceId!.Value, ServiceOfferStatusType.NOT_AVAILABLE);
            }
            else if (serviceEvent.EventType == ServiceEventType.ENABLED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<   
                _ = this.ChangeAllServiceOfferStatusByServiceId(serviceEvent.ServiceId!.Value, ServiceOfferStatusType.AVAILABLE);
            }
            else if (serviceEvent.EventType == ServiceEventType.DELETED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<   
                _ = this.ChangeAllServiceOfferStatusByServiceId(serviceEvent.ServiceId!.Value, ServiceOfferStatusType.DELETED);
            }
        }

        public void UpdateOnAssistantChanged(AssistantEvent assistantEvent)
        {
            if (assistantEvent.EventType == AssistantEventType.DISABLED)
            {
                // Reschedule appoinments are necessary?
                // x = instance of Assistant
                // Case 1:
                // context Appoinments inv:
                // self.scheduledServices->exists(service | service.Assistant = x)
                // Case 2:
                // context Appoinments inv:
                // self.scheduledServices->size()>1 -> and self.scheduledServices->exists(service | service.Assistant = x)
                // Case 3:
                // context Appoinments inv:
                // self.scheduledServices->size()>1 and self.scheduledServices->select(service | service.Assistant = x)-> size() => 1
                // Case 4:
                // context Appoinments inv:
                // self.scheduledServices->forAll(service | service.assistant <> x)
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                _ = this.ChangeAllServiceOfferStatusByAssistantId(assistantEvent.AssistantId!.Value, ServiceOfferStatusType.NOT_AVAILABLE);
            }
            else if (assistantEvent.EventType == AssistantEventType.ENABLED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                _ = this.ChangeAllServiceOfferStatusByAssistantId(assistantEvent.AssistantId!.Value, ServiceOfferStatusType.AVAILABLE);
            }
            else if (assistantEvent.EventType == AssistantEventType.DELETED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                _ = this.ChangeAllServiceOfferStatusByAssistantId(assistantEvent.AssistantId!.Value, ServiceOfferStatusType.DELETED);
                _ = this.CancelScheduledOrConfirmedAppointmentsOfAssistantById(assistantEvent.AssistantId!.Value);
            }
        }

        public void UpdateOnSchedulerEvent<T>(SchedulerEvent<T> schedulerEvent)
        {
            throw new NotImplementedException();
        }

    }
}