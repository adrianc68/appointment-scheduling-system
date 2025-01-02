using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer
{
    public class AppointmentSchedulingSystemFacade : ISchedulingInterfaces, IServiceInterfaces, IClientInterfaces, IAssistantInterfaces
    {
        private readonly IServiceMgt serviceMgr;
        private readonly ISchedulerMgt schedulerMgr;
        private readonly IAssistantMgt assistantMgr;
        private readonly IClientMgt clientMgr;

        public AppointmentSchedulingSystemFacade(IServiceMgt serviceMgr, IAssistantMgt assistantMgr, IClientMgt clientMgr, ISchedulerMgt schedulerMgr)
        {
            this.serviceMgr = serviceMgr;
            this.schedulerMgr = schedulerMgr;
            this.assistantMgr = assistantMgr;
            this.clientMgr = clientMgr;
        }


        public bool DeleteClient(int idClient)
        {
            throw new NotImplementedException();
        }

        public bool DeleteService(int idService)
        {
            return true;
        }

        public bool DisableAssistant(int dAssistant)
        {
            throw new NotImplementedException();
        }

        public bool DisableClient(int idClient)
        {
            throw new NotImplementedException();
        }

        public bool DisableService(int idService)
        {
            throw new NotImplementedException();
        }

        public bool EditAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public bool EditAssistant(Assistant assistant)
        {
            throw new NotImplementedException();
        }

        public bool EditClient(Client client)
        {
            throw new NotImplementedException();
        }

        public bool EditService(Service service)
        {
            throw new NotImplementedException();
        }

        public bool EnableAssistant(int idAssistant)
        {
            throw new NotImplementedException();
        }

        public bool EnableClient(int idClient)
        {
            throw new NotImplementedException();
        }

        public bool EnableService(int idService)
        {
            throw new NotImplementedException();
        }

        // public List<Appointment> GetAppoinments(DateTime startDate, DateTime endDate)
        // {
        //     throw new NotImplementedException();
        // }

        // public Appointment GetAppointmentDetails(int idAppointment)
        // {
        //     throw new NotImplementedException();
        // }

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

        public Task<List<AssistantService>> GetAvailableServicesClientAsync(DateOnly date)
        {
            return schedulerMgr.GetAvailableServicesAsync(date);
        }

        public Task<List<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlots(DateOnly startDate, DateOnly endDate)
        {
            return schedulerMgr.GetAllAvailabilityTimeSlotsAsync(startDate, endDate);
        }

        public async Task<List<ServiceOffer>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range)
        {
            return await schedulerMgr.GetConflictingServicesByDateTimeRangeAsync(range);
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
                GenericError genericError = new GenericError($"Assistant UUID <{availabilityTimeSlot.Assistant!.Uuid!.Value}>", []);
                genericError.AddData("AssistantUuid", availabilityTimeSlot.Assistant!.Uuid!.Value);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
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

        public async Task<OperationResult<Guid, GenericError>> RegisterService(Service service)
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

        public async Task<OperationResult<bool, GenericError>> AssignListServicesToAssistantAsync(Guid assistantUuid, List<Guid?> servicesUuid)
        {

            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(assistantUuid);
            if (assistantData == null)
            {
                GenericError genericError = new GenericError($"Asssistant with UUID <{assistantUuid}> is not found", []);
                genericError.AddData("assistantUuid", assistantUuid);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.ASSISTANT_NOT_FOUND);
            }

            List<int> idServices = [];
            foreach (var serviceUuid in servicesUuid)
            {
                Service? serviceData = await serviceMgr.GetServiceByUuidAsync(serviceUuid!.Value);
                if (serviceData == null)
                {
                    GenericError genericError = new GenericError($"Service with UUID <{serviceUuid}> is not found", []);
                    genericError.AddData("serviceUuid", servicesUuid);
                    return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
                }
                bool isAlreadyRegistered = await assistantMgr.IsAssistantOfferingServiceByUuidAsync(serviceData.Id!.Value, assistantData.Id!.Value);
                if (isAlreadyRegistered)
                {
                    GenericError genericError = new GenericError($"Service with UUID <{serviceUuid}> is already assigned to assistant {assistantUuid}", []);
                    genericError.AddData("assistantUuid", assistantUuid);
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

        public async Task<OperationResult<bool, GenericError>> FinalizeAppointment(Guid uuidAppointment)
        {
            Appointment? appointment = await schedulerMgr.GetAppointmentByUuidAsync(uuidAppointment);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuidAppointment} is not registered", []);
                genericError.AddData("AppointmentUuid", uuidAppointment);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }

            if (appointment.Status == AppointmentStatusType.SCHEDULED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuidAppointment} must be confirmed before proceeding", []);
                genericError.AddData("AppointmentUuid", uuidAppointment);
                genericError.AddData("Status", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NEEDS_TO_BE_CONFIRMED);
            }

            if (appointment.Status == AppointmentStatusType.FINISHED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuidAppointment} is already finished", []);
                genericError.AddData("AppointmentUuid", uuidAppointment);
                genericError.AddData("Status", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_FINISHED);
            }

            bool isStatusOfAppointmentChanged = await schedulerMgr.ChangeAppointmentStatusTypeAsync(appointment.Id!.Value, AppointmentStatusType.FINISHED);
            if (!isStatusOfAppointmentChanged)
            {
                return OperationResult<bool, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.UPDATE_ERROR);
            }
            return OperationResult<bool, GenericError>.Success(true);
        }

        public async Task<OperationResult<bool, GenericError>> ConfirmAppointment(Guid uuidAppointment)
        {
            Appointment? appointment = await schedulerMgr.GetAppointmentByUuidAsync(uuidAppointment);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuidAppointment} is not registered", []);
                genericError.AddData("AppointmentUuid", uuidAppointment);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }

            if (appointment.Status == AppointmentStatusType.CONFIRMED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID {uuidAppointment} is already confirmed", []);
                genericError.AddData("AppointmentUuid", uuidAppointment);
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

        public async Task<OperationResult<bool, GenericError>> CancelAppointmentClientSelf(Guid uuidAppointment, Guid UuidClient)
        {
            Client? clientData = await clientMgr.GetClientByUuidAsync(UuidClient);
            if (clientData == null)
            {
                GenericError genericError = new GenericError($"Client UUID <{UuidClient}> is not found");
                genericError.AddData("ClientUuid", uuidAppointment);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }

            Appointment? appointment = await schedulerMgr.GetAppointmentByUuidAsync(uuidAppointment);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{uuidAppointment}> is not registered", []);
                genericError.AddData("AppointmentUuid", uuidAppointment);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }

            if (appointment.Client.Id != clientData.Id)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{uuidAppointment}> belongs to another user");
                genericError.AddData("AppointmentUuid", uuidAppointment);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_BELONGS_TO_ANOTHER_USER);
            }

            if (appointment.Status == AppointmentStatusType.FINISHED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{uuidAppointment}> is already finished", []);
                genericError.AddData("AppointmentUuid", uuidAppointment);
                genericError.AddData("Status", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_FINISHED);
            }

            if (appointment.Status == AppointmentStatusType.CANCELED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{uuidAppointment}> is already canceled", []);
                genericError.AddData("AppointmentUuid", uuidAppointment);
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

        public async Task<OperationResult<bool, GenericError>> CancelAppointmentStaffAssisted(Guid uuidAppointment)
        {
            Appointment? appointment = await schedulerMgr.GetAppointmentByUuidAsync(uuidAppointment);
            if (appointment == null)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{uuidAppointment}> is not registered", []);
                genericError.AddData("AppointmentUuid", uuidAppointment);
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_NOT_FOUND);
            }

            if (appointment.Status == AppointmentStatusType.FINISHED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{uuidAppointment}> is already finished", []);
                genericError.AddData("AppointmentUuid", uuidAppointment);
                genericError.AddData("Status", appointment.Status.ToString());
                return OperationResult<bool, GenericError>.Failure(genericError, MessageCodeType.APPOINTMENT_IS_ALREADY_FINISHED);
            }

            if (appointment.Status == AppointmentStatusType.CANCELED)
            {
                GenericError genericError = new GenericError($"Appointment with UUID <{uuidAppointment}> is already canceled", []);
                genericError.AddData("AppointmentUuid", uuidAppointment);
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
            // Get Client data
            var clientData = await clientMgr.GetClientByUuidAsync(appointment.Client.Uuid!.Value);
            if (clientData == null)
            {
                GenericError genericError = new GenericError($"Client UUID: <{appointment.Client.Uuid.Value}> is not registered", []);
                genericError.AddData("ClientUuid", appointment.Client.Uuid.Value);
                return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.CLIENT_NOT_FOUND);
            }
            appointment.Client = clientData;
            // Get Services data
            for (int i = 0; i < appointment.ServiceOffers.Count; i++)
            {
                var serviceOffer = appointment.ServiceOffers[i];
                var proposedStartTime = serviceOffer.StartTime;
                ServiceOffer? serviceOfferData = await assistantMgr.GetServiceOfferByUuidAsync(serviceOffer.Uuid!.Value);
                if (serviceOfferData == null)
                {
                    GenericError genericError = new GenericError($"Service <{serviceOffer.Uuid.Value}> is not registered", []);
                    genericError.AddData("SelectedServiceUuid", serviceOffer.Uuid.Value);
                    return OperationResult<Guid, GenericError>.Failure(genericError, MessageCodeType.SERVICE_NOT_FOUND);
                }
                appointment.ServiceOffers[i] = serviceOfferData;
                appointment.ServiceOffers[i].StartTime = proposedStartTime;
                appointment.ServiceOffers[i].EndTime = proposedStartTime!.Value.AddMinutes(serviceOfferData.Service!.Minutes!.Value);
            }


            appointment.ServiceOffers = appointment.ServiceOffers.OrderBy(so => so.StartTime).ToList();

            // Validate if the services are scheduled consecutively without gaps
            List<GenericError> errorMessages = [];
            for (int i = 1; i < appointment.ServiceOffers.Count; i++)
            {
                var prevService = appointment.ServiceOffers[i - 1];
                var currentService = appointment.ServiceOffers[i];

                if (currentService.StartTime != prevService.EndTime)
                {
                    GenericError genericError = new GenericError($"Service with UUID <{currentService!.Uuid!.Value}> is not contiguous with the previous service. <{prevService!.Uuid!.Value}>. Suggestions <ServiceOfferUuid>:<StartTime>:", []);
                    genericError.AddData($"{currentService.Uuid.Value}", prevService.EndTime!.Value);
                    errorMessages.Add(genericError);
                }
            }

            if (errorMessages.Any())
            {
                var result = OperationResult<Guid, GenericError>.Failure(errorMessages, MessageCodeType.SERVICES_ARE_NOT_CONTIGUOUS);
                return result;
            }

            // Calculate cost and endtime
            appointment.TotalCost = appointment.ServiceOffers.Sum(service => service.Service!.Price!.Value);
            appointment.EndTime = appointment.StartTime!.Value.AddMinutes(appointment.ServiceOffers.Sum(service => service.Service!.Minutes!.Value));

            // 1. Check for each service if it's Assistant is available in time range
            foreach (var serviceOffer in appointment.ServiceOffers)
            {
                TimeOnly proposedStartTime = TimeOnly.Parse(serviceOffer.StartTime!.Value.ToString());
                TimeOnly proposedEndTime = proposedStartTime.AddMinutes(serviceOffer.Service!.Minutes!.Value);

                DateTimeRange serviceRange = new()
                {
                    StartTime = serviceOffer.StartTime.Value,
                    EndTime = proposedEndTime,
                    Date = appointment.Date!.Value
                };

                bool isAssistantAvailableInAvailabilityTimeSlots = await schedulerMgr.IsAssistantAvailableInAvailabilityTimeSlotsAsync(serviceRange, serviceOffer.Assistant!.Id!.Value);

                if (!isAssistantAvailableInAvailabilityTimeSlots)
                {
                    GenericError error = new GenericError($"Assistant: <{serviceOffer.Assistant!.Uuid!.Value}> is not available during the requested time range", []);
                    error.AddData("SelectedServiceUuid", serviceOffer.Uuid!.Value);
                    error.AddData("StartTime", serviceRange.StartTime);
                    error.AddData("EndTime", serviceRange.EndTime);
                    error.AddData("AssistantUuid", serviceOffer.Assistant!.Uuid!.Value);
                    return OperationResult<Guid, GenericError>.Failure(error, MessageCodeType.ASSISTANT_NOT_AVAILABLE_IN_TIME_RANGE);
                }

                bool hasAssistantConflictingAppoinments = await schedulerMgr.HasAssistantConflictingAppoinmentsAsync(serviceRange, serviceOffer.Assistant!.Id!.Value);
                if (!hasAssistantConflictingAppoinments)
                {
                    GenericError error = new GenericError($"Assistant: <{serviceOffer.Assistant!.Uuid!.Value}> is attending another appointment during the requested time range", []);
                    error.AddData("SelectedServiceUuid", serviceOffer.Uuid!.Value);
                    error.AddData("StartTime", serviceRange.StartTime);
                    error.AddData("EndTime", serviceRange.EndTime);
                    error.AddData("AssistantUuid", serviceOffer.Assistant!.Uuid!.Value);
                    return OperationResult<Guid, GenericError>.Failure(error, MessageCodeType.SELECTED_SERVICE_HAS_CONFLICTING_APPOINTMENT_TIME_SLOT);
                }
            }

            Guid? UuidRegistered = await schedulerMgr.ScheduleAppointmentAsync(appointment);
            if (UuidRegistered == null)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.REGISTER_ERROR);
            }
            return OperationResult<Guid, GenericError>.Success(UuidRegistered.Value);
        }

    }
}