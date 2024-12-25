using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class SchedulerMgr : ISchedulerMgt
    {
        private readonly ISchedulerRepository SchedulerRepository;

        public SchedulerMgr(ISchedulerRepository SchedulerRepository)
        {
            this.SchedulerRepository = SchedulerRepository;
        }

        // public bool AreServicesAvailable(List<int> services, DateTimeRange range)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool BlockTimeRange(DateTimeRange range)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool ChangeAppointmentStatus(int idAppointment, AppointmentStatusType status)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool DeleteAssistantAppointments(int idAssistant)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool DeleteAssistantAvailabilityTimeSlots(int idAssistant)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool DeleteAvailabilityTimeSlot(int idAvailabilityTimeSlot)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool EditAvailabilityTimeSlot(int idAvailabilityTimeSlot, AvailabilityTimeSlot newAvailabilityTimeSlot)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool FinalizeAppointment(int idAppointment)
        // {
        //     throw new NotImplementedException();
        // }

        // public Appointment GetAppointmentDetails(int idAppointment)
        // {
        //     throw new NotImplementedException();
        // }

        // public List<Appointment> GetAppointments(DateTime startDate, DateTime endDate)
        // {
        //     throw new NotImplementedException();
        // }

        // public List<int> GetAvailableServices(DateTimeRange range)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool IsAppointmentInSpecificState(int idAppointment, AppointmentStatusType expected)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool IsAssistantAvailableInTimeRange(int idAssistant, DateTimeRange range)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool IsAvailabilityTimeSlotAvailable(DateTimeRange range)
        // {
        //     throw new NotImplementedException();
        // }

        public async Task<Guid?> RegisterAvailabilityTimeSlot(AvailabilityTimeSlot availabilityTimeSlot, Guid assistantUuid)
        {
            availabilityTimeSlot.Uuid = Guid.CreateVersion7();
            bool isRegistered = await SchedulerRepository.RegisterAvailabilityTimeSlot(availabilityTimeSlot, assistantUuid);
            if (!isRegistered)
            {
                return null;
            }
            return availabilityTimeSlot.Uuid.Value;
        }

        // public bool ScheduleAppointment(DateTimeRange range, List<Service> services, Client client)
        // {
        //     throw new NotImplementedException();
        // }
    }
}