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

        public async Task<OperationResult<Guid?>> RegisterAssistant(Assistant assistant)
        {
            OperationResult<bool?> isAccountRegistered = await assistantMgr.IsAccountDataRegisteredAsync(assistant);
            if (isAccountRegistered.Result.HasValue && isAccountRegistered.Result.Value)
            {
                return new OperationResult<Guid?>(false, isAccountRegistered.Code);
            }
            Guid? uuidNewAssistant = await assistantMgr.RegisterAssistantAsync(assistant);
            if (uuidNewAssistant == null)
            {
                return new OperationResult<Guid?>(false, MessageCodeType.REGISTER_ERROR);
            }
            return new OperationResult<Guid?>(true, MessageCodeType.OK, uuidNewAssistant);
        }

        public Task<List<Assistant>> GetAllAssistantsAsync()
        {
            return assistantMgr.GetAllAssistantsAsync();
        }

        public async Task<OperationResult<Guid?>> RegisterAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot)
        {
            if (availabilityTimeSlot.Assistant!.Uuid == null)
            {
                return new OperationResult<Guid?>(false, MessageCodeType.NULL_VALUE_IS_PRESENT);
            }

            if (!(availabilityTimeSlot.StartTime < availabilityTimeSlot.EndTime || availabilityTimeSlot.StartTime == TimeOnly.MinValue))
            {
                return new OperationResult<Guid?>(false, MessageCodeType.INVALID_RANGE_TIME);
            }

            // 0. Check valid range time

            if (!(availabilityTimeSlot.StartTime < availabilityTimeSlot.EndTime || availabilityTimeSlot.StartTime == TimeOnly.MinValue))
            {
                return new OperationResult<Guid?>(false, MessageCodeType.INVALID_RANGE_TIME);
            }

            // 1. Get account data
            Assistant? assistantData = await assistantMgr.GetAssistantByUuidAsync(availabilityTimeSlot.Assistant.Uuid.Value);
            if (assistantData == null)
            {
                return new OperationResult<Guid?>(false, MessageCodeType.ASSISTANT_NOT_FOUND);
            }
            availabilityTimeSlot.Assistant = assistantData;
            // 2. Check if there is no slot already registered
            DateTimeRange range = new()
            {
                Date = availabilityTimeSlot.Date!.Value,
                StartTime = availabilityTimeSlot.StartTime!.Value,
                EndTime = availabilityTimeSlot.EndTime!.Value
            };

            bool isAvailabilityTimeSlotAvailable = await schedulerMgr.IsAvailabilityTimeSlotAvailable(range, assistantData.Id!.Value);
            if (!isAvailabilityTimeSlotAvailable)
            {
                return new OperationResult<Guid?>(false, MessageCodeType.AVAILABILITY_TIME_SLOT_NOT_AVAILABLE);
            }
            // 3. Register availability time slot
            Guid? uuid = await schedulerMgr.RegisterAvailabilityTimeSlot(availabilityTimeSlot);
            if (uuid == null)
            {
                return new OperationResult<Guid?>(false, MessageCodeType.REGISTER_ERROR);
            }
            return new OperationResult<Guid?>(true, MessageCodeType.OK, uuid);
        }

        public async Task<OperationResult<Guid?>> RegisterClientAsync(Client client)
        {
            OperationResult<bool?> isAccountRegistered = await clientMgr.IsAccountDataRegisteredAsync(client);
            if (isAccountRegistered.Result.HasValue && isAccountRegistered.Result.Value)
            {
                return new OperationResult<Guid?>(false, isAccountRegistered.Code);
            }
            Guid? uuidNewClient = await clientMgr.RegisterClientAsync(client);
            if (uuidNewClient == null)
            {
                return new OperationResult<Guid?>(false, MessageCodeType.REGISTER_ERROR);
            }
            return new OperationResult<Guid?>(true, MessageCodeType.OK, uuidNewClient);
        }

        public async Task<OperationResult<Guid?>> RegisterService(Service service)
        {
            OperationResult<bool?> isServiceRegistered = await serviceMgr.IsServiceDataRegisteredAsync(service);
            if (isServiceRegistered.Result.HasValue && isServiceRegistered.Result.Value)
            {
                return new OperationResult<Guid?>(false, isServiceRegistered.Code);
            }
            Guid? UuidNewservice = await serviceMgr.RegisterService(service);
            if (UuidNewservice == null)
            {
                return new OperationResult<Guid?>(false, MessageCodeType.REGISTER_ERROR);
            }
            return new OperationResult<Guid?>(true, MessageCodeType.OK, UuidNewservice);
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
            return assistantMgr.AssignServicesToAssistant(assistantUuid, servicesUuid);
        }

        public Task<List<AssistantService>> GetAvailableServices(DateOnly date)
        {
            throw new NotImplementedException();
        }

        public Task<List<AssistantService>> GetAvailableServicesClientAsync(DateOnly date)
        {
            return schedulerMgr.GetAvailableServicesAsync(date);
        }

        public Task<IEnumerable<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlots(DateOnly startDate, DateOnly endDate)
        {
            return schedulerMgr.GetAllAvailabilityTimeSlots(startDate, endDate);
        }

        public async Task<OperationResult<Guid?>> ScheduleAppointmentAsClientAsync(Appointment appointment)
        {
            if (appointment.Client.Uuid == null)
            {
                return new OperationResult<Guid?>(false, MessageCodeType.NULL_VALUE_IS_PRESENT);

            }
            // 0. Get Client data
            var clientData = await clientMgr.GetClientByUuidAsync(appointment.Client.Uuid.Value);
            if (clientData == null)
            {
                return new OperationResult<Guid?>(false, MessageCodeType.CLIENT_NOT_FOUND);
            }
            appointment.Client = clientData;

            // 0.1. Get Services data
            for (int i = 0; i < appointment.ServiceOffers.Count; i++)
            {
                var serviceOffer = appointment.ServiceOffers[i];
                if (serviceOffer.Uuid == null)
                {
                    return new OperationResult<Guid?>(false, MessageCodeType.NULL_VALUE_IS_PRESENT);
                }

                ServiceOffer? serviceOfferData = await assistantMgr.GetServiceOfferByUuidAsync(serviceOffer.Uuid.Value);
                if (serviceOfferData == null)
                {
                    return new OperationResult<Guid?>(false, MessageCodeType.SERVICE_NOT_FOUND, serviceOffer.Uuid.Value);
                }
                appointment.ServiceOffers[i] = serviceOfferData;
            }

            // 0.2. Calculate cost and endtime
            appointment.TotalCost = appointment.ServiceOffers.Sum(service => service.Service!.Price!.Value);
            appointment.EndTime = appointment.StartTime!.Value.AddMinutes(appointment.ServiceOffers.Sum(service => service.Service!.Minutes!.Value));
            appointment.Status = AppointmentStatusType.SCHEDULED;


            List<(TimeOnly StartTime, TimeOnly EndTime)> adjustedTimeRanges = new();
            TimeOnly currentAdjustedStartTime = appointment.StartTime!.Value;
            foreach (var serviceOffer in appointment.ServiceOffers)
            {
                var serviceDuration = TimeSpan.FromMinutes(serviceOffer.Service!.Minutes!.Value);
                TimeOnly proposedStartTime = currentAdjustedStartTime;
                TimeOnly proposedEndTime = proposedStartTime.Add(serviceDuration);
                while (true)
                {
                    int idAssistant = serviceOffer.Assistant!.Id!.Value;
                    bool isAssistantAvailable = await schedulerMgr.IsAssistantAvailableInTimeRange(
                        new DateTimeRange
                        {
                            Date = appointment.Date!.Value,
                            StartTime = proposedStartTime,
                            EndTime = proposedEndTime
                        },
                        idAssistant
                    );
                    if (isAssistantAvailable)
                    {
                        adjustedTimeRanges.Add((proposedStartTime, proposedEndTime));
                        currentAdjustedStartTime = proposedEndTime;
                        break;
                    }
                    else
                    {
                        proposedStartTime = proposedStartTime.AddMinutes(60);
                        if (proposedStartTime > TimeOnly.MaxValue)
                        {
                            return new OperationResult<Guid?>(false, MessageCodeType.NO_AVAILABLE_TIME_SLOT, serviceOffer.Uuid);
                        }
                        proposedEndTime = proposedStartTime.Add(serviceDuration);
                    }
                }
            }









            // 1.1. Check if availability time slot available
            DateTimeRange range = new()
            {
                Date = appointment.Date!.Value,
                StartTime = appointment.StartTime.Value,
                EndTime = appointment.EndTime.Value

            };


            var tasks = appointment.ServiceOffers.Select(async serviceOffer =>
            {
                int idAssistant = serviceOffer.Assistant!.Id!.Value;
                bool isAssistantAvailableInTimeRange = await schedulerMgr.IsAssistantAvailableInTimeRange(range, idAssistant);
                if (!isAssistantAvailableInTimeRange)
                {
                    return new OperationResult<Guid?>(false, MessageCodeType.SERVICE_NOT_PROVIDED_BY_ASSISTANT_IN_TIME_RANGE, serviceOffer.Uuid);
                }
                return null;
            }).ToList();

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                if (result != null)
                {
                    return result;
                }
            }

            Guid? UuidRegistered = await schedulerMgr.ScheduleAppointment(appointment);
            if (UuidRegistered == null)
            {
                return new OperationResult<Guid?>(false, MessageCodeType.REGISTER_ERROR);
            }
            return new OperationResult<Guid?>(true, MessageCodeType.OK, UuidRegistered);
        }
    }



}