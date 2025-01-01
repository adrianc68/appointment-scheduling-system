using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;


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

        public bool CancelAppointmentClientSelf(int idAppointment)
        {
            throw new NotImplementedException();
        }

        public bool CancelAppointmentStaffAssisted(int idAppointment)
        {
            throw new NotImplementedException();
        }

        public bool ConfirmAppointment(int idAppointment)
        {
            throw new NotImplementedException();
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

        public bool FinalizeAppointment(int idAppointment)
        {
            throw new NotImplementedException();
        }

        public List<Appointment> GetAppoinments(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Appointment GetAppointmentDetails(int idAppointment)
        {
            throw new NotImplementedException();
        }

        public List<Appointment> GetAppointments(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<Guid, GenericError>> RegisterAssistant(Assistant assistant)
        {
            bool IsUsernameRegistered = await assistantMgr.IsUsernameRegisteredAsync(assistant.Username!);
            if (IsUsernameRegistered)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError($"Username <{assistant.Username}> is already registered"), MessageCodeType.USERNAME_ALREADY_REGISTERED);
            }

            bool IsEmailRegistered = await assistantMgr.IsEmailRegisteredAsync(assistant.Email!);
            if (IsEmailRegistered)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError($"Email <{assistant.Email}> is already registered"), MessageCodeType.EMAIL_ALREADY_REGISTERED);
            }

            bool IsPhoneNumberRegistered = await assistantMgr.IsPhoneNumberRegisteredAsync(assistant.PhoneNumber!);
            if (IsPhoneNumberRegistered)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError($"PhoneNumber <{assistant.PhoneNumber}> is already registered"), MessageCodeType.PHONE_NUMBER_ALREADY_REGISTERED);
            }
            Guid? uuidNewAssistant = await assistantMgr.RegisterAssistantAsync(assistant);
            if (uuidNewAssistant == null)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.REGISTER_ERROR);
            }
            return OperationResult<Guid, GenericError>.Success(uuidNewAssistant.Value);
        }

        public Task<List<Assistant>> GetAllAssistantsAsync()
        {
            return assistantMgr.GetAllAssistantsAsync();
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
                return OperationResult<Guid, GenericError>.Failure(new GenericError($"Assistant UUID <{availabilityTimeSlot.Assistant!.Uuid!.Value}>"), MessageCodeType.ASSISTANT_NOT_FOUND);
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
                genericError.AdditionalData!.Add("range", range);
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

        public async Task<OperationResult<Guid, GenericError>> RegisterClientAsync(Client client)
        {
            bool IsUsernameRegistered = await clientMgr.IsUsernameRegisteredAsync(client.Username!);
            if (IsUsernameRegistered)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError($"Username <{client.Username}> is already registered"), MessageCodeType.USERNAME_ALREADY_REGISTERED);
            }

            bool IsEmailRegistered = await clientMgr.IsEmailRegisteredAsync(client.Email!);
            if (IsEmailRegistered)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError($"Email <{client.Email}> is already registered"), MessageCodeType.EMAIL_ALREADY_REGISTERED);
            }

            bool IsPhoneNumberRegistered = await clientMgr.IsPhoneNumberRegisteredAsync(client.PhoneNumber!);
            if (IsPhoneNumberRegistered)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError($"PhoneNumber <{client.PhoneNumber}> is already registered"), MessageCodeType.PHONE_NUMBER_ALREADY_REGISTERED);
            }
            Guid? uuidNewClient = await clientMgr.RegisterClientAsync(client);
            if (uuidNewClient == null)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError("An error has ocurred!"), MessageCodeType.REGISTER_ERROR);
            }
            return OperationResult<Guid, GenericError>.Success(uuidNewClient.Value);
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

        public Task<List<Service>> GetAllServicesAsync()
        {
            return serviceMgr.GetAllServicesAsync();
        }

        public bool ScheduleAppointmentAsStaff(DateTimeRange range, List<Service> services, int idClient)
        {
            throw new NotImplementedException();
        }

        public Task<List<Client>> GetAllClientsAsync()
        {
            return clientMgr.GetAllClientsAsync();
        }

        public Task<bool> AssignServicesToAssistant(Guid assistantUuid, List<Guid?> servicesUuid)
        {
            return assistantMgr.AssignServicesToAssistantAsync(assistantUuid, servicesUuid);
        }

        public Task<List<AssistantService>> GetAvailableServices(DateOnly date)
        {
            throw new NotImplementedException();
        }

        public Task<List<AssistantService>> GetAvailableServicesClientAsync(DateOnly date)
        {
            return schedulerMgr.GetAvailableServicesAsync(date);
        }

        public Task<List<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlots(DateOnly startDate, DateOnly endDate)
        {
            return schedulerMgr.GetAllAvailabilityTimeSlotsAsync(startDate, endDate);
        }

        public async Task<OperationResult<Guid, GenericError>> ScheduleAppointmentAsClientAsync(Appointment appointment)
        {
            // Get Client data
            var clientData = await clientMgr.GetClientByUuidAsync(appointment.Client.Uuid!.Value);
            if (clientData == null)
            {
                return OperationResult<Guid, GenericError>.Failure(new GenericError($"Client UUID: <{appointment.Client.Uuid.Value}> is not registered"), MessageCodeType.CLIENT_NOT_FOUND);
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
                    return OperationResult<Guid, GenericError>.Failure(new GenericError($"Service <{serviceOffer.Uuid.Value}> is not registered"), MessageCodeType.NULL_VALUE_IS_PRESENT);
                }
                appointment.ServiceOffers[i] = serviceOfferData;
                appointment.ServiceOffers[i].StartTime = proposedStartTime;
                appointment.ServiceOffers[i].EndTime = proposedStartTime!.Value.AddMinutes(serviceOfferData.Service!.Minutes!.Value);
            }

            // 0.2. Calculate cost and endtime
            appointment.TotalCost = appointment.ServiceOffers.Sum(service => service.Service!.Price!.Value);
            appointment.EndTime = appointment.StartTime!.Value.AddMinutes(appointment.ServiceOffers.Sum(service => service.Service!.Minutes!.Value));
            appointment.Status = AppointmentStatusType.SCHEDULED;


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
                    error.AddData("uuid", serviceOffer.Uuid!.Value);
                    error.AddData("startTime", serviceRange.StartTime);
                    error.AddData("endTime", serviceRange.EndTime);
                    return OperationResult<Guid, GenericError>.Failure(error, MessageCodeType.ASSISTANT_NOT_AVAILABLE_IN_TIME_RANGE);
                }

                bool hasAssistantConflictingAppoinments = await schedulerMgr.HasAssistantConflictingAppoinmentsAsync(serviceRange, serviceOffer.Assistant!.Id!.Value);
                if (!hasAssistantConflictingAppoinments)
                {
                    GenericError error = new GenericError($"Assistant: <{serviceOffer.Assistant!.Uuid!.Value}> is attending another appointment during the requested time range", []);
                    error.AddData("uuid", serviceOffer.Uuid!.Value);
                    error.AddData("startTime", serviceRange.StartTime);
                    error.AddData("endTime", serviceRange.EndTime);
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

        Task<IEnumerable<AvailabilityTimeSlot>> IGetAvailabilityTimeSlot.GetAllAvailabilityTimeSlots(DateOnly startDate, DateOnly endDate)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ServiceOffer>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range)
        {
            return await schedulerMgr.GetConflictingServicesByDateTimeRangeAsync(range);
        }
    }



}