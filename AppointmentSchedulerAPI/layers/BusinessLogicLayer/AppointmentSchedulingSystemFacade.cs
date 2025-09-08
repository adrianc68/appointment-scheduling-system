using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AccountInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authentication;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer
{
    public class AppointmentSchedulingSystemFacade : ISchedulingInterfaces, IServiceInterfaces, IClientInterfaces, IAssistantInterfaces, IAccountInterfaces, INotificationInterfaces
    {
        private readonly IServiceMgt serviceMgr;
        private readonly ISchedulerMgt schedulerMgr;
        private readonly IAssistantMgt assistantMgr;
        private readonly IClientMgt clientMgr;
        private readonly IAccountMgt accountMgr;
        private readonly INotificationMgt notificationMgr;
        private readonly EnvironmentVariableService envService;
        private readonly ITimeSlotLockMgt timeRangeLockMgr;
        private readonly IAuthenticationService<JwtUserCredentials, JwtTokenResult, JwtTokenData> authJwtService;

        public AppointmentSchedulingSystemFacade(IServiceMgt serviceMgr, IAssistantMgt assistantMgr, IClientMgt clientMgr, ISchedulerMgt schedulerMgr, IAccountMgt accountMgr, INotificationMgt notificationMgr, ITimeSlotLockMgt timeRangeLockMgr, EnvironmentVariableService envService, IAuthenticationService<JwtUserCredentials, JwtTokenResult, JwtTokenData> authJwtService)
        {
            this.serviceMgr = serviceMgr;
            this.schedulerMgr = schedulerMgr;
            this.assistantMgr = assistantMgr;
            this.accountMgr = accountMgr;
            this.clientMgr = clientMgr;
            this.timeRangeLockMgr = timeRangeLockMgr;
            this.envService = envService;
            this.authJwtService = authJwtService;
            this.notificationMgr = notificationMgr;
        }

        public Task<OperationResult<bool, GenericError>> EditAppointmentAsync(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Notification>> GetNotificationsByAccountUuidAsync(Guid uuid)
        {
            List<Notification> notifications = await notificationMgr.GetNotificationsByAccountUuidAsync(uuid);
            return notifications;
        }

        public async Task<List<Notification>> GetUnreadNotificationsByAccountUuidAsync(Guid uuid)
        {
            List<Notification> notifications = await notificationMgr.GetUnreadNotificationsByAccountUuidAsync(uuid);
            return notifications;
        }

        public async Task<bool> MarkNotificationAsReadAsync(Guid uuid, Guid accountUuid)
        {
            bool result = await notificationMgr.ChangeNotificationStatusByNotificationUuidAsync(uuid, accountUuid, Model.Types.Notification.NotificationStatusType.READ);
            return result;
        }

        public OperationResult<List<BlockedTimeSlot>, GenericError> GetSchedulingBlockRanges(DateOnly date)
        {
            return timeRangeLockMgr.GetBlockedTimeSlotsByDate(date);
        }

        public async Task<OperationResult<DateTime, GenericError>> BlockTimeRangeAsync(List<ScheduledService> services, DateTimeRange range, Guid clientUuid)
        {
            DateTime currentDateTime = DateTime.Now;
            int MAX_DAYS_FROM_NOW = int.Parse(envService.Get("MAX_DAYS_FOR_SCHEDULE"));
            int MAX_WEEKS_FOR_SCHEDULE = int.Parse(envService.Get("MAX_WEEKS_FOR_SCHEDULE"));
            int MAX_MONTHS_FOR_SCHEDULE = int.Parse(envService.Get("MAX_MONTHS_FOR_SCHEDULE"));
            int MAX_SERVICES_PER_CLIENT = int.Parse(envService.Get("MAX_SERVICES_PER_CLIENT"));
            int MAX_APPOINTMENTS_PER_CLIENT = int.Parse(envService.Get("MAX_APPOINTMENTS_PER_CLIENT"));
            bool IsPastSchedulingAllowed = bool.Parse(envService.Get("ALLOW_SCHEDULE_IN_THE_PAST"));

            DateTime maxDate = currentDateTime
                      .AddMonths(MAX_MONTHS_FOR_SCHEDULE)
                      .AddDays(MAX_WEEKS_FOR_SCHEDULE * 7)
                      .AddDays(MAX_DAYS_FROM_NOW);

            DateTime startDateTime = range.StartDate;

            // Check for past scheduling
            if (!IsPastSchedulingAllowed && startDateTime < currentDateTime)
            {
                GenericError genericError = new GenericError($"You cannot schedule an appoinment in the past. You can only schedule from the current time", []);
                genericError.AddData("SelectedDateTime", startDateTime.ToUniversalTime());
                genericError.AddData("SuggestedDateTime", currentDateTime.AddMinutes(1).ToUniversalTime());
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_SCHEDULING_IN_THE_PAST_NOT_ALLOWED);
            }

            if (startDateTime > maxDate)
            {
                GenericError genericError = new GenericError($"You cannot schedule an appoinment beyond {maxDate}.", []);
                genericError.AddData("SelectedDateTime", startDateTime.ToUniversalTime());
                genericError.AddData("SuggestedDateTime", currentDateTime.AddMinutes(5).ToUniversalTime());
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_SCHEDULING_BEYOND_X_NOT_ALLOWED);
            }

            // Check max services per client
            if (services.Count > MAX_SERVICES_PER_CLIENT)
            {
                GenericError genericError = new GenericError($"Too many services selected for the appointment. You can only select up to {MAX_SERVICES_PER_CLIENT} service(s) per appointment", []);
                genericError.AddData("MaxServicesPerAppointment", MAX_SERVICES_PER_CLIENT);
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_SERVICES_LIMIT_REACHED);
            }



            // Get Client data
            var clientData = await clientMgr.GetClientByUuidAsync(clientUuid);
            if (clientData == null)
            {
                GenericError genericError = new GenericError($"Client UUID: <{clientUuid}> is not registered", []);
                genericError.AddData("clientUuid", clientUuid);
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }

            if (clientData.Status != AccountStatusType.ENABLED)
            {
                GenericError genericError = new GenericError($"Client with UUID <{clientData.Uuid}> is not available. Client was disabled or deleted!", []);
                genericError.AddData("clientUuid", clientData.Uuid!.Value);
                genericError.AddData("Status", clientData.Status!.Value.ToString());
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_AVAILABLE);
            }

            // Check max allowed appoinments per client
            int scheduledAppoinmentsOfClientCount = await schedulerMgr.GetAppointmentsScheduledCountByClientId(clientData.Id!.Value);
            if (scheduledAppoinmentsOfClientCount >= MAX_APPOINTMENTS_PER_CLIENT)
            {
                timeRangeLockMgr.UnblockTimeSlot(clientData.Uuid!.Value);
                GenericError genericError = new GenericError($"The client with UUID {clientData.Uuid} has reached the maximum allowed number of appoinments {MAX_APPOINTMENTS_PER_CLIENT}", []);
                genericError.AddData("MaxAppointmentsPerClient", MAX_APPOINTMENTS_PER_CLIENT);
                genericError.AddData("AppointmentsScheduledCount", scheduledAppoinmentsOfClientCount);
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_SCHEDULED_LIMIT_REACHED);
            }

            // Get Services data
            for (int i = 0; i < services.Count; i++)
            {
                var serviceOffer = services[i];
                ServiceOffer? serviceOfferData = await assistantMgr.GetServiceOfferByUuidAsync(serviceOffer.Uuid!.Value);
                if (serviceOfferData == null)
                {
                    GenericError genericError = new GenericError($"Service <{serviceOffer.Uuid.Value}> is not registered", []);
                    genericError.AddData("SelectedServiceUuid", serviceOffer.Uuid.Value);
                    return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
                }

                if (serviceOfferData.Assistant!.Status != AccountStatusType.ENABLED || serviceOfferData.Service!.Status != ServiceStatusType.ENABLED)
                {
                    GenericError genericError = new GenericError($"Service or assistant is deleted or disabled. ServiceOffer with UUID <{serviceOffer.Uuid.Value}> is unavailable", []);
                    genericError.AddData("SelectedServiceUuid", serviceOfferData.Uuid!.Value);
                    genericError.AddData("AssistantStatus", serviceOfferData.Assistant!.Status!.Value.ToString());
                    genericError.AddData("ServiceStatus", serviceOfferData.Service!.Status!.Value.ToString());
                    genericError.AddData("SelectedServiceStatus", serviceOfferData.Status!.Value.ToString());
                    return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_UNAVAILABLE);
                }

                if (serviceOfferData.Status == ServiceOfferStatusType.DISABLED)
                {
                    GenericError genericError = new GenericError($"ServiceOffer with UUID <{serviceOffer.Uuid.Value}> is unavailable", []);
                    genericError.AddData("SelectedServiceUuid", serviceOffer.Uuid.Value);
                    genericError.AddData("ServiceOfferStatus", serviceOfferData.Status);
                    return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_UNAVAILABLE);
                }
                services[i].ServicesMinutes = serviceOfferData.Service.Minutes;
                services[i].ServiceEndDate = services[i].ServiceStartDate!.Value.AddMinutes(services[i].ServicesMinutes!.Value);
                services[i].ServicePrice = serviceOfferData.Service.Price;
                services[i].ServiceOffer = serviceOfferData;
            }

            services = services.OrderBy(so => so.ServiceStartDate).ToList();

            List<GenericError> errorMessages = [];
            for (int i = 1; i < services.Count; i++)
            {
                var prevService = services[i - 1];
                var currentService = services[i];

                if (currentService.ServiceStartDate != prevService.ServiceEndDate)
                {
                    GenericError genericError = new GenericError($"Service with UUID <{currentService!.Uuid!.Value}> is not contiguous with the previous service. <{prevService!.Uuid!.Value}>. Suggestions <ServiceOfferUuid>:<StartTime>:", []);
                    genericError.AddData($"{currentService.Uuid.Value}", prevService.ServiceEndDate!.Value.TimeOfDay);
                    errorMessages.Add(genericError);
                }
            }

            if (errorMessages.Any())
            {
                var result = OperationResult<DateTime, GenericError>.Failure(errorMessages, MessageCodeType.SERVICES_ARE_NOT_CONTIGUOUS);
                return result;
            }
            range.StartDate = services.Min(s => s.ServiceStartDate!.Value).ToUniversalTime();
            range.EndDate = range.StartDate.AddMinutes(services.Sum(s => s.ServicesMinutes!.Value)).ToUniversalTime();


            foreach (var scheduledService in services)
            {
                DateTime proposedStartDateTime = DateTime.SpecifyKind(
            scheduledService.ServiceStartDate!.Value,
            DateTimeKind.Utc
        );

                DateTime proposedEndDateTime = proposedStartDateTime.AddMinutes(scheduledService.ServicesMinutes!.Value);

                DateTimeRange serviceRange = new()
                {
                    StartDate = proposedStartDateTime,
                    EndDate = proposedEndDateTime
                };

                bool isAssistantAvailableInAvailabilityTimeSlots = await schedulerMgr.IsAssistantAvailableInAvailabilityTimeSlotsAsync(serviceRange, scheduledService.ServiceOffer!.Assistant!.Id!.Value);
                if (!isAssistantAvailableInAvailabilityTimeSlots)
                {
                    GenericError error = new GenericError($"Assistant: <{scheduledService.ServiceOffer.Assistant!.Uuid!.Value}> is not available during the requested time range", []);
                    error.AddData("SelectedServiceUuid", scheduledService.Uuid!.Value);
                    error.AddData("SelectedServiceStartDate", serviceRange.StartDate);
                    error.AddData("SelectedServiceEndDate", serviceRange.EndDate);
                    error.AddData("AssistantUuid", scheduledService.ServiceOffer.Assistant!.Uuid!.Value);
                    error.AddData("AssistantName", scheduledService.ServiceOffer.Assistant!.Name!);
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.ASSISTANT_NOT_AVAILABLE_IN_TIME_RANGE);
                }


                bool hasAssistantConflictingAppoinments = await schedulerMgr.HasAssistantConflictingAppoinmentsAsync(serviceRange, scheduledService.ServiceOffer.Assistant!.Id!.Value);
                if (hasAssistantConflictingAppoinments)
                {
                    GenericError error = new GenericError($"Assistant: <{scheduledService.ServiceOffer.Assistant!.Uuid!.Value}> is attending another appointment during the requested time range", []);
                    error.AddData("SelectedServiceUuid", scheduledService.Uuid!.Value);
                    error.AddData("SelectedServiceStartDate", serviceRange.StartDate);
                    error.AddData("SelectedServiceEndDate", serviceRange.EndDate);
                    error.AddData("AssistantUuid", scheduledService.ServiceOffer.Assistant!.Uuid!.Value);
                    error.AddData("AssistantName", scheduledService.ServiceOffer.Assistant!.Name!);
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.APPOINTMENT_SELECTED_SERVICE_CONFLICT_WITH_TIME_SLOT);
                }
            }


            List<ServiceTimeSlot> selectedServices = services.Select(service => new ServiceTimeSlot
            {
                StartDate = service.ServiceStartDate!.Value,
                EndDate = service.ServiceEndDate!.Value,
                ServiceUuid = service.Uuid!.Value,
                AssistantUuid = service.ServiceOffer!.Assistant!.Uuid!.Value
            }).ToList();

            return timeRangeLockMgr.BlockTimeSlot(selectedServices, range, clientUuid);
        }

        public OperationResult<bool, GenericError> UnblockTimeRange(Guid clientUuid)
        {
            return timeRangeLockMgr.UnblockTimeSlot(clientUuid);
        }

        public async Task<OperationResult<bool, GenericError>> EditAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot)
        {
            AvailabilityTimeSlot? slotData = await schedulerMgr.GetAvailabilityTimeSlotByUuidAsync(availabilityTimeSlot.Uuid!.Value);

            if (slotData == null)
            {
                GenericError genericError = new GenericError($"Availability time slot with UUID: <{availabilityTimeSlot.Uuid}> is not registered", []);
                genericError.AddData("AvailabilityTimeSlotUuid", availabilityTimeSlot.Uuid!.Value);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.AVAILABILITY_TIME_SLOT_NOT_FOUND);
            }
            availabilityTimeSlot.Id = slotData.Id;

            if (slotData.Status == AvailabilityTimeSlotStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot modify AvailabilityTimeSlot with UUID: <{availabilityTimeSlot.Uuid}> was deleted", []);
                genericError.AddData("AvailabilityTimeSlotUuid", availabilityTimeSlot.Uuid!.Value);
                genericError.AddData("Status", availabilityTimeSlot.Status);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.AVAILABILITY_TIME_SLOT_NOT_FOUND);
            }

            // 0. Check valid range time
            if (!(availabilityTimeSlot.StartDate < availabilityTimeSlot.EndDate || availabilityTimeSlot.StartDate == DateTime.MinValue))
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("Range provided is not valid"), MessageCodeType.INVALID_RANGE_TIME);
            }


            // 1. Get account data
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(slotData.Assistant!.Uuid!.Value);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Assistant UUID <{slotData.Assistant!.Uuid!.Value}> is not registered", []);
                genericError.AddData("AssistantUuid", slotData.Assistant!.Uuid!.Value);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (assistantData.Status != AccountStatusType.ENABLED)
            {
                GenericError genericError = new GenericError($"Cannot assign slot. Assistant with UUID <{assistantData!.Uuid!.Value}>. Assistant is unavailable!", []);
                genericError.AddData("AssistantUuid", assistantData!.Uuid.Value);
                genericError.AddData("Status", assistantData.Status!.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_UNAVAILABLE);
            }

            availabilityTimeSlot.Assistant = assistantData;

            // 2. Verify that no existing slot conflicts with the provided time range
            DateTimeRange range = new()
            {
                StartDate = availabilityTimeSlot.StartDate,
                EndDate = availabilityTimeSlot.EndDate,
            };

            bool hasConflictWithAnotherSlot = await schedulerMgr.HasAvailabilityTimeSlotConflictingSlotsAsync(range, slotData!.Id!.Value, assistantData.Id!.Value);
            if (hasConflictWithAnotherSlot)
            {
                GenericError genericError = new("Time range has conflicts with another slots", []);
                genericError.AdditionalData!.Add("Range", range);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.AVAILABILITY_TIME_SLOT_HAS_CONFLICTS);
            }

            bool isUpdated = await schedulerMgr.UpdateAvailabilityTimeSlotAsync(availabilityTimeSlot);
            if (!isUpdated)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public Task<List<ServiceOffer>> GetAvailableServicesClientAsync(DateOnly date)
        {
            return schedulerMgr.GetAvailableServicesAsync(date);
        }

        public Task<List<Assistant>> GetAllAssistantsAsync()
        {
            return assistantMgr.GetAllAssistantsAsync();
        }

        public Task<List<Service>> GetAllServicesAsync()
        {
            return serviceMgr.GetAllServicesAsync();
        }

        public Task<List<Client>> GetAllClientsAsync()
        {
            return clientMgr.GetAllClientsAsync();
        }

        public async Task<List<Appointment>> GetScheduledOrConfirmedAppoinmentsAsync(DateOnly startDate, DateOnly endDate)
        {
            List<Appointment>? appointment = await schedulerMgr.GetScheduledOrConfirmedAppointmentsAsync(startDate, endDate);
            return appointment;
        }

        public async Task<List<Appointment>> GetAllAppoinmentsAsync(DateOnly startDate, DateOnly endDate)
        {
            List<Appointment>? appointment = await schedulerMgr.GetAllAppoinmentsAsync(startDate, endDate);
            return appointment;
        }

        public async Task<OperationResult<Appointment, GenericError>> GetAppointmentDetailsAsync(Guid appointmentUuid)
        {
            Appointment? appointment = await schedulerMgr.GetAppointmentDetailsByUuidAsync(appointmentUuid);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {appointmentUuid} is not registered", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                return OperationResult<Appointment, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }
            return OperationResult<Appointment, GenericError>.Success(appointment);
        }

        public Task<List<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate)
        {
            return schedulerMgr.GetAllAvailabilityTimeSlotsAsync(startDate, endDate);
        }

        public async Task<List<ScheduledService>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range)
        {
            return await schedulerMgr.GetConflictingServicesByDateTimeRangeAsync(range);
        }

        public async Task<OperationResult<bool, GenericError>> EditAssistantAsync(Assistant assistant)
        {
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(assistant.Uuid!.Value);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Assistant with UUID <{assistant.Uuid.Value}> is not registered", []);
                genericError.AddData("AssistantUuid", assistant.Uuid.Value);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (assistantData.Status == AccountStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot modify Assistant with UUID <{assistantData.Uuid}>. Assistant was deleted!", []);
                genericError.AddData("AssistantUuid", assistantData.Uuid!.Value);
                genericError.AddData("Status", AssistantStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_WAS_DELETED);
            }

            if (assistant.Username != assistantData.Username)
            {
                bool newUsernameIsRegistered = await accountMgr.IsUsernameRegisteredAsync(assistant.Username!);
                if (newUsernameIsRegistered)
                {
                    GenericError genericError = new GenericError($"Username <{assistant.Username}> is already registered", []);
                    genericError.AddData("username", assistant.Username!);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_USERNAME_ALREADY_REGISTERED);
                }
            }


            if (assistant.PhoneNumber != assistantData.PhoneNumber)
            {
                bool newPhoneNumberIsRegistered = await accountMgr.IsPhoneNumberRegisteredAsync(assistant.PhoneNumber!);
                if (newPhoneNumberIsRegistered)
                {
                    GenericError genericError = new GenericError($"PhoneNumber <{assistant.PhoneNumber}> is already registered", []);
                    genericError.AddData("phoneNumber", assistant.PhoneNumber!);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_PHONE_NUMBER_ALREADY_REGISTERED);
                }
            }

            if (assistant.Email != assistantData.Email)
            {
                bool newEmailRegistered = await accountMgr.IsEmailRegisteredAsync(assistant.Email!);
                if (newEmailRegistered)
                {
                    GenericError genericError = new GenericError($"Email <{assistant.Email}> is already registered", []);
                    genericError.AddData("email", assistant.Email!);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_EMAIL_ALREADY_REGISTERED);
                }
            }

            bool isUpdated = await assistantMgr.UpdateAssistantAsync(assistant);
            if (!isUpdated)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(isUpdated);
        }

        public async Task<OperationResult<bool, GenericError>> UpdateClientAsync(Client client)
        {
            Client? clientData = await clientMgr.GetClientByUuidAsync(client.Uuid!.Value);
            if (clientData == null)
            {
                GenericError genericError = new GenericError($"Client with UUID <{client.Uuid.Value}> is not registered", []);
                genericError.AddData("clientUuid", client.Uuid.Value);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }

            if (clientData.Status == AccountStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot modify Client with UUID <{clientData.Uuid}>. Client was deleted!", []);
                genericError.AddData("clientUuid", clientData.Uuid!.Value);
                genericError.AddData("Status", ClientStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_WAS_DELETED);
            }

            if (client.Username != clientData.Username)
            {
                bool newUsernameIsRegistered = await accountMgr.IsUsernameRegisteredAsync(client.Username!);
                if (newUsernameIsRegistered)
                {
                    GenericError genericError = new GenericError($"Username <{client.Username}> is already registered", []);
                    genericError.AddData("username", client.Username!);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_USERNAME_ALREADY_REGISTERED);
                }
            }

            if (client.Email != clientData.Email)
            {
                bool newEmailRegistered = await accountMgr.IsEmailRegisteredAsync(client.Email!);
                if (newEmailRegistered)
                {
                    GenericError genericError = new GenericError($"Email <{client.Email}> is already registered", []);
                    genericError.AddData("email", client.Email!);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_EMAIL_ALREADY_REGISTERED);
                }
            }

            bool isUpdated = await clientMgr.UpdateClientAsync(client);
            if (!isUpdated)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(isUpdated);
        }

        public async Task<OperationResult<bool, GenericError>> EditServiceAsync(Service service)
        {
            Service? serviceData = await serviceMgr.GetServiceByUuidAsync(service.Uuid!.Value);
            if (service == null)
            {
                GenericError genericError = new GenericError($"Service with UUID <{service!.Uuid.Value}> is not registered", []);
                genericError.AddData("ServiceUuid", service.Uuid.Value);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
            }

            if (serviceData!.Status == ServiceStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot modify Service with UUID <{service.Uuid}>. Services was deleted!", []);
                genericError.AddData("ServiceUuid", service.Uuid);
                genericError.AddData("Status", ServiceStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_WAS_DELETED);
            }

            if (serviceData!.Name != service.Name)
            {
                bool isServiceNameRegistered = await serviceMgr.IsServiceNameRegisteredAsync(service.Name!);
                if (isServiceNameRegistered)
                {
                    return OperationResult<bool, GenericError>.Failure(new GenericError($"Service name <{service.Name}> is already registered"), MessageCodeType.SERVICE_NAME_ALREADY_REGISTERED);
                }
            }
            bool isUpdated = await serviceMgr.UpdateServiceAsync(service);
            if (!isUpdated)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has occurred"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<Guid, GenericError>> RegisterAssistantAsync(Assistant assistant)
        {
            bool IsUsernameRegistered = await accountMgr.IsUsernameRegisteredAsync(assistant.Username!);
            if (IsUsernameRegistered)
            {
                GenericError genericError = new GenericError($"Username <{assistant.Username}> is already registered", []);
                genericError.AddData("field", "Username");
                genericError.AddData("username", assistant.Username!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_USERNAME_ALREADY_REGISTERED);
            }

            bool IsEmailRegistered = await accountMgr.IsEmailRegisteredAsync(assistant.Email!);
            if (IsEmailRegistered)
            {
                GenericError genericError = new GenericError($"Email <{assistant.Email}> is already registered", []);
                genericError.AddData("field", "Email");
                genericError.AddData("email", assistant.Email!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_EMAIL_ALREADY_REGISTERED);
            }

            bool IsPhoneNumberRegistered = await accountMgr.IsPhoneNumberRegisteredAsync(assistant.PhoneNumber!);
            if (IsPhoneNumberRegistered)
            {
                GenericError genericError = new GenericError($"PhoneNumber <{assistant.PhoneNumber}> is already registered", []);
                genericError.AddData("field", "PhoneNumber");
                genericError.AddData("phoneNumber", assistant.PhoneNumber!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_PHONE_NUMBER_ALREADY_REGISTERED);
            }
            Guid? uuidNewAssistant = await assistantMgr.RegisterAssistantAsync(assistant);
            if (uuidNewAssistant == null)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.REGISTER_ERROR);
            }
            return OperationResult<Guid, GenericError>.Success(uuidNewAssistant.Value);
        }

        public async Task<OperationResult<Guid, GenericError>> RegisterClientAsync(Client client)
        {
            bool IsUsernameRegistered = await accountMgr.IsUsernameRegisteredAsync(client.Username!);
            if (IsUsernameRegistered)
            {
                GenericError genericError = new GenericError($"Username <{client.Username}> is already registered", []);
                genericError.AddData("field", "Username");
                genericError.AddData("username", client.Username!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_USERNAME_ALREADY_REGISTERED);
            }

            bool IsEmailRegistered = await accountMgr.IsEmailRegisteredAsync(client.Email!);
            if (IsEmailRegistered)
            {
                GenericError genericError = new GenericError($"Email <{client.Email}> is already registered", []);
                genericError.AddData("field", "Email");
                genericError.AddData("email", client.Email!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_EMAIL_ALREADY_REGISTERED);
            }

            bool IsPhoneNumberRegistered = await accountMgr.IsPhoneNumberRegisteredAsync(client.PhoneNumber!);
            if (IsPhoneNumberRegistered)
            {
                GenericError genericError = new GenericError($"PhoneNumber <{client.PhoneNumber}> is already registered", []);
                genericError.AddData("field", "PhoneNumber");
                genericError.AddData("phoneNumber", client.PhoneNumber!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_PHONE_NUMBER_ALREADY_REGISTERED);
            }
            Guid? uuidNewClient = await clientMgr.RegisterClientAsync(client);
            if (uuidNewClient == null)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.REGISTER_ERROR);
            }
            return OperationResult<Guid, GenericError>.Success(uuidNewClient.Value);
        }

        public async Task<OperationResult<Guid, GenericError>> RegisterAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot)
        {
            // 0. Check valid range time
            if (availabilityTimeSlot.StartDate == DateTime.MinValue || availabilityTimeSlot.EndDate == DateTime.MinValue || availabilityTimeSlot.StartDate >= availabilityTimeSlot.EndDate)
            {
                return OperationResult<Guid, GenericError>.Failure(
                    new GenericError("Range provided is not valid"),
                    MessageCodeType.INVALID_RANGE_TIME);
            }

            // 1. Get account data
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(availabilityTimeSlot.Assistant!.Uuid!.Value);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Assistant UUID <{availabilityTimeSlot.Assistant!.Uuid!.Value}> is not registered", []);
                genericError.AddData("AssistantUuid", availabilityTimeSlot.Assistant!.Uuid!.Value);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (assistantData.Status != AccountStatusType.ENABLED)
            {
                GenericError genericError = new GenericError($"Cannot assign slot. Assistant with UUID <{assistantData!.Uuid!.Value}>. Assistant is unavailable!", []);
                genericError.AddData("AssistantUuid", assistantData!.Uuid.Value);
                genericError.AddData("Status", assistantData.Status!.Value.ToString());
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_UNAVAILABLE);
            }

            availabilityTimeSlot.Assistant = assistantData;

            // 2. Verify that no existing slot conflicts with the provided time range
            DateTimeRange range = new()
            {
                StartDate = availabilityTimeSlot.StartDate,
                EndDate = availabilityTimeSlot.EndDate,
            };

            bool isAvailabilityTimeSlotAvailable = await schedulerMgr.IsAvailabilityTimeSlotAvailableAsync(range, assistantData.Id!.Value);
            if (!isAvailabilityTimeSlotAvailable)
            {
                GenericError genericError = new("Time range is not available. Another Time slot is disabled or enabled", new Dictionary<string, object>());
                genericError.AdditionalData!.Add("Range", range);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.AVAILABILITY_TIME_SLOT_NOT_AVAILABLE);
            }
            // 3. Register availability time slot
            Guid? uuid = await schedulerMgr.RegisterAvailabilityTimeSlotAsync(availabilityTimeSlot);
            if (uuid == null)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError("An error has ocurred"), MessageCodeType.REGISTER_ERROR);
            }
            return OperationResult<Guid, GenericError>.Success(uuid.Value);
        }

        public async Task<OperationResult<Guid, GenericError>> RegisterServiceAsync(Service service)
        {
            bool isServiceNameRegistered = await serviceMgr.IsServiceNameRegisteredAsync(service.Name!);
            if (isServiceNameRegistered)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError($"Service name <{service.Name}> is already registered"), MessageCodeType.SERVICE_NAME_ALREADY_REGISTERED);
            }

            Guid? UuidNewservice = await serviceMgr.RegisterServiceAsync(service);
            if (UuidNewservice == null)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError("An error has occurred"), MessageCodeType.REGISTER_ERROR);
            }
            return OperationResult<Guid, GenericError>.Success(UuidNewservice.Value);
        }

        public async Task<OperationResult<bool, GenericError>> DeleteServiceOfferAsync(Guid uuid)
        {
            ServiceOffer? serviceOfferData = await schedulerMgr.GetServiceOfferByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckServiceOfferStatusType(serviceOfferData, ServiceOfferStatusType.DELETED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await schedulerMgr.ChangeServiceOfferStatusTypeAsync(serviceOfferData!.Id!.Value, ServiceOfferStatusType.DELETED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DisableServiceOfferAsync(Guid uuid)
        {
            ServiceOffer? serviceOfferData = await schedulerMgr.GetServiceOfferByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckServiceOfferStatusType(serviceOfferData, ServiceOfferStatusType.DISABLED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await schedulerMgr.ChangeServiceOfferStatusTypeAsync(serviceOfferData!.Id!.Value, ServiceOfferStatusType.DISABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> EnableServiceOfferAsync(Guid uuid)
        {
            ServiceOffer? serviceOfferData = await schedulerMgr.GetServiceOfferByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckServiceOfferStatusType(serviceOfferData, ServiceOfferStatusType.ENABLED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await schedulerMgr.ChangeServiceOfferStatusTypeAsync(serviceOfferData!.Id!.Value, ServiceOfferStatusType.ENABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DisableAssistantAsync(Guid uuid)
        {
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckAssistantStatusType(assistantData, AccountStatusType.DISABLED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await accountMgr.ChangeAccountStatusAsync(assistantData!.Id!.Value, AccountStatusType.DISABLED, AccountType.ASSISTANT);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> EnableAssistantAsync(Guid uuid)
        {
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckAssistantStatusType(assistantData, AccountStatusType.ENABLED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await accountMgr.ChangeAccountStatusAsync(assistantData!.Id!.Value, AccountStatusType.ENABLED, AccountType.ASSISTANT);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DeleteAssistantAsync(Guid uuid)
        {
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckAssistantStatusType(assistantData, AccountStatusType.DELETED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await accountMgr.ChangeAccountStatusAsync(assistantData!.Id!.Value, AccountStatusType.DELETED, AccountType.ASSISTANT);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DisableClientAsync(Guid uuid)
        {
            Client? clientData = await clientMgr.GetClientByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckClientStatusType(clientData, AccountStatusType.DISABLED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await accountMgr.ChangeAccountStatusAsync(clientData!.Id!.Value, AccountStatusType.DISABLED, AccountType.CLIENT);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> EnableClientAsync(Guid uuid)
        {
            Client? clientData = await clientMgr.GetClientByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckClientStatusType(clientData, AccountStatusType.ENABLED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await accountMgr.ChangeAccountStatusAsync(clientData!.Id!.Value, AccountStatusType.ENABLED, AccountType.CLIENT);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DeleteClientAsync(Guid uuid)
        {
            Client? clientData = await clientMgr.GetClientByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckClientStatusType(clientData, AccountStatusType.DELETED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await accountMgr.ChangeAccountStatusAsync(clientData!.Id!.Value, AccountStatusType.DELETED, AccountType.CLIENT);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> EnableServiceAsync(Guid uuid)
        {
            Service? serviceData = await serviceMgr.GetServiceByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckServiceStatusType(serviceData, ServiceStatusType.ENABLED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await serviceMgr.ChangeServiceStatusTypeAsync(serviceData!.Id!.Value, ServiceStatusType.ENABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DisableServiceAsync(Guid uuid)
        {
            Service? serviceData = await serviceMgr.GetServiceByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckServiceStatusType(serviceData, ServiceStatusType.DISABLED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await serviceMgr.ChangeServiceStatusTypeAsync(serviceData!.Id!.Value, ServiceStatusType.DISABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DeleteServiceAsync(Guid uuid)
        {
            Service? serviceData = await serviceMgr.GetServiceByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckServiceStatusType(serviceData, ServiceStatusType.DELETED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await serviceMgr.ChangeServiceStatusTypeAsync(serviceData!.Id!.Value, ServiceStatusType.DELETED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DeleteAvailabilityTimeSlotAsync(Guid uuid)
        {
            AvailabilityTimeSlot? slotData = await schedulerMgr.GetAvailabilityTimeSlotByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckAvailabilityTimeSlotStatusType(slotData, AvailabilityTimeSlotStatusType.DELETED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await schedulerMgr.ChangeAvailabilityStatusTypeAsync(slotData!.Id!.Value, AvailabilityTimeSlotStatusType.DELETED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DisableAvailabilityTimeSlotAsync(Guid uuid)
        {
            AvailabilityTimeSlot? slotData = await schedulerMgr.GetAvailabilityTimeSlotByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckAvailabilityTimeSlotStatusType(slotData, AvailabilityTimeSlotStatusType.DISABLED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            bool isStatusChanged = await schedulerMgr.ChangeAvailabilityStatusTypeAsync(slotData!.Id!.Value, AvailabilityTimeSlotStatusType.DISABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> EnableAvailabilityTimeSlotAsync(Guid uuid)
        {
            AvailabilityTimeSlot? slotData = await schedulerMgr.GetAvailabilityTimeSlotByUuidAsync(uuid);
            OperationResult<bool, GenericError> checkStatus = this.CheckAvailabilityTimeSlotStatusType(slotData, AvailabilityTimeSlotStatusType.ENABLED);
            if (!checkStatus.IsSuccessful)
            {
                return checkStatus;
            }

            // 2. Verify that no existing slot conflicts with the provided time range
            DateTimeRange range = new()
            {
                StartDate = slotData!.StartDate,
                EndDate = slotData!.EndDate,
            };

            // bool isAvailabilityTimeSlotAvailable = await schedulerMgr.IsAvailabilityTimeSlotAvailableAsync(range, slotData.Assistant!.Id!.Value);
            // if (!isAvailabilityTimeSlotAvailable)
            // {
            //     GenericError genericError = new("Time range is not available. Another Time slot is disabled or enabled", new Dictionary<string, object>());
            //     genericError.AdditionalData!.Add("Range", range);
            //     return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.AVAILABILITY_TIME_SLOT_NOT_AVAILABLE);
            // }

            bool isStatusChanged = await schedulerMgr.ChangeAvailabilityStatusTypeAsync(slotData!.Id!.Value, AvailabilityTimeSlotStatusType.ENABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<List<Guid>, GenericError>> AssignListServicesToAssistantAsync(Guid assistantUuid, List<Guid> servicesUuids)
        {

            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(assistantUuid);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Asssistant with UUID <{assistantUuid}> is not found", []);
                genericError.AddData("AssistantUuid", assistantUuid);
                return OperationResult<List<Guid>, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (assistantData.Status == AccountStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot modify Assistant with UUID <{assistantData.Uuid}>. Assistant was deleted!", []);
                genericError.AddData("AssistantUuid", assistantData.Uuid!.Value);
                genericError.AddData("Status", AssistantStatusType.DELETED.ToString());
                return OperationResult<List<Guid>, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_WAS_DELETED);
            }


            List<int> idServices = [];
            foreach (var serviceUuid in servicesUuids)
            {
                Service? serviceData = await serviceMgr.GetServiceByUuidAsync(serviceUuid);
                if (serviceData == null)
                {
                    GenericError genericError = new GenericError($"Service with UUID <{serviceUuid}> is not found", []);
                    genericError.AddData("serviceUuid", servicesUuids);
                    return OperationResult<List<Guid>, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
                }

                if (serviceData.Status != ServiceStatusType.ENABLED)
                {
                    GenericError genericError = new GenericError($"Cannot assign Service with UUID <{serviceData.Uuid}>. Service is unavailable", []);
                    genericError.AddData("ServiceUuid", serviceData.Uuid!.Value);
                    genericError.AddData("Status", serviceData.Status!.Value.ToString());
                    return OperationResult<List<Guid>, GenericError>.Failure(genericError, MessageCodeType.SERVICE_UNAVAILABLE);
                }


                bool isAlreadyRegistered = await assistantMgr.IsAssistantOfferingServiceByUuidAsync(serviceData.Id!.Value, assistantData.Id!.Value);
                if (isAlreadyRegistered)
                {
                    GenericError genericError = new GenericError($"Service with UUID <{serviceUuid}> is already assigned to assistant {assistantUuid}", []);
                    genericError.AddData("AssistantUuid", assistantUuid);
                    genericError.AddData("conflictingServiceUuid", serviceUuid);
                    return OperationResult<List<Guid>, GenericError>.Failure(genericError, MessageCodeType.SERVICE_ALREADY_ASSIGNED_TO_ASSISTANT);
                }
                idServices.Add(serviceData.Id.Value);
            }
            List<Guid> assignedServiceUuids = await assistantMgr.AssignListServicesToAssistantAsync(assistantData.Id!.Value, idServices);
            if (assignedServiceUuids == null || !assignedServiceUuids.Any())
            {
                return OperationResult<List<Guid>, GenericError>.Failure(
                    new GenericError("An error has occurred"),
                    MessageCodeType.REGISTER_ERROR
                );
            }

            // Devuelve la lista de GUIDs correctamente
            return OperationResult<List<Guid>, GenericError>.Success(assignedServiceUuids);
        }

        public async Task<OperationResult<bool, GenericError>> FinalizeAppointmentAsync(Guid uuid)
        {
            Appointment? appointment = await schedulerMgr.GetAppointmentByUuidAsync(uuid);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuid} is not registered", []);
                genericError.AddData("AppointmentUuid", uuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }

            if (appointment.Status == AppointmentStatusType.SCHEDULED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuid} must be confirmed before proceeding", []);
                genericError.AddData("AppointmentUuid", uuid);
                genericError.AddData("AppointmentStatus", appointment.Status.ToString()!);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NEEDS_TO_BE_CONFIRMED);
            }

            if (appointment.Status == AppointmentStatusType.CANCELED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuid} was cancelled before.", []);
                genericError.AddData("AppointmentUuid", uuid);
                genericError.AddData("AppointmentStatus", appointment.Status.ToString()!);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREDY_CANCELED);
            }

            if (appointment.Status == AppointmentStatusType.FINISHED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuid} is already finished", []);
                genericError.AddData("AppointmentUuid", uuid);
                genericError.AddData("AppointmentStatus", appointment.Status.ToString()!);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_FINISHED);
            }

            bool isStatusOfAppointmentChanged = await schedulerMgr.ChangeAppointmentStatusTypeAsync(appointment.Id!.Value, AppointmentStatusType.FINISHED);
            if (!isStatusOfAppointmentChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> ConfirmAppointmentAsync(Guid uuid)
        {
            Appointment? appointment = await schedulerMgr.GetAppointmentByUuidAsync(uuid);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuid} is not registered", []);
                genericError.AddData("AppointmentUuid", uuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }

            if (appointment.Status == AppointmentStatusType.CANCELED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuid} was cancelled before.", []);
                genericError.AddData("AppointmentUuid", uuid);
                genericError.AddData("AppointmentStatus", appointment.Status.ToString()!);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREDY_CANCELED);
            }

            if (appointment.Status == AppointmentStatusType.FINISHED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuid} is finished.", []);
                genericError.AddData("AppointmentUuid", uuid);
                genericError.AddData("AppointmentStatus", appointment.Status.ToString()!);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_FINISHED);
            }

            if (appointment.Status == AppointmentStatusType.CONFIRMED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuid} is already confirmed", []);
                genericError.AddData("AppointmentUuid", uuid);
                genericError.AddData("Status", appointment.Status.ToString()!);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_CONFIRMED);
            }

            bool isStatusOfAppointmentChanged = await schedulerMgr.ChangeAppointmentStatusTypeAsync(appointment.Id!.Value, AppointmentStatusType.CONFIRMED);
            if (!isStatusOfAppointmentChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> CancelAppointmentClientSelfAsync(Guid appointmentUuid, Guid ClientUuid)
        {
            Client? clientData = await clientMgr.GetClientByUuidAsync(ClientUuid);
            if (clientData == null)
            {
                GenericError genericError = new GenericError($"Client UUID <{ClientUuid}> is not found");
                genericError.AddData("clientUuid", appointmentUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }

            Appointment? appointment = await schedulerMgr.GetAppointmentByUuidAsync(appointmentUuid);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{appointmentUuid}> is not registered", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }

            if (appointment.Client!.Id != clientData.Id)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{appointmentUuid}> belongs to another user");
                genericError.AddData("AppointmentUuid", appointmentUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_BELONGS_TO_ANOTHER_USER);
            }

            if (appointment.Status == AppointmentStatusType.FINISHED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{appointmentUuid}> is already finished", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                genericError.AddData("Status", appointment.Status.ToString()!);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_FINISHED);
            }

            if (appointment.Status == AppointmentStatusType.CANCELED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{appointmentUuid}> is already canceled", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                genericError.AddData("Status", appointment.Status.ToString()!);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREDY_CANCELED);
            }

            bool isStatusOfAppointmentChanged = await schedulerMgr.ChangeAppointmentStatusTypeAsync(appointment.Id!.Value, AppointmentStatusType.CANCELED);
            if (!isStatusOfAppointmentChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> CancelAppointmentStaffAssistedAsync(Guid uuid)
        {
            Appointment? appointment = await schedulerMgr.GetAppointmentByUuidAsync(uuid);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{uuid}> is not registered", []);
                genericError.AddData("AppointmentUuid", uuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }

            if (appointment.Status == AppointmentStatusType.FINISHED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{uuid}> is already finished", []);
                genericError.AddData("AppointmentUuid", uuid);
                genericError.AddData("Status", appointment.Status.ToString()!);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_FINISHED);
            }

            if (appointment.Status == AppointmentStatusType.CANCELED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{uuid}> is already canceled", []);
                genericError.AddData("AppointmentUuid", uuid);
                genericError.AddData("Status", appointment.Status.ToString()!);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREDY_CANCELED);
            }

            bool isStatusOfAppointmentChanged = await schedulerMgr.ChangeAppointmentStatusTypeAsync(appointment.Id!.Value, AppointmentStatusType.CANCELED);
            if (!isStatusOfAppointmentChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);

        }

        public async Task<OperationResult<Guid, GenericError>> ScheduleAppointmentAsClientAsync(Appointment appointment)
        {
            appointment.Status = AppointmentStatusType.SCHEDULED;
            return await ScheduleAppointmentAsync(appointment);
        }

        public async Task<OperationResult<Guid, GenericError>> ScheduleAppointmentAsStaffAsync(Appointment appointment)
        {
            appointment.Status = AppointmentStatusType.CONFIRMED;
            return await ScheduleAppointmentAsync(appointment);
        }

        private async Task<OperationResult<Guid, GenericError>> ScheduleAppointmentAsync(Appointment appointment)
        {
            DateTime currentDateTimeUtc = DateTime.UtcNow;

            int MAX_DAYS_FROM_NOW = int.Parse(envService.Get("MAX_DAYS_FOR_SCHEDULE"));
            int MAX_WEEKS_FOR_SCHEDULE = int.Parse(envService.Get("MAX_WEEKS_FOR_SCHEDULE"));
            int MAX_MONTHS_FOR_SCHEDULE = int.Parse(envService.Get("MAX_MONTHS_FOR_SCHEDULE"));
            bool IsPastSchedulingAllowed = bool.Parse(envService.Get("ALLOW_SCHEDULE_IN_THE_PAST"));
            int MAX_SERVICES_PER_CLIENT = int.Parse(envService.Get("MAX_SERVICES_PER_CLIENT"));
            int MAX_APPOINTMENTS_PER_CLIENT = int.Parse(envService.Get("MAX_APPOINTMENTS_PER_CLIENT"));

            DateTime maxDateUtc = currentDateTimeUtc
                .AddMonths(MAX_MONTHS_FOR_SCHEDULE)
                .AddDays(MAX_WEEKS_FOR_SCHEDULE * 7)
                .AddDays(MAX_DAYS_FROM_NOW);

            // Convert appointment.StartDate to UTC real
            DateTime startDateTimeUtc = appointment.StartDate.ToUniversalTime();

            // 1. Check max services per client
            if (appointment.ScheduledServices!.Count > MAX_SERVICES_PER_CLIENT)
            {
                GenericError genericError = new GenericError(
                    $"Too many services selected for the appointment. You can only select up to {MAX_SERVICES_PER_CLIENT} service(s) per appointment",
                    []
                );
                genericError.AddData("MaxServicesPerAppointment", MAX_SERVICES_PER_CLIENT);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_SERVICES_LIMIT_REACHED);
            }

            // 2. Check past scheduling
            if (!IsPastSchedulingAllowed && startDateTimeUtc < currentDateTimeUtc)
            {
                GenericError genericError = new GenericError(
                    $"You cannot schedule an appointment in the past. You can only schedule from the current time to {maxDateUtc}",
                    []
                );
                genericError.AddData("SelectedDateTime", startDateTimeUtc);
                genericError.AddData("SuggestedDateTime", currentDateTimeUtc.AddMinutes(1));
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_SCHEDULING_IN_THE_PAST_NOT_ALLOWED);
            }

            // 3. Check max date
            if (startDateTimeUtc > maxDateUtc)
            {
                GenericError genericError = new GenericError(
                    $"You cannot schedule an appointment beyond {maxDateUtc}.",
                    []
                );
                genericError.AddData("SelectedDateTime", startDateTimeUtc);
                genericError.AddData("SuggestedDateTime", currentDateTimeUtc.AddMinutes(5));
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_SCHEDULING_BEYOND_X_NOT_ALLOWED);
            }

            // 4. Get Client data
            var clientData = await clientMgr.GetClientByUuidAsync(appointment.Client!.Uuid!.Value);
            if (clientData == null)
            {
                GenericError genericError = new GenericError(
                    $"Client UUID: <{appointment.Client.Uuid.Value}> is not registered", []
                );
                genericError.AddData("clientUuid", appointment.Client.Uuid.Value);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }

            if (clientData.Status != AccountStatusType.ENABLED)
            {
                GenericError genericError = new GenericError(
                    $"Client with UUID <{clientData.Uuid}> is not available. Client was disabled or deleted!", []
                );
                genericError.AddData("clientUuid", clientData.Uuid!.Value);
                genericError.AddData("Status", clientData.Status!.Value.ToString());
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_AVAILABLE);
            }
            appointment.Client = clientData;

            // 5. Check max appointments per client
            int scheduledAppointmentsCount = await schedulerMgr.GetAppointmentsScheduledCountByClientId(clientData.Id!.Value);
            if (scheduledAppointmentsCount >= MAX_APPOINTMENTS_PER_CLIENT)
            {
                timeRangeLockMgr.UnblockTimeSlot(clientData.Uuid!.Value);
                GenericError genericError = new GenericError(
                    $"The client with UUID {clientData.Uuid} has reached the maximum allowed number of appointments {MAX_APPOINTMENTS_PER_CLIENT}", []
                );
                genericError.AddData("MaxAppointmentsPerClient", MAX_APPOINTMENTS_PER_CLIENT);
                genericError.AddData("AppointmentsScheduledCount", scheduledAppointmentsCount);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_SCHEDULED_LIMIT_REACHED);
            }

            // 6. Get and validate services
            for (int i = 0; i < appointment.ScheduledServices!.Count; i++)
            {
                var serviceOffer = appointment.ScheduledServices[i];

                // Convert ServiceStartDate to UTC
                DateTime serviceStartUtc = serviceOffer.ServiceStartDate!.Value.ToUniversalTime();

                ServiceOffer? serviceOfferData = await assistantMgr.GetServiceOfferByUuidAsync(serviceOffer.Uuid!.Value);
                if (serviceOfferData == null)
                {
                    GenericError genericError = new GenericError($"Service <{serviceOffer.Uuid.Value}> is not registered", []);
                    genericError.AddData("SelectedServiceUuid", serviceOffer.Uuid.Value);
                    return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
                }

                if (serviceOfferData.Assistant!.Status != AccountStatusType.ENABLED || serviceOfferData.Service!.Status != ServiceStatusType.ENABLED)
                {
                    GenericError genericError = new GenericError(
                        $"Service or assistant is deleted or disabled. ServiceOffer with UUID <{serviceOffer.Uuid.Value}> is unavailable", []
                    );
                    genericError.AddData("SelectedServiceUuid", serviceOfferData.Uuid!.Value);
                    genericError.AddData("AssistantStatus", serviceOfferData.Assistant!.Status!.Value.ToString());
                    genericError.AddData("ServiceStatus", serviceOfferData.Service!.Status!.Value.ToString());
                    genericError.AddData("SelectedServiceStatus", serviceOfferData.Status!.Value.ToString());
                    return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_UNAVAILABLE);
                }

                if (serviceOfferData.Status == ServiceOfferStatusType.DISABLED)
                {
                    GenericError genericError = new GenericError(
                        $"ServiceOffer with UUID <{serviceOffer.Uuid.Value}> is unavailable", []
                    );
                    genericError.AddData("SelectedServiceUuid", serviceOffer.Uuid.Value);
                    genericError.AddData("ServiceOfferStatus", serviceOfferData.Status);
                    return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_UNAVAILABLE);
                }

                appointment.ScheduledServices[i].ServiceOffer = serviceOfferData;
                appointment.ScheduledServices[i].ServiceStartDate = serviceStartUtc;
                appointment.ScheduledServices[i].ServiceEndDate = serviceStartUtc.AddMinutes(serviceOfferData.Service!.Minutes!.Value).ToUniversalTime();
                appointment.ScheduledServices[i].ServiceName = serviceOfferData.Service.Name;
                appointment.ScheduledServices[i].ServicesMinutes = serviceOfferData.Service.Minutes;
                appointment.ScheduledServices[i].ServicePrice = serviceOfferData.Service.Price;
            }

            appointment.ScheduledServices = appointment.ScheduledServices.OrderBy(so => so.ServiceStartDate).ToList();

            // 7. Validate contiguous services
            List<GenericError> errorMessages = new();
            for (int i = 1; i < appointment.ScheduledServices.Count; i++)
            {
                var prevService = appointment.ScheduledServices[i - 1];
                var currentService = appointment.ScheduledServices[i];

                if (currentService.ServiceStartDate != prevService.ServiceEndDate)
                {
                    GenericError genericError = new GenericError(
                        $"Service with UUID <{currentService!.Uuid!.Value}> is not contiguous with the previous service <{prevService!.Uuid!.Value}>.", []
                    );
                    genericError.AddData($"{currentService.Uuid.Value}", prevService.ServiceEndDate!.Value);
                    errorMessages.Add(genericError);
                }
            }

            if (errorMessages.Any())
                return OperationResult<Guid, GenericError>.Failure(errorMessages, MessageCodeType.SERVICES_ARE_NOT_CONTIGUOUS);

            // 8. Calculate total cost and EndDate
            appointment.TotalCost = appointment.ScheduledServices.Sum(service => service.ServicePrice!.Value);
            appointment.EndDate = appointment.StartDate.AddMinutes(appointment.ScheduledServices.Sum(service => service.ServicesMinutes!.Value));

            DateTimeRange appointmentRange = new()
            {
                StartDate = appointment.StartDate.ToUniversalTime(),
                EndDate = appointment.EndDate.ToUniversalTime()
            };

            // 9. Check blocked time slot by client
            OperationResult<BlockedTimeSlot, GenericError> timeRangeBlockedByUser = timeRangeLockMgr.GetBlockedTimeSlotByClientUuid(clientData.Uuid!.Value);
            if (!timeRangeBlockedByUser.IsSuccessful)
            {
                timeRangeBlockedByUser.Error!.AddData("TryBlockDateTimeRangeFirst", appointmentRange);
                timeRangeBlockedByUser.Error.Message += " Try to block the date time range first.";
                return OperationResult<Guid, GenericError>.Failure(timeRangeBlockedByUser.Error!, timeRangeBlockedByUser.Code);
            }

            if (!(appointmentRange.EndDate >= timeRangeBlockedByUser.Result!.TotalServicesTimeRange!.EndDate
                  && appointmentRange.EndDate <= timeRangeBlockedByUser.Result.TotalServicesTimeRange.EndDate)
                && !timeRangeBlockedByUser.Result!.Equals(appointmentRange))
            {
                bool hasConflictWithAnotherSlot = await schedulerMgr.IsAppointmentTimeSlotAvailableAsync(appointmentRange);
                if (hasConflictWithAnotherSlot)
                {
                    GenericError genericError = new GenericError($"The new date time range selected has conflicts with another appointments.", []);
                    genericError.AddData("BlockedTimeRange", timeRangeBlockedByUser.Result);
                    genericError.AddData("SelectedTimeRange", appointmentRange);
                    return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_SLOT_UNAVAILABLE);
                }

                OperationResult<bool, GenericError> extendTimeRange = timeRangeLockMgr.ExtendBlockedTimeSlot(appointmentRange, clientData.Uuid!.Value);
                if (!extendTimeRange.IsSuccessful)
                {
                    GenericError error = new GenericError($"Cannot extend blocked time range, another blocked range overlaps. Remove some services", []);
                    error.AddData("BlockedTimeRange", timeRangeBlockedByUser.Result);
                    error.AddData("SelectedTimeRange", appointmentRange);
                    return OperationResult<Guid, GenericError>.Failure(error, MessageCodeType.APPOINTMENT_TIME_RANGE_LOCK_CANNOT_BE_EXTENDED);
                }
            }

            // 10. Check assistant availability and conflicts
            foreach (var scheduledService in appointment.ScheduledServices)
            {
                DateTime serviceStartUtc = scheduledService.ServiceStartDate!.Value.ToUniversalTime();
                DateTime serviceEndUtc = scheduledService.ServiceEndDate!.Value.ToUniversalTime();

                DateTimeRange serviceRange = new()
                {
                    StartDate = serviceStartUtc,
                    EndDate = serviceEndUtc
                };

                bool isAssistantAvailable = await schedulerMgr.IsAssistantAvailableInAvailabilityTimeSlotsAsync(
                    serviceRange,
                    scheduledService.ServiceOffer!.Assistant!.Id!.Value
                );

                if (!isAssistantAvailable)
                {
                    GenericError error = new GenericError(
                        $"Assistant: <{scheduledService.ServiceOffer.Assistant!.Uuid!.Value}> is not available during the requested time range", []
                    );
                    error.AddData("SelectedServiceUuid", scheduledService.Uuid!.Value);
                    error.AddData("SelectedServiceStartDate", serviceRange.StartDate);
                    error.AddData("SelectedServiceEndDate", serviceRange.EndDate);
                    error.AddData("AssistantUuid", scheduledService.ServiceOffer.Assistant!.Uuid!.Value);
                    error.AddData("AssistantName", scheduledService.ServiceOffer.Assistant!.Name!);
                    return OperationResult<Guid, GenericError>.Failure(error, MessageCodeType.ASSISTANT_NOT_AVAILABLE_IN_TIME_RANGE);
                }

                bool hasConflict = await schedulerMgr.HasAssistantConflictingAppoinmentsAsync(
                    serviceRange,
                    scheduledService.ServiceOffer.Assistant!.Id!.Value
                );

                if (hasConflict)
                {
                    GenericError error = new GenericError(
                        $"Assistant: <{scheduledService.ServiceOffer.Assistant!.Uuid!.Value}> is attending another appointment during the requested time range", []
                    );
                    error.AddData("SelectedServiceUuid", scheduledService.Uuid!.Value);
                    error.AddData("SelectedServiceStartDate", serviceRange.StartDate);
                    error.AddData("SelectedServiceEndDate", serviceRange.EndDate);
                    error.AddData("AssistantUuid", scheduledService.ServiceOffer.Assistant!.Uuid!.Value);
                    error.AddData("AssistantName", scheduledService.ServiceOffer.Assistant!.Name!);
                    return OperationResult<Guid, GenericError>.Failure(error, MessageCodeType.APPOINTMENT_SELECTED_SERVICE_CONFLICT_WITH_TIME_SLOT);
                }
            }

            // 11. Schedule the appointment
            Guid? UuidRegistered = await schedulerMgr.ScheduleAppointmentAsync(appointment);
            timeRangeLockMgr.UnblockTimeSlot(clientData.Uuid!.Value);
            if (UuidRegistered == null)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError("An error has occurred!"), MessageCodeType.REGISTER_ERROR);
            }

            return OperationResult<Guid, GenericError>.Success(UuidRegistered.Value);
        }

        public async Task<OperationResult<JwtTokenResult, GenericError>> LoginWithEmailOrUsernameOrPhoneNumberJwtTokenAsync(string account, string password)
        {
            AccountData? accountData = await accountMgr.GetAccountDataByEmailOrUsernameOrPhoneNumberAsync(account, password);
            if (accountData == null)
            {
                GenericError genericError = new GenericError($"Account <{account}> not found", []);
                genericError.AddData("field", "Account");
                genericError.AddData("account", account);
                return OperationResult<JwtTokenResult, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_LOGIN_WRONG_CREDENTIALS);
            }

            JwtUserCredentials credentials = new JwtUserCredentials
            {
                Username = accountData.Username!,
                Role = accountData.Role!.Value.ToString(),
                Email = accountData.Email!,
                Uuid = accountData.Uuid!.Value.ToString()
            };

            JwtTokenResult? token = authJwtService.Authenticate(credentials);

            if (token == null)
            {
                return OperationResult<JwtTokenResult, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.AUTHENTICATION_FAILED_TO_GENERATE_JWT_TOKEN);
            }

            return OperationResult<JwtTokenResult, GenericError>.Success(token);
        }

        public Task<OperationResult<JwtTokenResult, GenericError>> RefreshToken(string token)
        {

            JwtTokenResult tokenResult = new JwtTokenResult
            {
                Token = token
            };

            JwtTokenData? tokenData = authJwtService.ValidateCredentials(tokenResult);
            if (tokenData == null)
            {
                return Task.FromResult(OperationResult<JwtTokenResult, GenericError>.Failure(new GenericError("Token is not valid"), MessageCodeType.AUTHENTICATION_INVALID_CREDENTIALS));
            }

            JwtUserCredentials credentials = new JwtUserCredentials
            {
                Email = tokenData!.Email,
                Username = tokenData!.Username,
                Role = tokenData.Role,
                Uuid = tokenData.Uuid
            };

            JwtTokenResult? newToken = authJwtService.RefreshCredentials(credentials);
            if (newToken == null)
            {
                return Task.FromResult(OperationResult<JwtTokenResult, GenericError>.Failure(new GenericError("Cannot refresh token!"), MessageCodeType.AUTHENTICATION_CANNOT_REFRESH_TOKEN));
            }
            return Task.FromResult(OperationResult<JwtTokenResult, GenericError>.Success(newToken));
        }

        public Task<OperationResult<AccountData, GenericError>> ValidateCredentials(string token)
        {
            JwtTokenResult tokenResult = new JwtTokenResult
            {
                Token = token
            };
            JwtTokenData? tokenData = authJwtService.ValidateCredentials(tokenResult);
            if (tokenData == null)
            {
                return Task.FromResult(OperationResult<AccountData, GenericError>.Failure(new GenericError("Token is not valid"), MessageCodeType.AUTHENTICATION_INVALID_CREDENTIALS));
            }

            AccountData accountData = new AccountData
            {
                Username = tokenData.Username,
                Email = tokenData.Email,
                Uuid = Guid.Parse(tokenData.Uuid),
                Role = Enum.TryParse<RoleType>(tokenData.Role, true, out var roleValue) ? roleValue : null,
            };
            return Task.FromResult(OperationResult<AccountData, GenericError>.Success(accountData));
        }

        private OperationResult<bool, GenericError> CheckServiceStatusType(Service? serviceData, ServiceStatusType newStatus)
        {
            if (serviceData == null)
            {
                GenericError genericError = new GenericError($"Service not found", []);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
            }

            if (serviceData.Status == ServiceStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot change status of Service with UUID <{serviceData!.Uuid!.Value}>. Service was deleted!", []);
                genericError.AddData("ServiceUuid", serviceData!.Uuid!.Value);
                genericError.AddData("Status", ServiceStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_WAS_DELETED);
            }

            if (newStatus == serviceData.Status)
            {
                string message;
                MessageCodeType messageCodeType;
                if (newStatus == ServiceStatusType.ENABLED)
                {
                    message = $"Service with UUID: <{serviceData!.Uuid!.Value}> is already enabled";
                    messageCodeType = MessageCodeType.SERVICE_IS_ALREADY_ENABLED;
                }
                else if (newStatus == ServiceStatusType.DISABLED)
                {
                    message = $"Service with UUID: <{serviceData!.Uuid!.Value}> is already disabled";
                    messageCodeType = MessageCodeType.SERVICE_IS_ALREADY_DISABLED;
                }
                else
                {
                    throw new KeyNotFoundException($"The value '{newStatus}' was not found in the expected {nameof(ServiceStatusType)} collection.");
                }
                GenericError genericError = new GenericError(message);
                genericError.AddData("ServiceUuid", serviceData!.Uuid!.Value);
                genericError.AddData("Status", serviceData.Status!.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, messageCodeType);
            }
            return OperationResult<bool, GenericError>.Success(true, MessageCodeType.OK);
        }

        private OperationResult<bool, GenericError> CheckAvailabilityTimeSlotStatusType(AvailabilityTimeSlot? slotData, AvailabilityTimeSlotStatusType newStatus)
        {
            if (slotData == null)
            {
                GenericError genericError = new GenericError($"AvailabilityTimeSlot not found", []);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.AVAILABILITY_TIME_SLOT_NOT_FOUND);
            }

            if (slotData.Status == AvailabilityTimeSlotStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"AvailabilityTimeSlot with UUID: <{slotData.Uuid!.Value}> was deleted", []);
                genericError.AddData("AvailabilityTimeSlotUuid", slotData.Uuid);
                genericError.AddData("Status", slotData.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.AVAILABILITY_TIME_SLOT_IS_ALREADY_DELETED);
            }

            if (newStatus == slotData.Status)
            {
                string message;
                MessageCodeType messageCodeType;
                if (newStatus == AvailabilityTimeSlotStatusType.ENABLED)
                {
                    message = $"AvailabilityTimeSlot with UUID: <{slotData.Uuid}> is already enabled";
                    messageCodeType = MessageCodeType.AVAILABILITY_TIME_SLOT_IS_ALREADY_ENABLED;
                }
                else if (newStatus == AvailabilityTimeSlotStatusType.DISABLED)
                {
                    message = $"AvailabilityTimeSlot with UUID: <{slotData.Uuid}> is already disabled";
                    messageCodeType = MessageCodeType.AVAILABILITY_TIME_SLOT_IS_ALREADY_DISABLED;
                }
                else
                {
                    throw new KeyNotFoundException($"The value '{newStatus}' was not found in the expected {nameof(AvailabilityTimeSlotStatusType)} collection.");
                }
                GenericError genericError = new GenericError(message);
                genericError.AddData("AvailabilityTimeSlotUuid", slotData!.Uuid!.Value);
                genericError.AddData("Status", slotData.Status);
                return OperationResult<bool, GenericError>.Failure(genericError, messageCodeType);
            }
            return OperationResult<bool, GenericError>.Success(true, MessageCodeType.OK);
        }

        private OperationResult<bool, GenericError> CheckClientStatusType(Client? clientData, AccountStatusType newStatus)
        {
            if (clientData == null)
            {
                GenericError genericError = new GenericError($"Client not found", []);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }

            if (clientData.Status == AccountStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Client with UUID: <{clientData!.Uuid!.Value}> is already deleted", []);
                genericError.AddData("clientUuid", clientData.Uuid.Value);
                genericError.AddData("Status", clientData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_IS_ALREADY_DELETED);
            }

            if (newStatus == clientData.Status)
            {
                string message;
                MessageCodeType messageCodeType;
                if (newStatus == AccountStatusType.ENABLED)
                {
                    message = $"Client with UUID: <{clientData.Uuid}> is already enabled";
                    messageCodeType = MessageCodeType.CLIENT_IS_ALREADY_ENABLED;
                }
                else if (newStatus == AccountStatusType.DISABLED)
                {
                    message = $"Client with UUID: <{clientData.Uuid}> is already disabled";
                    messageCodeType = MessageCodeType.CLIENT_IS_ALREADY_DISABLED;
                }
                else
                {
                    throw new KeyNotFoundException($"The value '{newStatus}' was not found in the expected {nameof(AccountStatusType)} collection.");
                }
                GenericError genericError = new GenericError(message);
                genericError.AddData("clientUuid", clientData.Uuid!.Value);
                genericError.AddData("Status", clientData.Status!.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, messageCodeType);
            }
            return OperationResult<bool, GenericError>.Success(true, MessageCodeType.OK);
        }

        private OperationResult<bool, GenericError> CheckAssistantStatusType(Assistant? asisstantData, AccountStatusType newStatus)
        {
            if (asisstantData == null)
            {
                GenericError genericError = new GenericError($"Assistant not found", []);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (asisstantData.Status == AccountStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Assistant with UUID: <{asisstantData!.Uuid!.Value}> is already deleted", []);
                genericError.AddData("assistantUuid", asisstantData.Uuid.Value);
                genericError.AddData("Status", asisstantData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_IS_ALREADY_DELETED);
            }

            if (newStatus == asisstantData.Status)
            {
                string message;
                MessageCodeType messageCodeType;
                if (newStatus == AccountStatusType.ENABLED)
                {
                    message = $"Assistant with UUID: <{asisstantData.Uuid}> is already enabled";
                    messageCodeType = MessageCodeType.ASSISTANT_IS_ALREADY_ENABLED;
                }
                else if (newStatus == AccountStatusType.DISABLED)
                {
                    message = $"Assistant with UUID: <{asisstantData.Uuid}> is already disabled";
                    messageCodeType = MessageCodeType.ASSISTANT_IS_ALREADY_DISABLED;
                }
                else
                {
                    throw new KeyNotFoundException($"The value '{newStatus}' was not found in the expected {nameof(AccountStatusType)} collection.");
                }
                GenericError genericError = new GenericError(message);
                genericError.AddData("assistantUuid", asisstantData.Uuid!.Value);
                genericError.AddData("Status", asisstantData.Status!.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, messageCodeType);
            }
            return OperationResult<bool, GenericError>.Success(true, MessageCodeType.OK);
        }

        public OperationResult<bool, GenericError> CheckServiceOfferStatusType(ServiceOffer? serviceOfferData, ServiceOfferStatusType newStatus)
        {
            if (serviceOfferData == null)
            {
                GenericError genericError = new GenericError($"ServiceOffer not found", []);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_NOT_FOUND);
            }

            if (serviceOfferData.Status == ServiceOfferStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"ServiceOffer with UUID: <{serviceOfferData!.Uuid!.Value}> is already deleted", []);
                genericError.AddData("serviceOfferUuid", serviceOfferData.Uuid.Value);
                genericError.AddData("Status", serviceOfferData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_WAS_DELETED);
            }

            if (newStatus == serviceOfferData.Status)
            {
                string message;
                MessageCodeType messageCodeType;
                if (newStatus == ServiceOfferStatusType.ENABLED)
                {
                    message = $"ServiceOffer with UUID: <{serviceOfferData.Uuid!.Value}> is already enabled";
                    messageCodeType = MessageCodeType.SERVICE_IS_ALREADY_ENABLED;
                }
                else if (newStatus == ServiceOfferStatusType.DISABLED)
                {
                    message = $"ServiceOffer with UUID: <{serviceOfferData.Uuid!.Value}> is already disabled";
                    messageCodeType = MessageCodeType.SERVICE_OFFER_IS_ALREADY_DISABLED;
                }
                else
                {
                    throw new KeyNotFoundException($"The value '{newStatus}' was not found in the expected {nameof(ServiceOfferStatusType)} collection.");
                }
                GenericError genericError = new GenericError(message);
                genericError.AddData("ServiceUuid", serviceOfferData.Uuid!.Value);
                genericError.AddData("Status", serviceOfferData.Status);

                return OperationResult<bool, GenericError>.Failure(genericError, messageCodeType);
            }
            return OperationResult<bool, GenericError>.Success(true, MessageCodeType.OK);
        }

        public async Task<OperationResult<AccountData, GenericError>> GetAccountData(Guid accountUuid)
        {
            AccountData? accountData = await accountMgr.GetAccountDataByUuid(accountUuid);
            if (accountData == null)
            {
                GenericError genericError = new GenericError($"Account not found", []);
                return OperationResult<AccountData, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_NOT_FOUND);
            }


            if (accountData.Status != AccountStatusType.ENABLED)
            {
                GenericError genericError = new GenericError($"Account with UUID <{accountData.Uuid}> is not available. Account was disabled or deleted!", []);
                genericError.AddData("accountUuid", accountData.Uuid!.Value);
                genericError.AddData("Status", accountData.Status!.Value.ToString());
                return OperationResult<AccountData, GenericError>.Failure(genericError, MessageCodeType.ACCOUNT_NOT_AVAILABLE);
            }
            return OperationResult<AccountData, GenericError>.Success(accountData, MessageCodeType.OK);
        }

        public async Task<OperationResult<List<ServiceOffer>, GenericError>> GetAllAssignedServicesAsync(Guid uuid)
        {
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(uuid);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Assistant with UUID <{uuid}> is not registered", []);
                genericError.AddData("AssistantUuid", uuid);
                return OperationResult<List<ServiceOffer>, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (assistantData.Status == AccountStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot modify Assistant with UUID <{assistantData.Uuid}>. Assistant was deleted!", []);
                genericError.AddData("AssistantUuid", assistantData.Uuid!.Value);
                genericError.AddData("Status", AssistantStatusType.DELETED.ToString());
                return OperationResult<List<ServiceOffer>, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_WAS_DELETED);
            }
            List<ServiceOffer> services = await assistantMgr.GetAssignedServicesOfAssistantByUuidAsync(uuid);
            return OperationResult<List<ServiceOffer>, GenericError>.Success(services);
        }

        public async Task<List<ServiceOffer>> GetAllAssistantsAndServicesOffer()
        {
            List<ServiceOffer> services = await assistantMgr.GetAllAssistantsAndServicesOffer();
            return services;
        }
    }
}