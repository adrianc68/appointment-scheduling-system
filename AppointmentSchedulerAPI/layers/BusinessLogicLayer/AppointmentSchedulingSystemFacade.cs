using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Interfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer
{
    public class AppointmentSchedulingSystemFacade : ISchedulingInterfaces, IServiceInterfaces, IClientInterfaces, IAssistantInterfaces
    {
        private readonly IServiceMgt serviceMgr;
        private readonly ISchedulerMgt schedulerMgr;
        private readonly IAssistantMgt assistantMgr;
        private readonly IClientMgt clientMgr;
        private readonly EnvironmentVariableService envService;
        private readonly ITimeRangeLockMgt timeRangeLockMgr;

        public AppointmentSchedulingSystemFacade(IServiceMgt serviceMgr, IAssistantMgt assistantMgr, IClientMgt clientMgr, ISchedulerMgt schedulerMgr, ITimeRangeLockMgt timeRangeLockMgr, EnvironmentVariableService envService)
        {
            this.serviceMgr = serviceMgr;
            this.schedulerMgr = schedulerMgr;
            this.assistantMgr = assistantMgr;
            this.clientMgr = clientMgr;
            this.timeRangeLockMgr = timeRangeLockMgr;
            this.envService = envService;
        }

        public Task<OperationResult<bool, GenericError>> EditAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public OperationResult<List<SchedulingBlock>, GenericError> GetSchedulingBlockRanges(DateOnly date)
        {
            return timeRangeLockMgr.GetSchedulingBlockByDate(date);
        }

        public async Task<OperationResult<DateTime, GenericError>> BlockTimeRange(List<ScheduledService> services, DateTimeRange range, Guid clientUuid)
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
            DateTime startDateTime = range.Date!.ToDateTime(range.StartTime);

            // Check for past scheduling
            if (!IsPastSchedulingAllowed && startDateTime < currentDateTime)
            {
                GenericError genericError = new GenericError($"You cannot schedule an appoinment in the past. You can only schedule from the current time", []);
                genericError.AddData("SelectedDateTime", startDateTime.ToUniversalTime());
                genericError.AddData("SuggestedDateTime", currentDateTime.AddMinutes(1).ToUniversalTime());
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.CANNOT_SCHEDULE_APPOINTMENT_IN_THE_PAST);
            }

            if (startDateTime > maxDate)
            {
                GenericError genericError = new GenericError($"You cannot schedule an appoinment beyond {maxDate}.", []);
                genericError.AddData("SelectedDateTime", startDateTime.ToUniversalTime());
                genericError.AddData("SuggestedDateTime", currentDateTime.AddMinutes(5).ToUniversalTime());
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.CANNOT_SCHEDULE_APPOINTMENT_BEYOND_X);
            }

            // Check max services per client
            if (services.Count > MAX_SERVICES_PER_CLIENT)
            {
                GenericError genericError = new GenericError($"Too many services selected for the appointment. You can only select up to {MAX_SERVICES_PER_CLIENT} service(s) per appointment", []);
                genericError.AddData("MaxServicesPerAppointment", MAX_SERVICES_PER_CLIENT);
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.TOO_MANY_SERVICES_SELECTED);
            }



            // Get Client data
            var clientData = await clientMgr.GetClientByUuidAsync(clientUuid);
            if (clientData == null)
            {
                GenericError genericError = new GenericError($"Client UUID: <{clientUuid}> is not registered", []);
                genericError.AddData("clientUuid", clientUuid);
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }

            if (clientData.Status != ClientStatusType.ENABLED)
            {
                GenericError genericError = new GenericError($"Client with UUID <{clientData.Uuid}> is not available. Client was disabled or deleted!", []);
                genericError.AddData("clientUuid", clientData.Uuid!.Value);
                genericError.AddData("Status", clientData.Status!.Value.ToString());
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.CLIENT_UNAVAILABLE);
            }

            // Check max allowed appoinments per client
            int scheduledAppoinmentsOfClientCount = await schedulerMgr.GetAppointmentsScheduledCountByClientId(clientData.Id!.Value);
            if (scheduledAppoinmentsOfClientCount >= MAX_APPOINTMENTS_PER_CLIENT)
            {
                timeRangeLockMgr.UnblockTimeRange(clientData.Uuid!.Value);
                GenericError genericError = new GenericError($"The client with UUID {clientData.Uuid} has reached the maximum allowed number of appoinments {MAX_APPOINTMENTS_PER_CLIENT}", []);
                genericError.AddData("MaxAppointmentsPerClient", MAX_APPOINTMENTS_PER_CLIENT);
                genericError.AddData("AppointmentsScheduledCount", scheduledAppoinmentsOfClientCount);
                return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.MAX_SCHEDULED_APPOINTMENTS_REACHED);
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

                if (serviceOfferData.Assistant!.Status != AssistantStatusType.ENABLED || serviceOfferData.Service!.Status != ServiceStatusType.ENABLED)
                {
                    GenericError genericError = new GenericError($"Service or assistant is deleted or disabled. ServiceOffer with UUID <{serviceOffer.Uuid.Value}> is unavailable", []);
                    genericError.AddData("SelectedServiceUuid", serviceOfferData.Uuid!.Value);
                    genericError.AddData("AssistantStatus", serviceOfferData.Assistant!.Status!.Value.ToString());
                    genericError.AddData("ServiceStatus", serviceOfferData.Service!.Status!.Value.ToString());
                    genericError.AddData("SelectedServiceStatus", serviceOfferData.Status!.Value.ToString());
                    return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_UNAVAILABLE);
                }

                if (serviceOfferData.Status == ServiceOfferStatusType.NOT_AVAILABLE)
                {
                    GenericError genericError = new GenericError($"ServiceOffer with UUID <{serviceOffer.Uuid.Value}> is unavailable", []);
                    genericError.AddData("SelectedServiceUuid", serviceOffer.Uuid.Value);
                    genericError.AddData("ServiceOfferStatus", serviceOfferData.Status);
                    return OperationResult<DateTime, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_UNAVAILABLE);
                }
                services[i].ServicesMinutes = serviceOfferData.Service.Minutes;
                services[i].ServiceEndTime = services[i].ServiceStartTime!.Value.AddMinutes(services[i].ServicesMinutes!.Value);
                services[i].ServicePrice = serviceOfferData.Service.Price;
                services[i].ServiceOffer = serviceOfferData;
            }

            services = services.OrderBy(so => so.ServiceStartTime).ToList();
            
            List<GenericError> errorMessages = [];
            for (int i = 1; i < services.Count; i++)
            {
                var prevService = services[i - 1];
                var currentService = services[i];

                if (currentService.ServiceStartTime != prevService.ServiceEndTime)
                {
                    GenericError genericError = new GenericError($"Service with UUID <{currentService!.Uuid!.Value}> is not contiguous with the previous service. <{prevService!.Uuid!.Value}>. Suggestions <ServiceOfferUuid>:<StartTime>:", []);
                    genericError.AddData($"{currentService.Uuid.Value}", prevService.ServiceEndTime!.Value);
                    errorMessages.Add(genericError);
                }
            }

            if (errorMessages.Any())
            {
                var result = OperationResult<DateTime, GenericError>.Failure(errorMessages, MessageCodeType.SERVICES_ARE_NOT_CONTIGUOUS);
                return result;
            }

            range.StartTime = services.Min(asd => asd.ServiceStartTime!.Value);
            range.EndTime = range.StartTime.AddMinutes(services.Sum(service => service.ServicesMinutes!.Value));



            foreach (var scheduledService in services)
            {
                TimeOnly proposedStartTime = TimeOnly.Parse(scheduledService.ServiceStartTime!.Value.ToString());
                TimeOnly proposedEndTime = proposedStartTime.AddMinutes(scheduledService.ServicesMinutes!.Value);

                DateTimeRange serviceRange = new()
                {
                    StartTime = scheduledService.ServiceStartTime.Value,
                    EndTime = proposedEndTime,
                    Date = range.Date,
                };

                bool isAssistantAvailableInAvailabilityTimeSlots = await schedulerMgr.IsAssistantAvailableInAvailabilityTimeSlotsAsync(serviceRange, scheduledService.ServiceOffer!.Assistant!.Id!.Value);

                if (!isAssistantAvailableInAvailabilityTimeSlots)
                {
                    GenericError error = new GenericError($"Assistant: <{scheduledService.ServiceOffer.Assistant!.Uuid!.Value}> is not available during the requested time range", []);
                    error.AddData("SelectedServiceUuid", scheduledService.Uuid!.Value);
                    error.AddData("SelectedServiceStartTime", serviceRange.StartTime);
                    error.AddData("SelectedServiceEndTime", serviceRange.EndTime);
                    error.AddData("AssistantUuid", scheduledService.ServiceOffer.Assistant!.Uuid!.Value);
                    error.AddData("AssistantName", scheduledService.ServiceOffer.Assistant!.Name!);
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.ASSISTANT_NOT_AVAILABLE_IN_TIME_RANGE);
                }

                bool hasAssistantConflictingAppoinments = await schedulerMgr.HasAssistantConflictingAppoinmentsAsync(serviceRange, scheduledService.ServiceOffer.Assistant!.Id!.Value);
                if (!hasAssistantConflictingAppoinments)
                {
                    GenericError error = new GenericError($"Assistant: <{scheduledService.ServiceOffer.Assistant!.Uuid!.Value}> is attending another appointment during the requested time range", []);
                    error.AddData("SelectedServiceUuid", scheduledService.Uuid!.Value);
                    error.AddData("SelectedServiceStartTime", serviceRange.StartTime);
                    error.AddData("SelectedServiceEndTime", serviceRange.EndTime);
                    error.AddData("AssistantUuid", scheduledService.ServiceOffer.Assistant!.Uuid!.Value);
                    error.AddData("AssistantName", scheduledService.ServiceOffer.Assistant!.Name!);
                    return OperationResult<DateTime, GenericError>.Failure(error, MessageCodeType.SELECTED_SERVICE_HAS_CONFLICTING_APPOINTMENT_TIME_SLOT);
                }
            }

        
            List<ServiceWithTime> selectedServices = services.Select(service => new ServiceWithTime
            {
                StartTime = service.ServiceStartTime!.Value,
                EndTime = service.ServiceEndTime!.Value,
                ServiceUuid = service.Uuid!.Value,
                AssistantUuid = service.ServiceOffer!.Assistant!.Uuid!.Value
            }).ToList();

            return timeRangeLockMgr.BlockTimeRange(selectedServices, range, clientUuid);
        }

        public OperationResult<bool, GenericError> UnblockTimeRange(Guid clientUuid)
        {
            return timeRangeLockMgr.UnblockTimeRange(clientUuid);
        }


        public async Task<OperationResult<bool, GenericError>> EditAvailabilityTimeSlot(AvailabilityTimeSlot availabilityTimeSlot)
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
            if (!(availabilityTimeSlot.StartTime < availabilityTimeSlot.EndTime || availabilityTimeSlot.StartTime == TimeOnly.MinValue))
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

            if (assistantData.Status != AssistantStatusType.ENABLED)
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
                Date = availabilityTimeSlot.Date!.Value,
                StartTime = availabilityTimeSlot.StartTime!.Value,
                EndTime = availabilityTimeSlot.EndTime!.Value
            };

            bool hasConflictWithAnotherSlot = await schedulerMgr.HasAvailabilityTimeSlotConflictingSlotsAsync(range, slotData!.Id!.Value, assistantData.Id!.Value);
            if (hasConflictWithAnotherSlot)
            {
                GenericError genericError = new("Time range has conflicts with another slots", []);
                genericError.AdditionalData!.Add("Range", range);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.AVAILABILITY_TIME_SLOT_HAS_CONFLICTS);
            }

            bool isUpdated = await schedulerMgr.UpdateAvailabilityTimeSlot(availabilityTimeSlot);
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
            List<Appointment>? appointment = await schedulerMgr.GetAllAppoinments(startDate, endDate);
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

        public async Task<OperationResult<bool, GenericError>> UpdateAssistant(Assistant assistant)
        {
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(assistant.Uuid!.Value);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Assistant with UUID <{assistant.Uuid.Value}> is not registered", []);
                genericError.AddData("AssistantUuid", assistant.Uuid.Value);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (assistantData.Status == AssistantStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot modify Assistant with UUID <{assistantData.Uuid}>. Assistant was deleted!", []);
                genericError.AddData("AssistantUuid", assistantData.Uuid!.Value);
                genericError.AddData("Status", AssistantStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_WAS_DELETED);
            }

            if (assistant.Username != assistantData.Username)
            {
                bool newUsernameIsRegistered = await assistantMgr.IsUsernameRegisteredAsync(assistant.Username!);
                if (newUsernameIsRegistered)
                {
                    GenericError genericError = new GenericError($"Username <{assistant.Username}> is already registered", []);
                    genericError.AddData("username", assistant.Username!);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.USERNAME_ALREADY_REGISTERED);
                }
            }

            if (assistant.Email != assistantData.Email)
            {
                bool newEmailRegistered = await assistantMgr.IsEmailRegisteredAsync(assistant.Email!);
                if (newEmailRegistered)
                {
                    GenericError genericError = new GenericError($"Email <{assistant.Email}> is already registered", []);
                    genericError.AddData("email", assistant.Email!);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.EMAIL_ALREADY_REGISTERED);
                }
            }

            bool isUpdated = await assistantMgr.UpdateAssistant(assistant);
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

            if (clientData.Status == ClientStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot modify Client with UUID <{clientData.Uuid}>. Client was deleted!", []);
                genericError.AddData("clientUuid", clientData.Uuid!.Value);
                genericError.AddData("Status", ClientStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_WAS_DELETED);
            }

            if (client.Username != clientData.Username)
            {
                bool newUsernameIsRegistered = await clientMgr.IsUsernameRegisteredAsync(client.Username!);
                if (newUsernameIsRegistered)
                {
                    GenericError genericError = new GenericError($"Username <{client.Username}> is already registered", []);
                    genericError.AddData("username", client.Username!);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.USERNAME_ALREADY_REGISTERED);
                }
            }

            if (client.Email != clientData.Email)
            {
                bool newEmailRegistered = await clientMgr.IsEmailRegisteredAsync(client.Email!);
                if (newEmailRegistered)
                {
                    GenericError genericError = new GenericError($"Email <{client.Email}> is already registered", []);
                    genericError.AddData("email", client.Email!);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.EMAIL_ALREADY_REGISTERED);
                }
            }

            bool isUpdated = await clientMgr.UpdateClient(client);
            if (!isUpdated)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(isUpdated);
        }

        public async Task<OperationResult<bool, GenericError>> UpdateServiceAsync(Service service)
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
            bool isUpdated = await serviceMgr.UpdateService(service);
            if (!isUpdated)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has occurred"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<Guid, GenericError>> RegisterAssistant(Assistant assistant)
        {
            bool IsUsernameRegistered = await assistantMgr.IsUsernameRegisteredAsync(assistant.Username!);
            if (IsUsernameRegistered)
            {
                GenericError genericError = new GenericError($"Username <{assistant.Username}> is already registered", []);
                genericError.AddData("username", assistant.Username!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.USERNAME_ALREADY_REGISTERED);
            }

            bool IsEmailRegistered = await assistantMgr.IsEmailRegisteredAsync(assistant.Email!);
            if (IsEmailRegistered)
            {
                GenericError genericError = new GenericError($"Email <{assistant.Email}> is already registered", []);
                genericError.AddData("email", assistant.Email!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.EMAIL_ALREADY_REGISTERED);
            }

            bool IsPhoneNumberRegistered = await assistantMgr.IsPhoneNumberRegisteredAsync(assistant.PhoneNumber!);
            if (IsPhoneNumberRegistered)
            {
                GenericError genericError = new GenericError($"PhoneNumber <{assistant.PhoneNumber}> is already registered", []);
                genericError.AddData("phoneNumber", assistant.PhoneNumber!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.PHONE_NUMBER_ALREADY_REGISTERED);
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
            bool IsUsernameRegistered = await clientMgr.IsUsernameRegisteredAsync(client.Username!);
            if (IsUsernameRegistered)
            {
                GenericError genericError = new GenericError($"Username <{client.Username}> is already registered", []);
                genericError.AddData("username", client.Username!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.USERNAME_ALREADY_REGISTERED);
            }

            bool IsEmailRegistered = await clientMgr.IsEmailRegisteredAsync(client.Email!);
            if (IsEmailRegistered)
            {
                GenericError genericError = new GenericError($"Email <{client.Email}> is already registered", []);
                genericError.AddData("email", client.Email!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.EMAIL_ALREADY_REGISTERED);
            }

            bool IsPhoneNumberRegistered = await clientMgr.IsPhoneNumberRegisteredAsync(client.PhoneNumber!);
            if (IsPhoneNumberRegistered)
            {
                GenericError genericError = new GenericError($"PhoneNumber <{client.PhoneNumber}> is already registered", []);
                genericError.AddData("phoneNumber", client.PhoneNumber!);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.PHONE_NUMBER_ALREADY_REGISTERED);
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
            if (!(availabilityTimeSlot.StartTime < availabilityTimeSlot.EndTime || availabilityTimeSlot.StartTime == TimeOnly.MinValue))
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError("Range provided is not valid"), MessageCodeType.INVALID_RANGE_TIME);
            }

            // 1. Get account data
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(availabilityTimeSlot.Assistant!.Uuid!.Value);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Assistant UUID <{availabilityTimeSlot.Assistant!.Uuid!.Value}> is not registered", []);
                genericError.AddData("AssistantUuid", availabilityTimeSlot.Assistant!.Uuid!.Value);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (assistantData.Status != AssistantStatusType.ENABLED)
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
                Date = availabilityTimeSlot.Date!.Value,
                StartTime = availabilityTimeSlot.StartTime!.Value,
                EndTime = availabilityTimeSlot.EndTime!.Value
            };

            bool isAvailabilityTimeSlotAvailable = await schedulerMgr.IsAvailabilityTimeSlotAvailableAsync(range, assistantData.Id!.Value);
            if (!isAvailabilityTimeSlotAvailable)
            {
                GenericError genericError = new("Time range is not available", new Dictionary<string, object>());
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

        public async Task<OperationResult<bool, GenericError>> DisableServiceOfferAsync(Guid serviceOfferUuid)
        {
            ServiceOffer? serviceOffer = await schedulerMgr.GetServiceOfferByUuidAsync(serviceOfferUuid);
            if (serviceOffer == null)
            {
                GenericError genericError = new GenericError($"ServiceOffer with UUID: <{serviceOfferUuid}> is not found", []);
                genericError.AddData("ServiceUuid", serviceOfferUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_NOT_FOUND);
            }

            if (serviceOffer.Status == ServiceOfferStatusType.NOT_AVAILABLE)
            {
                GenericError genericError = new GenericError($"ServiceOffer with UUID: <{serviceOfferUuid}> is already disabled", []);
                genericError.AddData("ServiceUuid", serviceOfferUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_IS_ALREADY_UNAVAILABLE);
            }
            bool isStatusChanged = await schedulerMgr.ChangeServiceOfferStatusTypeAsync(serviceOffer.Id!.Value, ServiceOfferStatusType.NOT_AVAILABLE);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> EnableServiceOfferAsync(Guid serviceOfferUuid)
        {
            ServiceOffer? serviceOffer = await schedulerMgr.GetServiceOfferByUuidAsync(serviceOfferUuid);
            if (serviceOffer == null)
            {
                GenericError genericError = new GenericError($"ServiceOffer with UUID: <{serviceOfferUuid}> is not found", []);
                genericError.AddData("ServiceUuid", serviceOfferUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_NOT_FOUND);
            }

            if (serviceOffer.Status == ServiceOfferStatusType.AVAILABLE)
            {
                GenericError genericError = new GenericError($"ServiceOffer with UUID: <{serviceOfferUuid}> is already enabled", []);
                genericError.AddData("ServiceUuid", serviceOfferUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_IS_ALREADY_AVAILABLE);
            }
            bool isStatusChanged = await schedulerMgr.ChangeServiceOfferStatusTypeAsync(serviceOffer.Id!.Value, ServiceOfferStatusType.NOT_AVAILABLE);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }


        public async Task<OperationResult<bool, GenericError>> DisableAssistantAsync(Guid assistantUuid)
        {
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(assistantUuid);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Assistant with UUID: <{assistantUuid}> is not found", []);
                genericError.AddData("AssistantUuid", assistantUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (assistantData.Status == AssistantStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot change status of Assistant <{assistantUuid}>. Assistant was deleted!", []);
                genericError.AddData("AssistantUuid", assistantUuid);
                genericError.AddData("Status", AssistantStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_WAS_DELETED);
            }

            if (assistantData.Status == AssistantStatusType.DISABLED)
            {
                GenericError genericError = new GenericError($"Assistant with UUID: <{assistantUuid}> is already disabled", []);
                genericError.AddData("AssistantUuid", assistantUuid);
                genericError.AddData("Status", assistantData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_IS_ALREADY_DISABLED);
            }

            bool isStatusChanged = await assistantMgr.ChangeAssistantStatusAsync(assistantData.Id!.Value, AssistantStatusType.DISABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DisableClientAsync(Guid clientUuid)
        {
            Client? clientData = await clientMgr.GetClientByUuidAsync(clientUuid);
            if (clientData == null)
            {
                GenericError genericError = new GenericError($"Client with UUID: <{clientUuid}> is not found", []);
                genericError.AddData("clientUuid", clientUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }

            if (clientData.Status == ClientStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot change status of Client <{clientUuid}>. Client was deleted!", []);
                genericError.AddData("clientUuid", clientUuid);
                genericError.AddData("Status", ClientStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_WAS_DELETED);
            }

            if (clientData.Status == ClientStatusType.DISABLED)
            {
                GenericError genericError = new GenericError($"Client with UUID: <{clientUuid}> is already disabled", []);
                genericError.AddData("clientUuid", clientUuid);
                genericError.AddData("Status", clientData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_IS_ALREADY_DISABLED);
            }

            bool isStatusChanged = await clientMgr.ChangeClientStatusTypeAsync(clientData.Id!.Value, ClientStatusType.DISABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> EnableAssistantAsync(Guid assistantUuid)
        {
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(assistantUuid);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Assistant with UUID: <{assistantUuid}> is not found", []);
                genericError.AddData("AssistantUuid", assistantUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (assistantData.Status == AssistantStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot change status of Assistant <{assistantUuid}>. Assistant was deleted!", []);
                genericError.AddData("AssistantUuid", assistantUuid);
                genericError.AddData("Status", AssistantStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_WAS_DELETED);
            }

            if (assistantData.Status == AssistantStatusType.ENABLED)
            {
                GenericError genericError = new GenericError($"Assistant with UUID: <{assistantUuid}> is already enabled", []);
                genericError.AddData("AssistantUuid", assistantUuid);
                genericError.AddData("Status", assistantData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_IS_ALREADY_ENABLED);
            }

            bool isStatusChanged = await assistantMgr.ChangeAssistantStatusAsync(assistantData.Id!.Value, AssistantStatusType.ENABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> EnableClientAsync(Guid clientUuid)
        {
            Client? clientData = await clientMgr.GetClientByUuidAsync(clientUuid);
            if (clientData == null)
            {
                GenericError genericError = new GenericError($"Client with UUID: <{clientUuid}> is not found", []);
                genericError.AddData("clientUuid", clientUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }

            if (clientData.Status == ClientStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot change status of Client <{clientUuid}>. Client was deleted!", []);
                genericError.AddData("clientUuid", clientUuid);
                genericError.AddData("Status", ClientStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_WAS_DELETED);
            }

            if (clientData.Status == ClientStatusType.ENABLED)
            {
                GenericError genericError = new GenericError($"Client with UUID: <{clientUuid}> is already enabled", []);
                genericError.AddData("clientUuid", clientUuid);
                genericError.AddData("Status", clientData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_IS_ALREADY_ENABLED);
            }

            bool isStatusChanged = await clientMgr.ChangeClientStatusTypeAsync(clientData.Id!.Value, ClientStatusType.ENABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DeleteAssistantAsync(Guid assistantUuid)
        {
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(assistantUuid);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Assistant with UUID: <{assistantUuid}> is not found", []);
                genericError.AddData("AssistantUuid", assistantUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (assistantData.Status == AssistantStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Assistant with UUID: <{assistantUuid}> is already disabled", []);
                genericError.AddData("AssistantUuid", assistantUuid);
                genericError.AddData("Status", assistantData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_IS_ALREADY_DELETED);
            }

            bool isStatusChanged = await assistantMgr.ChangeAssistantStatusAsync(assistantData.Id!.Value, AssistantStatusType.DELETED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DeleteClientAsync(Guid clientUuid)
        {
            Client? clientData = await clientMgr.GetClientByUuidAsync(clientUuid);
            if (clientData == null)
            {
                GenericError genericError = new GenericError($"Client with UUID: <{clientUuid}> is not found", []);
                genericError.AddData("clientUuid", clientUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }

            if (clientData.Status == ClientStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Client with UUID: <{clientUuid}> is already deleted", []);
                genericError.AddData("clientUuid", clientUuid);
                genericError.AddData("Status", clientData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_IS_ALREADY_DELETED);
            }

            bool isStatusChanged = await clientMgr.ChangeClientStatusTypeAsync(clientData.Id!.Value, ClientStatusType.DELETED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> EnableServiceAsync(Guid ServiceUuid)
        {
            Service? serviceData = await serviceMgr.GetServiceByUuidAsync(ServiceUuid);
            if (serviceData == null)
            {
                GenericError genericError = new GenericError($"Service with UUID: <{ServiceUuid}> is not found", []);
                genericError.AddData("ServiceUuid", ServiceUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
            }

            if (serviceData.Status == ServiceStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot change status of Service with UUID <{ServiceUuid}>. Service was deleted!", []);
                genericError.AddData("ServiceUuid", ServiceUuid);
                genericError.AddData("Status", ServiceStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_WAS_DELETED);
            }

            if (serviceData.Status == ServiceStatusType.ENABLED)
            {
                GenericError genericError = new GenericError($"Service with UUID: <{ServiceUuid}> is already enabled", []);
                genericError.AddData("ServiceUuid", ServiceUuid);
                genericError.AddData("Status", serviceData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_IS_ALREADY_ENABLED);
            }

            bool isStatusChanged = await serviceMgr.ChangeServiceStatusType(serviceData.Id!.Value, ServiceStatusType.ENABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DisableServiceAsync(Guid ServiceUuid)
        {
            Service? serviceData = await serviceMgr.GetServiceByUuidAsync(ServiceUuid);
            if (serviceData == null)
            {
                GenericError genericError = new GenericError($"Service with UUID: <{ServiceUuid}> is not found", []);
                genericError.AddData("ServiceUuid", ServiceUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
            }

            if (serviceData.Status == ServiceStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot change status of Service with UUID <{ServiceUuid}>. Service was deleted!", []);
                genericError.AddData("ServiceUuid", ServiceUuid);
                genericError.AddData("Status", ServiceStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_WAS_DELETED);
            }

            if (serviceData.Status == ServiceStatusType.DISABLED)
            {
                GenericError genericError = new GenericError($"Service with UUID: <{ServiceUuid}> is already disabled", []);
                genericError.AddData("ServiceUuid", ServiceUuid);
                genericError.AddData("Status", serviceData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_IS_ALREADY_DISABLED);
            }

            bool isStatusChanged = await serviceMgr.ChangeServiceStatusType(serviceData.Id!.Value, ServiceStatusType.DISABLED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DeleteServiceAsync(Guid ServiceUuid)
        {
            Service? serviceData = await serviceMgr.GetServiceByUuidAsync(ServiceUuid);
            if (serviceData == null)
            {
                GenericError genericError = new GenericError($"Service with UUID: <{ServiceUuid}> is not found", []);
                genericError.AddData("ServiceUuid", ServiceUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
            }

            if (serviceData.Status == ServiceStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Service with UUID: <{ServiceUuid}> is already deleted", []);
                genericError.AddData("ServiceUuid", ServiceUuid);
                genericError.AddData("Status", serviceData.Status.Value.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_IS_ALREADY_DELETED);
            }

            bool isStatusChanged = await serviceMgr.ChangeServiceStatusType(serviceData.Id!.Value, ServiceStatusType.DELETED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> DeleteAvailabilityTimeSlotAsync(Guid uuid)
        {
            AvailabilityTimeSlot? slotData = await schedulerMgr.GetAvailabilityTimeSlotByUuidAsync(uuid);
            if (slotData == null)
            {
                GenericError genericError = new GenericError($"Availability time slot with UUID: <{uuid}> is not registered", []);
                genericError.AddData("AvailabilityTimeSlotUuid", uuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.AVAILABILITY_TIME_SLOT_NOT_FOUND);
            }

            if (slotData.Status == AvailabilityTimeSlotStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Availability time slot with UUID: <{uuid}> is already deleted", []);
                genericError.AddData("AvailabilityTimeSlotUuid", uuid);
                genericError.AddData("Status", slotData.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.AVAILABILITY_TIME_SLOT_IS_ALREADY_DELETED);
            }

            bool isStatusChanged = await schedulerMgr.ChangeAvailabilityStatusTypeAsync(slotData.Id!.Value, AvailabilityTimeSlotStatusType.DELETED);
            if (!isStatusChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);

        }

        public async Task<OperationResult<bool, GenericError>> AssignListServicesToAssistantAsync(Guid assistantUuid, List<Guid> servicesUuids)
        {

            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(assistantUuid);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Asssistant with UUID <{assistantUuid}> is not found", []);
                genericError.AddData("AssistantUuid", assistantUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            if (assistantData.Status == AssistantStatusType.DELETED)
            {
                GenericError genericError = new GenericError($"Cannot modify Assistant with UUID <{assistantData.Uuid}>. Assistant was deleted!", []);
                genericError.AddData("AssistantUuid", assistantData.Uuid!.Value);
                genericError.AddData("Status", AssistantStatusType.DELETED.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_WAS_DELETED);
            }


            List<int> idServices = [];
            foreach (var serviceUuid in servicesUuids)
            {
                Service? serviceData = await serviceMgr.GetServiceByUuidAsync(serviceUuid);
                if (serviceData == null)
                {
                    GenericError genericError = new GenericError($"Service with UUID <{serviceUuid}> is not found", []);
                    genericError.AddData("serviceUuid", servicesUuids);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
                }

                if (serviceData.Status != ServiceStatusType.ENABLED)
                {
                    GenericError genericError = new GenericError($"Cannot assign Service with UUID <{serviceData.Uuid}>. Service is unavailable", []);
                    genericError.AddData("ServiceUuid", serviceData.Uuid!.Value);
                    genericError.AddData("Status", serviceData.Status!.Value.ToString());
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_UNAVAILABLE);
                }


                bool isAlreadyRegistered = await assistantMgr.IsAssistantOfferingServiceByUuidAsync(serviceData.Id!.Value, assistantData.Id!.Value);
                if (isAlreadyRegistered)
                {
                    GenericError genericError = new GenericError($"Service with UUID <{serviceUuid}> is already assigned to assistant {assistantUuid}", []);
                    genericError.AddData("AssistantUuid", assistantUuid);
                    genericError.AddData("conflictingServiceUuid", serviceUuid);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_ALREADY_ASSIGNED_TO_ASSISTANT);
                }
                idServices.Add(serviceData.Id.Value);
            }
            bool areAllServicesAssigned = await assistantMgr.AssignListServicesToAssistantAsync(assistantData.Id!.Value, idServices);
            if (!areAllServicesAssigned)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has occurred"), MessageCodeType.REGISTER_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> FinalizeAppointmentAsync(Guid appointmentUuid)
        {
            Appointment? appointment = await schedulerMgr.GetAppointmentByUuidAsync(appointmentUuid);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {appointmentUuid} is not registered", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }

            if (appointment.Status == AppointmentStatusType.SCHEDULED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {appointmentUuid} must be confirmed before proceeding", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                genericError.AddData("AppointmentStatus", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NEEDS_TO_BE_CONFIRMED);
            }

            if (appointment.Status == AppointmentStatusType.CANCELED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {appointmentUuid} was cancelled before.", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                genericError.AddData("AppointmentStatus", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREDY_CANCELED);
            }

            if (appointment.Status == AppointmentStatusType.FINISHED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {appointmentUuid} is already finished", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                genericError.AddData("AppointmentStatus", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_FINISHED);
            }

            bool isStatusOfAppointmentChanged = await schedulerMgr.ChangeAppointmentStatusTypeAsync(appointment.Id!.Value, AppointmentStatusType.FINISHED);
            if (!isStatusOfAppointmentChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> ConfirmAppointment(Guid appointmentUuid)
        {
            Appointment? appointment = await schedulerMgr.GetAppointmentByUuidAsync(appointmentUuid);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {appointmentUuid} is not registered", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }

            if (appointment.Status == AppointmentStatusType.CANCELED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {appointmentUuid} was cancelled before.", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                genericError.AddData("AppointmentStatus", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREDY_CANCELED);
            }

            if (appointment.Status == AppointmentStatusType.FINISHED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {appointmentUuid} is finished.", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                genericError.AddData("AppointmentStatus", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_FINISHED);
            }


            if (appointment.Status == AppointmentStatusType.CONFIRMED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {appointmentUuid} is already confirmed", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                genericError.AddData("Status", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_CONFIRMED);
            }
            bool isStatusOfAppointmentChanged = await schedulerMgr.ChangeAppointmentStatusTypeAsync(appointment.Id!.Value, AppointmentStatusType.CONFIRMED);
            if (!isStatusOfAppointmentChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> CancelAppointmentClientSelf(Guid appointmentUuid, Guid ClientUuid)
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
                genericError.AddData("Status", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_FINISHED);
            }

            if (appointment.Status == AppointmentStatusType.CANCELED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{appointmentUuid}> is already canceled", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                genericError.AddData("Status", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREDY_CANCELED);
            }

            bool isStatusOfAppointmentChanged = await schedulerMgr.ChangeAppointmentStatusTypeAsync(appointment.Id!.Value, AppointmentStatusType.CANCELED);
            if (!isStatusOfAppointmentChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> CancelAppointmentStaffAssisted(Guid appointmentUuid)
        {
            Appointment? appointment = await schedulerMgr.GetAppointmentByUuidAsync(appointmentUuid);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{appointmentUuid}> is not registered", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }

            if (appointment.Status == AppointmentStatusType.FINISHED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{appointmentUuid}> is already finished", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                genericError.AddData("Status", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_FINISHED);
            }

            if (appointment.Status == AppointmentStatusType.CANCELED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{appointmentUuid}> is already canceled", []);
                genericError.AddData("AppointmentUuid", appointmentUuid);
                genericError.AddData("Status", appointment.Status.ToString());
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
            return await ScheduleAppointment(appointment);
        }

        public async Task<OperationResult<Guid, GenericError>> ScheduleAppointmentAsStaffAsync(Appointment appointment)
        {
            appointment.Status = AppointmentStatusType.CONFIRMED;
            return await ScheduleAppointment(appointment);
        }

        private async Task<OperationResult<Guid, GenericError>> ScheduleAppointment(Appointment appointment)
        {
            DateTime currentDateTime = DateTime.Now;
            int MAX_DAYS_FROM_NOW = int.Parse(envService.Get("MAX_DAYS_FOR_SCHEDULE"));
            int MAX_WEEKS_FOR_SCHEDULE = int.Parse(envService.Get("MAX_WEEKS_FOR_SCHEDULE"));
            int MAX_MONTHS_FOR_SCHEDULE = int.Parse(envService.Get("MAX_MONTHS_FOR_SCHEDULE"));
            bool IsPastSchedulingAllowed = bool.Parse(envService.Get("ALLOW_SCHEDULE_IN_THE_PAST"));
            int MAX_SERVICES_PER_CLIENT = int.Parse(envService.Get("MAX_SERVICES_PER_CLIENT"));
            int MAX_APPOINTMENTS_PER_CLIENT = int.Parse(envService.Get("MAX_APPOINTMENTS_PER_CLIENT"));

            DateTime maxDate = currentDateTime
                .AddMonths(MAX_MONTHS_FOR_SCHEDULE)
                .AddDays(MAX_WEEKS_FOR_SCHEDULE * 7)
                .AddDays(MAX_DAYS_FROM_NOW);
            DateTime startDateTime = appointment.Date!.Value.ToDateTime(appointment.StartTime!.Value);

            // Check max services per client
            if (appointment.ScheduledServices!.Count > MAX_SERVICES_PER_CLIENT)
            {
                GenericError genericError = new GenericError($"Too many services selected for the appointment. You can only select up to {MAX_SERVICES_PER_CLIENT} service(s) per appointment", []);
                genericError.AddData("MaxServicesPerAppointment", MAX_SERVICES_PER_CLIENT);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.TOO_MANY_SERVICES_SELECTED);
            }

            if (!IsPastSchedulingAllowed && startDateTime < currentDateTime)
            {
                GenericError genericError = new GenericError($"You cannot schedule an appoinment in the past. You can only schedule from the current time", []);
                genericError.AddData("SelectedDateTime", startDateTime.ToUniversalTime());
                genericError.AddData("SuggestedDateTime", currentDateTime.AddMinutes(1).ToUniversalTime());
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.CANNOT_SCHEDULE_APPOINTMENT_IN_THE_PAST);
            }

            if (startDateTime > maxDate)
            {
                GenericError genericError = new GenericError($"You cannot schedule an appoinment beyond {maxDate}.", []);
                genericError.AddData("SelectedDateTime", startDateTime.ToUniversalTime());
                genericError.AddData("SuggestedDateTime", currentDateTime.AddMinutes(5).ToUniversalTime());
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.CANNOT_SCHEDULE_APPOINTMENT_BEYOND_X);
            }


            // Get Client data
            var clientData = await clientMgr.GetClientByUuidAsync(appointment.Client!.Uuid!.Value);
            if (clientData == null)
            {
                GenericError genericError = new GenericError($"Client UUID: <{appointment.Client.Uuid.Value}> is not registered", []);
                genericError.AddData("clientUuid", appointment.Client.Uuid.Value);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }

            if (clientData.Status != ClientStatusType.ENABLED)
            {
                GenericError genericError = new GenericError($"Client with UUID <{clientData.Uuid}> is not available. Client was disabled or deleted!", []);
                genericError.AddData("clientUuid", clientData.Uuid!.Value);
                genericError.AddData("Status", clientData.Status!.Value.ToString());
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.CLIENT_UNAVAILABLE);
            }
            appointment.Client = clientData;

            // Check max allowed appoinments per client
            int scheduledAppoinmentsOfClientCount = await schedulerMgr.GetAppointmentsScheduledCountByClientId(clientData.Id!.Value);

            if (scheduledAppoinmentsOfClientCount >= MAX_APPOINTMENTS_PER_CLIENT)
            {
                timeRangeLockMgr.UnblockTimeRange(clientData.Uuid!.Value);
                GenericError genericError = new GenericError($"The client with UUID {clientData.Uuid} has reached the maximum allowed number of appoinments {MAX_APPOINTMENTS_PER_CLIENT}", []);
                genericError.AddData("MaxAppointmentsPerClient", MAX_APPOINTMENTS_PER_CLIENT);
                genericError.AddData("AppointmentsScheduledCount", scheduledAppoinmentsOfClientCount);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.MAX_SCHEDULED_APPOINTMENTS_REACHED);
            }

            // Get Services data
            for (int i = 0; i < appointment.ScheduledServices!.Count; i++)
            {
                var serviceOffer = appointment.ScheduledServices[i];
                var proposedStartTime = serviceOffer.ServiceStartTime;
                ServiceOffer? serviceOfferData = await assistantMgr.GetServiceOfferByUuidAsync(serviceOffer.Uuid!.Value);
                if (serviceOfferData == null)
                {
                    GenericError genericError = new GenericError($"Service <{serviceOffer.Uuid.Value}> is not registered", []);
                    genericError.AddData("SelectedServiceUuid", serviceOffer.Uuid.Value);
                    return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
                }

                if (serviceOfferData.Assistant!.Status != AssistantStatusType.ENABLED || serviceOfferData.Service!.Status != ServiceStatusType.ENABLED)
                {
                    GenericError genericError = new GenericError($"Service or assistant is deleted or disabled. ServiceOffer with UUID <{serviceOffer.Uuid.Value}> is unavailable", []);
                    genericError.AddData("SelectedServiceUuid", serviceOfferData.Uuid!.Value);
                    genericError.AddData("AssistantStatus", serviceOfferData.Assistant!.Status!.Value.ToString());
                    genericError.AddData("ServiceStatus", serviceOfferData.Service!.Status!.Value.ToString());
                    genericError.AddData("SelectedServiceStatus", serviceOfferData.Status!.Value.ToString());
                    return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_UNAVAILABLE);
                }

                if (serviceOfferData.Status == ServiceOfferStatusType.NOT_AVAILABLE)
                {
                    GenericError genericError = new GenericError($"ServiceOffer with UUID <{serviceOffer.Uuid.Value}> is unavailable", []);
                    genericError.AddData("SelectedServiceUuid", serviceOffer.Uuid.Value);
                    genericError.AddData("ServiceOfferStatus", serviceOfferData.Status);
                    return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.SERVICE_OFFER_UNAVAILABLE);
                }

                appointment.ScheduledServices[i].ServiceOffer = serviceOfferData;
                appointment.ScheduledServices[i].ServiceStartTime = proposedStartTime;
                appointment.ScheduledServices[i].ServiceEndTime = proposedStartTime!.Value.AddMinutes(serviceOfferData.Service!.Minutes!.Value);
                appointment.ScheduledServices[i].ServiceName = serviceOfferData.Service.Name;
                appointment.ScheduledServices[i].ServicesMinutes = serviceOfferData.Service.Minutes;
                appointment.ScheduledServices[i].ServicePrice = serviceOfferData.Service.Price;
            }

            appointment.ScheduledServices = appointment.ScheduledServices.OrderBy(so => so.ServiceStartTime).ToList();

            // Validate if the services are scheduled consecutively without gaps
            List<GenericError> errorMessages = [];
            for (int i = 1; i < appointment.ScheduledServices.Count; i++)
            {
                var prevService = appointment.ScheduledServices[i - 1];
                var currentService = appointment.ScheduledServices[i];

                if (currentService.ServiceStartTime != prevService.ServiceEndTime)
                {
                    GenericError genericError = new GenericError($"Service with UUID <{currentService!.Uuid!.Value}> is not contiguous with the previous service. <{prevService!.Uuid!.Value}>. Suggestions <ServiceOfferUuid>:<StartTime>:", []);
                    genericError.AddData($"{currentService.Uuid.Value}", prevService.ServiceEndTime!.Value);
                    errorMessages.Add(genericError);
                }
            }

            if (errorMessages.Any())
            {
                var result = OperationResult<Guid, GenericError>.Failure(errorMessages, MessageCodeType.SERVICES_ARE_NOT_CONTIGUOUS);
                return result;
            }

            // Calculate cost and endtime
            appointment.TotalCost = appointment.ScheduledServices.Sum(service => service.ServicePrice!.Value);
            appointment.EndTime = appointment.StartTime!.Value.AddMinutes(appointment.ScheduledServices.Sum(service => service.ServicesMinutes!.Value));

            DateTimeRange appointmentRange = new()
            {
                StartTime = appointment.StartTime.Value,
                EndTime = appointment.EndTime.Value,
                Date = appointment.Date!.Value,
            };

            OperationResult<DateTimeRange, GenericError> timeRangeBlockedByUser = timeRangeLockMgr.GetDateTimeRangeByAccountUuid(clientData.Uuid!.Value);
            if (!timeRangeBlockedByUser.IsSuccessful)
            {
                timeRangeBlockedByUser.Error!.AddData("TryBlockDateTimeRangeFirst", appointmentRange);
                timeRangeBlockedByUser.Error!.Message = timeRangeBlockedByUser.Error.Message + " Try to block the date time range first.";
                return OperationResult<Guid, GenericError>.Failure(timeRangeBlockedByUser.Error!, timeRangeBlockedByUser.Code);
            }

            if (!(appointmentRange.StartTime >= timeRangeBlockedByUser.Result!.StartTime && appointmentRange.EndTime <= timeRangeBlockedByUser.Result.EndTime) && !timeRangeBlockedByUser.Result!.Equals(appointmentRange))
            {


                bool hasConflictWithAnotherSlot = await schedulerMgr.IsAppointmentTimeSlotAvailableAsync(appointmentRange);
                if (!hasConflictWithAnotherSlot)
                {
                    GenericError genericError = new GenericError($"The new date time range selected has conflicts with another appoinments.", []);
                    genericError.AddData("BlockedTimeRange", timeRangeBlockedByUser.Result);
                    genericError.AddData("SelectedTimeRange", appointmentRange);
                    return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_SLOT_UNAVAILABLE);
                }

                OperationResult<bool, GenericError> extendTimeRange = timeRangeLockMgr.ExtendTimeRange(appointmentRange, clientData.Uuid!.Value);
                if (!extendTimeRange.IsSuccessful)
                {
                    GenericError error = new GenericError($"Cannot extend blocked time range, another blocked range overlaps. Remove some services", []);
                    error.AddData("BlockedTimeRange", timeRangeBlockedByUser.Result);
                    error.AddData("SelectedTimeRange", appointmentRange);
                    return OperationResult<Guid, GenericError>.Failure(error, MessageCodeType.CANNOT_EXTEND_BLOCKED_TIME_RANGE);
                }
            }

            // 1. Check for each service if it's Assistant is available in time range
            foreach (var scheduledService in appointment.ScheduledServices)
            {
                TimeOnly proposedStartTime = TimeOnly.Parse(scheduledService.ServiceStartTime!.Value.ToString());
                TimeOnly proposedEndTime = proposedStartTime.AddMinutes(scheduledService.ServicesMinutes!.Value);

                DateTimeRange serviceRange = new()
                {
                    StartTime = scheduledService.ServiceStartTime.Value,
                    EndTime = proposedEndTime,
                    Date = appointment.Date!.Value,
                };

                bool isAssistantAvailableInAvailabilityTimeSlots = await schedulerMgr.IsAssistantAvailableInAvailabilityTimeSlotsAsync(serviceRange, scheduledService.ServiceOffer!.Assistant!.Id!.Value);

                if (!isAssistantAvailableInAvailabilityTimeSlots)
                {
                    GenericError error = new GenericError($"Assistant: <{scheduledService.ServiceOffer.Assistant!.Uuid!.Value}> is not available during the requested time range", []);
                    error.AddData("SelectedServiceUuid", scheduledService.Uuid!.Value);
                    error.AddData("SelectedServiceStartTime", serviceRange.StartTime);
                    error.AddData("SelectedServiceEndTime", serviceRange.EndTime);
                    error.AddData("AssistantUuid", scheduledService.ServiceOffer.Assistant!.Uuid!.Value);
                    error.AddData("AssistantName", scheduledService.ServiceOffer.Assistant!.Name!);
                    return OperationResult<Guid, GenericError>.Failure(error, MessageCodeType.ASSISTANT_NOT_AVAILABLE_IN_TIME_RANGE);
                }

                bool hasAssistantConflictingAppoinments = await schedulerMgr.HasAssistantConflictingAppoinmentsAsync(serviceRange, scheduledService.ServiceOffer.Assistant!.Id!.Value);
                if (!hasAssistantConflictingAppoinments)
                {
                    GenericError error = new GenericError($"Assistant: <{scheduledService.ServiceOffer.Assistant!.Uuid!.Value}> is attending another appointment during the requested time range", []);
                    error.AddData("SelectedServiceUuid", scheduledService.Uuid!.Value);
                    error.AddData("SelectedServiceStartTime", serviceRange.StartTime);
                    error.AddData("SelectedServiceEndTime", serviceRange.EndTime);
                    error.AddData("AssistantUuid", scheduledService.ServiceOffer.Assistant!.Uuid!.Value);
                    error.AddData("AssistantName", scheduledService.ServiceOffer.Assistant!.Name!);
                    return OperationResult<Guid, GenericError>.Failure(error, MessageCodeType.SELECTED_SERVICE_HAS_CONFLICTING_APPOINTMENT_TIME_SLOT);
                }
            }

            Guid? UuidRegistered = await schedulerMgr.ScheduleAppointmentAsync(appointment);
            timeRangeLockMgr.UnblockTimeRange(clientData.Uuid!.Value);
            if (UuidRegistered == null)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.REGISTER_ERROR);
            }
            return OperationResult<Guid, GenericError>.Success(UuidRegistered.Value);
        }

    }
}