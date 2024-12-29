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

        public Task<Guid?> RegisterAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot, Guid assistantUuid)
        {
            return schedulerMgr.RegisterAvailabilityTimeSlot(availabilityTimeSlot, assistantUuid);
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

        public async Task<OperationResult<Guid>> ScheduleAppointmentAsClientAsync(Appointment appointment)
        {
            // 1. Get Services data
            var serviceTasks = appointment.AssistantService.Select(async service =>
            {
                if (service.Uuid == null)
                {
                    throw new InvalidOperationException("Service UUID cannot be null.");
                }

                int? result = await assistantMgr.GetServiceIdByAssistantServiceUuidAsync(service.Uuid.Value);
                if (result == null)
                {
                    throw new KeyNotFoundException("Service not found.");
                }

                return await serviceMgr.GetServiceByIdAsync(result.Value); // Retornar datos del servicio
            });

            // Ejecutar todas las tareas en paralelo
            var serviceResults = await Task.WhenAll(serviceTasks);

            // 2. Calculate cost and endtime
            double totalCost = serviceResults.Sum(service => service.Price.Value);
            int totalMinutes = serviceResults.Sum(service => service.Minutes.Value);

            appointment.TotalCost = totalCost;
            appointment.EndTime = appointment.StartTime.Value.AddMinutes(totalMinutes);


            // 3. Check if availability time slot available
            DateTimeRange range = new()
            {
                Date = appointment.Date.Value,
                StartTime = appointment.StartTime.Value,
                EndTime = appointment.EndTime.Value

            };
            bool isTimeSlotAvailable = await schedulerMgr.IsTimeSlotAvailable(range);
            if (!isTimeSlotAvailable)
            {
                return new OperationResult<Guid>(false, MessageCodeType.TIME_SLOT_NOT_AVAILABLE);
            }
            return await schedulerMgr.ScheduleAppointment(appointment);
        }
    }



}