using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class SchedulerRepository : ISchedulerRepository
    {
        // private readonly Model.AppointmentDbContext context;

        // public SchedulerRepository(Model.AppointmentDbContext context)
        // {
        //     this.context = context;
        // }

        public bool AreServicesAvailable(List<int> services, DateTimeRange range)
        {
            throw new NotImplementedException();
        }

        public bool BlockTimeRange(DateTimeRange range)
        {
            throw new NotImplementedException();
        }

        public bool ChangeAppointmentStatus(int idAppointment, AppointmentStatusType status)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAssistantAppointments(int idAssistant)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAssistantAvailabilityTimeSlots(int idAssistant)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAvailabilityTimeSlot(int idAvailabilityTimeSlot)
        {
            throw new NotImplementedException();
        }

        public bool EditAvailabilityTimeSlot(int idAvailabilityTimeSlot, AvailabilityTimeSlot newAvailabilityTimeSlot)
        {
            throw new NotImplementedException();
        }

        public bool FinalizeAppointment(int idAppointment)
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

        public List<int> GetAvailableServices(DateTimeRange range)
        {
            throw new NotImplementedException();
        }

        public bool IsAppointmentInSpecificState(int idAppointment, AppointmentStatusType expected)
        {
            throw new NotImplementedException();
        }

        public bool IsAssistantAvailableInTimeRange(int idAssistant, DateTimeRange range)
        {
            throw new NotImplementedException();
        }

        public bool IsAvailabilityTimeSlotAvailable(DateTimeRange range)
        {
            throw new NotImplementedException();
        }

        public bool RegisterAvailabilityTimeSlot(int idAssistant, DateTimeRange range)
        {
            throw new NotImplementedException();
        }

        public bool ScheduleAppointment(DateTimeRange range, List<Service> services, Client client)
        {
            throw new NotImplementedException();
        }
    }
}