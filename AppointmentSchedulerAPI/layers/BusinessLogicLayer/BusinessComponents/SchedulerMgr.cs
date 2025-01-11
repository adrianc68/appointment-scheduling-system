using System.Linq.Expressions;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
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

            if (isUpdated)
            {
                SchedulerEvent<AvailabilityTimeSlotEventData> availabilityTimeSlotEvent = new SchedulerEvent<AvailabilityTimeSlotEventData>
                {
                    Source = EventSource.AVAILABILITY_TIME_SLOT,
                    EventType = SchedulerEventType.AVAILABILITY_TIME_SLOT_UPDATED,
                    EventData = new AvailabilityTimeSlotEventData
                    {
                        Uuid = availabilityTimeSlot.Uuid,
                    }
                };
                this.NotifySuscribers(availabilityTimeSlotEvent);
            }

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

        public async void UpdateOnServiceChanged(ServiceEvent serviceEvent)
        {
            List<int> serviceOffers = await schedulerRepository.GetServiceOfferIdsByServiceId(serviceEvent.ServiceId!.Value);
            if (serviceEvent.EventType == ServiceEventType.DISABLED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                await this.ChangeAllServiceOfferStatusAsync(serviceOffers, ServiceOfferStatusType.NOT_AVAILABLE);
            }
            else if (serviceEvent.EventType == ServiceEventType.ENABLED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<   
                await this.ChangeAllServiceOfferStatusAsync(serviceOffers, ServiceOfferStatusType.AVAILABLE);
            }
            else if (serviceEvent.EventType == ServiceEventType.DELETED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<   
                await this.ChangeAllServiceOfferStatusAsync(serviceOffers, ServiceOfferStatusType.DELETED);
            }
        }

        public async void UpdateOnSchedulerEvent<T>(SchedulerEvent<T> schedulerEvent)
        {
            if (schedulerEvent.EventType == SchedulerEventType.AVAILABILITY_TIME_SLOT_UPDATED)
            {
                AvailabilityTimeSlotEventData? slotEventData = schedulerEvent.EventData as AvailabilityTimeSlotEventData;
                AvailabilityTimeSlot? slotData = await schedulerRepository.GetAvailabilityTimeSlotByUuidAsync(slotEventData!.Uuid!.Value);
                if (slotData != null)
                {
                    if (slotData.UnavailableTimeSlots == null || slotData.UnavailableTimeSlots!.Count == 0)
                    {
                        return;
                    }

                    List<Appointment> appointments = [];

                    foreach (var unavailableSlot in slotData.UnavailableTimeSlots!)
                    {
                        DateTimeRange range = new DateTimeRange
                        {
                            StartTime = unavailableSlot.StartTime,
                            EndTime = unavailableSlot.EndTime,
                            Date = slotData.Date!.Value
                        };
                        appointments.AddRange(await schedulerRepository.GetScheduledOrConfirmedAppointmentsOfAsssistantByUidAndRange(slotData.Assistant!.Id!.Value, range));
                    }

                    appointments = appointments.DistinctBy(a => a.Id).ToList();

                    List<Appointment> appointmentsToCancel = [];
                    List<Appointment> appointmentsToReschedule = [];

                    foreach (var appointment in appointments)
                    {
                        var overlappingServices = appointment.ScheduledServices!
                            .Where(ss => slotData.UnavailableTimeSlots.Any(uts =>
                                ((ss.ServiceStartTime < uts.EndTime && ss.ServiceEndTime > uts.StartTime) || // Partially overlap
                                (ss.ServiceStartTime >= uts.StartTime && ss.ServiceEndTime <= uts.EndTime)) && // Fully overlap 
                                ss.ServiceOffer!.Assistant!.Id!.Value == slotData.Assistant!.Id!.Value))
                            .ToList();

                        if (overlappingServices.Count == appointment.ScheduledServices!.Count)
                        {
                            appointmentsToCancel.Add(appointment);
                        }
                        else if (overlappingServices.Count > 0)
                        {
                            foreach (var service in overlappingServices)
                            {
                                appointment.ScheduledServices!.Remove(service);
                            }

                            appointment.StartTime = appointment.ScheduledServices!.Min(ss => ss.ServiceStartTime);
                            appointment.EndTime = appointment.ScheduledServices!.Max(ss => ss.ServiceEndTime);
                            appointment.TotalCost = appointment.ScheduledServices!.Sum(ss => ss.ServicePrice!.Value);

                            appointmentsToReschedule.Add(appointment);
                        }
                    }

                    // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                    (bool allCanceled, List<int> cancelAppoinments) = await this.CancelScheduledOrConfirmedAppointments(appointmentsToCancel.Select(a => a.Id!.Value).ToList());
                    (bool allRescheduled, List<int> rescheduledAppointments) = await this.RescheduleScheduledOrConfirmedAppointments(appointmentsToReschedule);
                }

            }
        }

        public async void UpdateOnClientChanged(ClientEvent clientEvent)
        {
            if (clientEvent.EventType == ClientEventType.DISABLED || clientEvent.EventType == ClientEventType.DELETED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                List<int> appointmentsIds = await schedulerRepository.GetScheduledOrConfirmedAppoinmentsIdsOfClientById(clientEvent.ClientId!.Value);
                (bool allCanceled, List<int> cancelAppoinments) = await this.CancelScheduledOrConfirmedAppointments(appointmentsIds);
            }
        }

        public async void UpdateOnAssistantChanged(AssistantEvent assistantEvent)
        {
            List<int> serviceOffers = await schedulerRepository.GetServiceOfferIdsByAssistantId(assistantEvent.AssistantId!.Value);

            if (assistantEvent.EventType == AssistantEventType.DISABLED)
            {
                int idAssistant = assistantEvent.AssistantId!.Value;
                List<Appointment> appointments = await schedulerRepository.GetScheduledOrConfirmedAppointmentsOfAsssistantByUid(idAssistant);

                List<int> appointmentsToCancel = [];
                List<Appointment> appointmentsToReschedule = [];
                foreach (var appointment in appointments)
                {
                    var scheduledServices = appointment.ScheduledServices!;
                    var servicesToRemove = scheduledServices.Where(ss => ss.ServiceOffer!.Assistant!.Id == idAssistant).ToList();

                    if (servicesToRemove.Count == scheduledServices.Count)
                    {
                        appointmentsToCancel.Add(appointment.Id!.Value);
                    }
                    else
                    {
                        foreach (var service in servicesToRemove)
                        {
                            scheduledServices.Remove(service);
                        }
                        appointment.StartTime = scheduledServices.Min(ss => ss.ServiceStartTime);
                        appointment.EndTime = scheduledServices.Max(ss => ss.ServiceEndTime);
                        appointment.TotalCost = scheduledServices.Sum(ss => ss.ServicePrice!.Value);

                        appointmentsToReschedule.Add(appointment);
                    }
                }

                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                (bool allRescheduled, List<int> rescheduledAppointments) = await this.RescheduleScheduledOrConfirmedAppointments(appointments);
                (bool allCanceled, List<int> cancelAppoinments) = await this.CancelScheduledOrConfirmedAppointments(appointmentsToCancel);
                (bool allServiceOfferCanceled, List<int> changedStatusServiceOffers) = await this.ChangeAllServiceOfferStatusAsync(serviceOffers, ServiceOfferStatusType.NOT_AVAILABLE);
            }
            else if (assistantEvent.EventType == AssistantEventType.ENABLED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                (bool allServiceOfferCanceled, List<int> changedStatusServiceOffers) = await this.ChangeAllServiceOfferStatusAsync(serviceOffers, ServiceOfferStatusType.AVAILABLE);
            }
            else if (assistantEvent.EventType == AssistantEventType.DELETED)
            {

                int idAssistant = assistantEvent.AssistantId!.Value;
                List<Appointment> appointments = await schedulerRepository.GetScheduledOrConfirmedAppointmentsOfAsssistantByUid(idAssistant);


                List<int> appointmentsToCancel = [];
                List<Appointment> appointmentsToReschedule = [];
                foreach (var appointment in appointments)
                {
                    var scheduledServices = appointment.ScheduledServices!;
                    var servicesToRemove = scheduledServices.Where(ss => ss.ServiceOffer!.Assistant!.Id == idAssistant).ToList();

                    if (servicesToRemove.Count == scheduledServices.Count)
                    {
                        appointmentsToCancel.Add(appointment.Id!.Value);
                    }
                    else
                    {
                        foreach (var service in servicesToRemove)
                        {
                            scheduledServices.Remove(service);
                        }
                        appointment.StartTime = scheduledServices.Min(ss => ss.ServiceStartTime);
                        appointment.EndTime = scheduledServices.Max(ss => ss.ServiceEndTime);
                        appointment.TotalCost = scheduledServices.Sum(ss => ss.ServicePrice!.Value);

                        appointmentsToReschedule.Add(appointment);
                    }
                }

                (bool allRescheduled, List<int> rescheduledAppointments) = await this.RescheduleScheduledOrConfirmedAppointments(appointments);
                (bool allServiceOfferCanceled, List<int> changedStatusServiceOffers) = await this.ChangeAllServiceOfferStatusAsync(serviceOffers, ServiceOfferStatusType.DELETED);
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
            }
        }

        private async Task<(bool, List<int>)> ChangeAllServiceOfferStatusAsync(List<int> serviceOffersIds, ServiceOfferStatusType status)
        {
            List<int> failedServiceOffers = new();
            bool allCanceled = true;
            foreach (var id in serviceOffersIds)
            {
                bool isUpdated = await schedulerRepository.ChangeServiceOfferStatusTypeAsync(id, status);
                if (!isUpdated)
                {
                    failedServiceOffers.Add(id);
                    allCanceled = false;
                }
            }
            return (allCanceled, failedServiceOffers);
        }

        private async Task<(bool, List<int>)> CancelScheduledOrConfirmedAppointments(List<int> appointmentsIds)
        {
            List<int> failedAppointments = new();
            bool allCanceled = true;
            foreach (var id in appointmentsIds)
            {
                bool isCanceled = await schedulerRepository.ChangeAppointmentStatusTypeAsync(id, AppointmentStatusType.CANCELED);
                if (!isCanceled)
                {
                    failedAppointments.Add(id);
                    allCanceled = false;
                }
            }
            return (allCanceled, failedAppointments);
        }

        private async Task<(bool, List<int>)> RescheduleScheduledOrConfirmedAppointments(List<Appointment> appointments)
        {
            List<int> failedAppointments = new();
            bool allCanceled = true;
            foreach (var appointment in appointments)
            {
                bool isUpdated = await schedulerRepository.UpdateAppointment(appointment);

                if (!isUpdated)
                {
                    failedAppointments.Add(appointment.Id!.Value);
                    allCanceled = false;
                }
            }
            return (allCanceled, failedAppointments);
        }



    }
}