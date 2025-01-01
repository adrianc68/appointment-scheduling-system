using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;



namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface ISchedulerRepository
    {
        // bool AreServicesAvailable(List<int> services, DateTimeRange range);
        // bool BlockTimeRange(DateTimeRange range);
        // bool ChangeAppointmentStatus(int idAppointment, AppointmentStatusType status);
        // bool DeleteAssistantAppointments(int idAssistant);
        // bool DeleteAvailabilityTimeSlot(int idAvailabilityTimeSlot);
        // bool DeleteAssistantAvailabilityTimeSlots(int idAssistant);
        // bool EditAvailabilityTimeSlot(int idAvailabilityTimeSlot, AvailabilityTimeSlot newAvailabilityTimeSlot);
        // bool FinalizeAppointment(int idAppointment);
        // List<Appointment> GetAppointments(DateTime startDate, DateTime endDate);
        // Appointment GetAppointmentDetails(int idAppointment);
        // List<int> GetAvailableServices(DateTimeRange range);
        // bool IsAppointmentInSpecificState(int idAppointment, AppointmentStatusType expected);
        // bool IsAvailabilityTimeSlotAvailable(DateTimeRange range);
        Task<IEnumerable<AssistantService>> GetAvailableServicesAsync(DateOnly date);
        Task<IEnumerable<AvailabilityTimeSlot>> GetAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Appointment>> GetAppointmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<int?> GetAvailabilityTimeSlotIdByUuidAsync(Guid uuid);
        Task<int?> GetAppointmentIdByUuidAsync(Guid uuid);
        Task<bool> IsAssistantAvailableInAvailabilityTimeSlotsAsync(DateTimeRange range, int idAssistant);
        Task<bool> HasAssistantConflictingAppoinmentsAsync(DateTimeRange range, int idAssistant);
        Task<bool> IsAppointmentTimeSlotAvailableAsync(DateTimeRange range);
        Task<bool> IsAvailabilityTimeSlotRegisteredAsync(DateTimeRange range, int idAssistant);
        Task<bool> AddAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot);
        Task<bool> AddAppointmentAsync(Appointment appointment);

    }
}