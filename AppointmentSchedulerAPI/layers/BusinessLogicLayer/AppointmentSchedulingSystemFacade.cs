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

        public List<Service> GetAvailableServices(DateTimeRange range)
        {
            throw new NotImplementedException();
        }

        public bool RegisterAssistant(Assistant assistant)
        {
            throw new NotImplementedException();
        }

        public bool RegisterAvailabilityTimeSlot(DateTimeRange range, int idAssistant)
        {
            throw new NotImplementedException();
        }

        public bool RegisterClient(Client client)
        {
            throw new NotImplementedException();
        }

        public bool RegisterService(Service service)
        {
            throw new NotImplementedException();
        }

        public bool ScheduleAppointmentAsClient(DateTimeRange range, List<Service> services, int idClient)
        {
            throw new NotImplementedException();
        }

        public bool ScheduleAppointmentAsStaff(DateTimeRange range, List<Service> services, int idClient)
        {
            throw new NotImplementedException();
        }
    }



}