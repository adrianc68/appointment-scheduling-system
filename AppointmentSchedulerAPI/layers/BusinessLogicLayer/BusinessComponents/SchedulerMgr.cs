using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class SchedulerMgr : ISchedulerMgt, ISchedulerEvent, IClientObserver, IServiceObserver, IAssistantObserver, ISchedulerObserver
    {
        private readonly ISchedulerRepository schedulerRepository;
        private readonly INotificationMgt notificationMgr;
        private static readonly List<ISchedulerObserver> observers = new();

        public SchedulerMgr(ISchedulerRepository SchedulerRepository, INotificationMgt notificationMgr)
        {
            this.schedulerRepository = SchedulerRepository;
            this.notificationMgr = notificationMgr;
        }

        public async Task<List<Appointment>> GetScheduledOrConfirmedAppointmentsAsync(DateOnly startDate, DateOnly endDate)
        {
            return (List<Appointment>)await schedulerRepository.GetScheduledOrConfirmedAppoinmentsAsync(startDate, endDate);
        }

        public async Task<List<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate)
        {
            return (List<AvailabilityTimeSlot>)await schedulerRepository.GetAvailabilityTimeSlotsAsync(startDate, endDate);
        }

        public async Task<List<Appointment>> GetAllAppoinmentsAsync(DateOnly startDate, DateOnly endDate)
        {
            return (List<Appointment>)await schedulerRepository.GetAllAppoinmentsAsync(startDate, endDate);
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

        public async Task<int?> GetAppointmentIdByUuidAsync(Guid uuid)
        {
            int? id = await schedulerRepository.GetAppointmentIdByUuidAsync(uuid);
            return id;
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
            if (isStatusChanged)
            {
                SchedulerEventType eventType = status switch
                {
                    AvailabilityTimeSlotStatusType.DISABLED => SchedulerEventType.AVAILABILITY_TIME_SLOT_DISABLED,
                    AvailabilityTimeSlotStatusType.DELETED => SchedulerEventType.AVAILABILITY_TIME_SLOT_DELETED,
                    AvailabilityTimeSlotStatusType.ENABLED => SchedulerEventType.AVAILABILITY_TIME_SLOT_ENABLED,
                    _ => throw new ArgumentOutOfRangeException()
                };

                SchedulerEvent<AvailabilityTimeSlotEventData> availabilityTimeSlotEvent = new SchedulerEvent<AvailabilityTimeSlotEventData>
                {
                    Source = EventSource.AVAILABILITY_TIME_SLOT,
                    EventType = eventType,
                    EventData = new AvailabilityTimeSlotEventData
                    {
                        Id = idAvailabilityTimeSlot
                    }
                };

                if (status == AvailabilityTimeSlotStatusType.DISABLED || status == AvailabilityTimeSlotStatusType.DELETED)
                {
                    this.NotifySuscribers(availabilityTimeSlotEvent);
                }
            }

            return isStatusChanged;
        }

        public async Task<bool> UpdateAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot)
        {
            bool isUpdated = await schedulerRepository.UpdateAvailabilityTimeSlotAsync(availabilityTimeSlot);

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
            int totalAppoinemnts = await schedulerRepository.GetAppointmentsScheduledCountByClientUuidAsync(idClient);
            return totalAppoinemnts;
        }

        public async Task<List<DateTimeRange>> GetAppointmentDateTimeRangeConflictsByRangeAsync(DateTimeRange range)
        {
            List<DateTimeRange> ranges = (List<DateTimeRange>)await schedulerRepository.GetAppointmentDateTimeRangeConflictsByRangeAsync(range);
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

        public void UpdateOnAccountChanged(AssistantEvent eventType)
        {
            this.UpdateOnAssistantChanged(eventType);
        }

        public void UpdateOnAccountChanged(ClientEvent eventType)
        {
            this.UpdateOnClientChanged(eventType);
        }

        public async void UpdateOnServiceChanged(ServiceEvent serviceEvent)
        {
            if (serviceEvent.EventType == ServiceEventType.DISABLED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                int idService = serviceEvent.ServiceId!.Value;
                await this.ChangeAllServiceOfferStatusByServiceIdAsync(idService, ServiceOfferStatusType.DISABLED);
            }
            else if (serviceEvent.EventType == ServiceEventType.ENABLED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<< 
                int idService = serviceEvent.ServiceId!.Value;
                await this.ChangeAllServiceOfferStatusByServiceIdAsync(idService, ServiceOfferStatusType.ENABLED);
            }
            else if (serviceEvent.EventType == ServiceEventType.DELETED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                int idService = serviceEvent.ServiceId!.Value;
                await this.ChangeAllServiceOfferStatusByServiceIdAsync(idService, ServiceOfferStatusType.DELETED);
            }
        }

        public async void UpdateOnSchedulerEvent<T>(SchedulerEvent<T> schedulerEvent)
        {
            if (schedulerEvent.EventType == SchedulerEventType.AVAILABILITY_TIME_SLOT_UPDATED)
            {
                AvailabilityTimeSlotEventData? slotEventData = schedulerEvent.EventData as AvailabilityTimeSlotEventData;
                Guid uuidAvailabilityTimeSlot = slotEventData!.Uuid!.Value;
                await this.CancelScheduledOrConfirmedAppointmentsOfAssistantByAvailabilityTimeSlotUuid(uuidAvailabilityTimeSlot);
            }
            else if (schedulerEvent.EventType == SchedulerEventType.AVAILABILITY_TIME_SLOT_DISABLED || schedulerEvent.EventType == SchedulerEventType.AVAILABILITY_TIME_SLOT_DELETED)
            {
                AvailabilityTimeSlotEventData? slotEventData = schedulerEvent.EventData as AvailabilityTimeSlotEventData;
                int idAvailabilityTimeSlot = slotEventData!.Id!.Value;
                await this.CancelScheduledOrConfirmedAppointmentsByAvailabilityTimeSlot(idAvailabilityTimeSlot);
            }
        }

        public async void UpdateOnClientChanged(ClientEvent clientEvent)
        {
            if (clientEvent.EventType == ClientEventType.DISABLED || clientEvent.EventType == ClientEventType.DELETED)
            {
                // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
                int idClient = clientEvent.ClientId!.Value;
                // await this.CancelScheduledOrConfirmedAppointmentsOfClientById(idClient);
            }
        }

        public async void UpdateOnAssistantChanged(AssistantEvent assistantEvent)
        {

            // if (assistantEvent.EventType == AssistantEventType.DISABLED)
            // {
            //     int idAssistant = assistantEvent.AssistantId!.Value;
            //     await this.CancelOrRescheduledAppointmentsOfAssistantById(idAssistant);
            //     await this.ChangeAllServiceOfferStatusByAssistantIdAsync(idAssistant, ServiceOfferStatusType.DISABLED);
            // }
            // else if (assistantEvent.EventType == AssistantEventType.ENABLED)
            // {
            //     // $$$>> Create a Retry Mechanism to avoid inconsistencies <<<<<
            //     int idAssistant = assistantEvent.AssistantId!.Value;
            //     await this.ChangeAllServiceOfferStatusByAssistantIdAsync(idAssistant, ServiceOfferStatusType.ENABLED);
            // }
            // else if (assistantEvent.EventType == AssistantEventType.DELETED)
            // {
            //     int idAssistant = assistantEvent.AssistantId!.Value;
            //     await this.CancelOrRescheduledAppointmentsOfAssistantById(idAssistant);
            //     await this.ChangeAllServiceOfferStatusByAssistantIdAsync(idAssistant, ServiceOfferStatusType.DISABLED);
            // }
        }

        private async Task<(bool allCanceled, List<int> failedAppointments)> CancelScheduledOrConfirmedAppointmentsOfClientById(int idClient)
        {
            List<Appointment> appointments = await schedulerRepository.GetScheduledOrConfirmedAppointmentsOfClientByIdAsync(idClient);
            (bool allCanceled, List<int> failedAppointments) = await this.CancelAppointments(appointments);
            return (allCanceled, failedAppointments);
        }

        private async Task<((bool allCanceled, List<int> failedAppointments), (bool allRescheduled, List<int> rescheduledAppointments))> CancelScheduledOrConfirmedAppointmentsByAvailabilityTimeSlot(int idAvailabilityTimeSlot)
        {
            AvailabilityTimeSlot? slotData = await schedulerRepository.GetAvailabilityTimeSlotByIdAsync(idAvailabilityTimeSlot);
            if (slotData != null)
            {
                DateTimeRange range = new DateTimeRange
                {
                    StartDate = slotData.StartDate,
                    EndDate = slotData.EndDate,
                };
                List<Appointment> appointments = await schedulerRepository.GetScheduledOrConfirmedAppointmentsOfAsssistantByIdAndRangeAsync(slotData.Assistant!.Id!.Value, range);
                appointments = appointments.DistinctBy(a => a.Id).ToList();

                List<Appointment> appointmentsToCancel = [];
                List<Appointment> appointmentsToReschedule = [];

                foreach (var appointment in appointments)
                {
                    var scheduledServices = appointment.ScheduledServices!;
                    var servicesToRemove = scheduledServices.Where(ss => ss.ServiceOffer!.Assistant!.Id == slotData.Assistant.Id.Value).ToList();

                    if (servicesToRemove.Count == scheduledServices.Count)
                    {
                        appointmentsToCancel.Add(appointment);
                    }
                    else
                    {
                        foreach (var service in servicesToRemove)
                        {
                            scheduledServices.Remove(service);
                        }
                        appointment.StartDate = scheduledServices.Min(ss => ss.ServiceStartDate!.Value.Date);
                        appointment.StartDate = scheduledServices.Max(ss => ss.ServiceEndDate!.Value.Date);
                        appointment.TotalCost = scheduledServices.Sum(ss => ss.ServicePrice!.Value);

                        appointmentsToReschedule.Add(appointment);
                    }
                }

                (bool allCanceled, List<int> cancelAppoinments) = await this.CancelAppointments(appointmentsToCancel);
                (bool allRescheduled, List<int> rescheduledAppointments) = await this.RescheduleAppointments(appointmentsToReschedule);
                return ((allCanceled, cancelAppoinments), (allRescheduled, rescheduledAppointments));
            }
            return ((false, []), (false, []));
        }

        private async Task<((bool allCanceled, List<int> failedAppoinments), (bool allReschedueld, List<int> rescheduledAppointments))> CancelScheduledOrConfirmedAppointmentsOfAssistantByAvailabilityTimeSlotUuid(Guid availabilityTimeSlotUuid)
        {
            AvailabilityTimeSlot? slotData = await schedulerRepository.GetAvailabilityTimeSlotByUuidAsync(availabilityTimeSlotUuid);
            if (slotData != null)
            {
                if (slotData.UnavailableTimeSlots == null || slotData.UnavailableTimeSlots!.Count == 0)
                {
                    return ((false, []), (false, []));
                }

                List<Appointment> appointments = [];

                foreach (var unavailableSlot in slotData.UnavailableTimeSlots!)
                {
                    DateTimeRange range = new DateTimeRange
                    {
                        EndDate = unavailableSlot.EndDate,
                        StartDate = slotData.StartDate
                    };
                    appointments.AddRange(await schedulerRepository.GetScheduledOrConfirmedAppointmentsOfAsssistantByIdAndRangeAsync(slotData.Assistant!.Id!.Value, range));
                }

                appointments = appointments.DistinctBy(a => a.Id).ToList();

                List<Appointment> appointmentsToCancel = [];
                List<Appointment> appointmentsToReschedule = [];

                foreach (var appointment in appointments)
                {
                    var overlappingServices = appointment.ScheduledServices!
                        .Where(ss => slotData.UnavailableTimeSlots.Any(uts =>
                            ((ss.ServiceStartDate < uts.EndDate && ss.ServiceEndDate > uts.StartDate) || // Partially overlap
                            (ss.ServiceStartDate >= uts.StartDate && ss.ServiceEndDate <= uts.EndDate)) && // Fully overlap 
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

                        appointment.StartDate = appointment.ScheduledServices!.Min(ss => ss.ServiceStartDate!.Value.Date);
                        appointment.EndDate = appointment.ScheduledServices!.Max(ss => ss.ServiceEndDate!.Value.Date);
                        appointment.TotalCost = appointment.ScheduledServices!.Sum(ss => ss.ServicePrice!.Value);

                        appointmentsToReschedule.Add(appointment);
                    }
                }
                (bool allRescheduled, List<int> rescheduledAppointments) = await this.RescheduleAppointments(appointmentsToReschedule);
                (bool allCanceled, List<int> cancelAppoinments) = await this.CancelAppointments(appointmentsToCancel);
                return ((allCanceled, cancelAppoinments), (allRescheduled, rescheduledAppointments));
            }
            return ((false, []), (false, []));
        }

        private async Task<((bool allCancelled, List<int> canceledAppointments), (bool allRescheduled, List<int> rescheduledAppointments))> CancelOrRescheduledAppointmentsOfAssistantById(int idAssistant)
        {
            List<Appointment> appointments = await schedulerRepository.GetScheduledOrConfirmedAppointmentsOfAsssistantByIdAsync(idAssistant);

            List<Appointment> appointmentsToCancel = [];
            List<Appointment> appointmentsToReschedule = [];
            foreach (var appointment in appointments)
            {
                var scheduledServices = appointment.ScheduledServices!;
                var servicesToRemove = scheduledServices.Where(ss => ss.ServiceOffer!.Assistant!.Id == idAssistant).ToList();

                if (servicesToRemove.Count == scheduledServices.Count)
                {
                    appointmentsToCancel.Add(appointment);
                }
                else
                {
                    foreach (var service in servicesToRemove)
                    {
                        scheduledServices.Remove(service);
                    }
                    appointment.StartDate = scheduledServices.Min(ss => ss.ServiceStartDate!.Value.Date);
                    appointment.EndDate = scheduledServices.Max(ss => ss.ServiceEndDate!.Value.Date);
                    appointment.TotalCost = scheduledServices.Sum(ss => ss.ServicePrice!.Value);

                    appointmentsToReschedule.Add(appointment);
                }
            }
            (bool allRescheduled, List<int> rescheduledAppointments) = await this.RescheduleAppointments(appointmentsToReschedule);
            (bool allCanceled, List<int> cancelAppoinments) = await this.CancelAppointments(appointmentsToCancel);
            return ((allCanceled, cancelAppoinments), (allRescheduled, rescheduledAppointments));
        }


        private async Task<(bool allChanged, List<int> failedServiceOffers)> ChangeAllServiceOfferStatusByServiceIdAsync(int idService, ServiceOfferStatusType status)
        {
            List<ServiceOffer> serviceOffers = await schedulerRepository.GetServiceOffersByServiceIdAsync(idService);
            (bool allChanged, List<int> failedServiceOffers) = await ChangeServiceOffersStatus(serviceOffers, status);
            return (allChanged, failedServiceOffers);
        }

        private async Task<(bool allChanged, List<int> failedServiceOffers)> ChangeAllServiceOfferStatusByAssistantIdAsync(int idAssistant, ServiceOfferStatusType status)
        {
            List<ServiceOffer> serviceOffers = await schedulerRepository.GetServiceOffersByAssistantIdAsync(idAssistant);
            (bool allChanged, List<int> failedServiceOffers) = await ChangeServiceOffersStatus(serviceOffers, status);
            return (allChanged, failedServiceOffers);
        }



        private async Task<(bool allChanged, List<int> failedServiceOffers)> ChangeServiceOffersStatus(List<ServiceOffer> serviceOffers, ServiceOfferStatusType status)
        {
            List<int> failedServiceOffers = new();
            bool allChanged = true;
            foreach (var serviceOffer in serviceOffers)
            {
                bool isUpdated = await schedulerRepository.ChangeServiceOfferStatusTypeAsync(serviceOffer.Id!.Value, status);

                if (!isUpdated)
                {
                    failedServiceOffers.Add(serviceOffer.Id!.Value);
                    allChanged = false;
                }
            }
            return (allChanged, failedServiceOffers);
        }

        private async Task<(bool allCanceled, List<int> failedAppointments)> CancelAppointments(List<Appointment> appointmentsIds)
        {
            List<int> failedAppointments = new();
            bool allCanceled = true;
            foreach (var appointment in appointmentsIds)
            {
                bool isCanceled = await schedulerRepository.ChangeAppointmentStatusTypeAsync(appointment.Id!.Value, AppointmentStatusType.CANCELED);
                if (!isCanceled)
                {
                    failedAppointments.Add(appointment.Id!.Value);
                    allCanceled = false;
                }
                else
                {
                    await this.SendAppointmentNotification(appointment, "Your appointment has been canceled", AppointmentNotificationCodeType.APPOINTMENT_CANCELED);
                }
            }
            return (allCanceled, failedAppointments);
        }

        private async Task<(bool allRescheduled, List<int> failedAppointments)> RescheduleAppointments(List<Appointment> appointments)
        {
            List<int> failedAppointments = new();
            bool allRescheduled = true;
            foreach (var appointment in appointments)
            {
                bool isUpdated = await schedulerRepository.UpdateAppointmentAsync(appointment);

                if (!isUpdated)
                {
                    failedAppointments.Add(appointment.Id!.Value);
                    allRescheduled = false;
                }
                else
                {
                    await this.SendAppointmentNotification(appointment, "Your appointment has been rescheduled", AppointmentNotificationCodeType.APPOINTMENT_RESCHEDULED);
                }
            }
            return (allRescheduled, failedAppointments);
        }

        private async Task<Guid?> SendAppointmentNotification(Appointment appointment, string message, AppointmentNotificationCodeType code)
        {
            var notification = new AppointmentNotification
            {
                Recipients = [],
                Message = message,
                Code = code,
                Appointment = new AppointmentIdentifiers
                {
                    Id = appointment.Id!.Value,
                    Uuid = appointment.Uuid!.Value
                },
                Options = new NotificationOptions
                {
                    Channels = new List<NotificationChannelType>
                    {
                        NotificationChannelType.WEB_APPLICATION
                    }
                }
            };

            notification.Recipients.Add(new NotificationRecipient
            {
                RecipientData = new NotificationRecipientData
                {
                    Email = appointment.Client!.Email!,
                    PhoneNumber = appointment.Client.PhoneNumber!,
                    UserAccountId = appointment.Client.Id!.Value,
                    UserAccountUuid = appointment.Client.Uuid!.Value
                }
            });

            foreach (var scheduledService in appointment.ScheduledServices!)
            {
                var assistant = scheduledService!.ServiceOffer!.Assistant;

                notification.Recipients.Add(new NotificationRecipient
                {
                    RecipientData = new NotificationRecipientData
                    {
                        Email = assistant!.Email!,
                        PhoneNumber = assistant.PhoneNumber!,
                        UserAccountId = assistant.Id!.Value,
                        UserAccountUuid = assistant.Uuid!.Value
                    },
                });
            }

            Guid? isCreated = await this.notificationMgr.CreateNotificationAsync(notification);
            return isCreated;
        }


    }
}