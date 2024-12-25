using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;


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

        public Task<Guid?> RegisterAssistant(Assistant assistant)
        {
            return assistantMgr.RegisterAssistantAsync(assistant);
        }

        public Task<List<Assistant>> GetAllAssistantsAsync()
        {
            return assistantMgr.GetAllAssistantsAsync();
        }

        public Task<Guid?>  RegisterAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot, Guid assistantUuid)
        {
            return schedulerMgr.RegisterAvailabilityTimeSlot(availabilityTimeSlot, assistantUuid);
        }

        public Task<Guid?> RegisterClientAsync(Client client)
        {
            return clientMgr.RegisterClientAsync(client);
        }

        public Task<Guid?> RegisterService(Service service)
        {
            return serviceMgr.RegisterService(service);
        }

        public Task<List<Service>> GetAllServicesAsync()
        {
            return serviceMgr.GetAllServicesAsync();
        }

        public bool ScheduleAppointmentAsClient(DateTimeRange range, List<Service> services, int idClient)
        {
            throw new NotImplementedException();
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
    }



}