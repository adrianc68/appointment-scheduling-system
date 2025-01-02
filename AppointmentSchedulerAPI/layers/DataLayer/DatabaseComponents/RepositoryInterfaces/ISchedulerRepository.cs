using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;



namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface ISchedulerRepository
    {
        // bool BlockTimeRange(DateTimeRange range);
        // bool DeleteAssistantAppointments(int idAssistant);
        // bool DeleteAvailabilityTimeSlot(int idAvailabilityTimeSlot);
        // bool DeleteAssistantAvailabilityTimeSlots(int idAssistant);
        // bool EditAvailabilityTimeSlot(int idAvailabilityTimeSlot, AvailabilityTimeSlot newAvailabilityTimeSlot);
        Task<IEnumerable<AssistantService>> GetAvailableServicesAsync(DateOnly date);
        Task<Appointment?> GetAppointmentFullDetailsByUuidAsync(Guid uuid);
        Task<Appointment?> GetAppointmentByUuidAsync(Guid uuid);
        Task<int?> GetAppointmentIdByUuidAsync(Guid uuid);
        Task<IEnumerable<Appointment>> GetScheduledOrConfirmedAppoinmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<AvailabilityTimeSlot>> GetAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate);
        Task<int?> GetAvailabilityTimeSlotIdByUuidAsync(Guid uuid);
        Task<IEnumerable<ServiceOffer>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range);
        Task<bool> ChangeAppointmentStatusTypeAsync(int idApointment, AppointmentStatusType status);
        Task<bool> IsAssistantAvailableInAvailabilityTimeSlotsAsync(DateTimeRange range, int idAssistant);
        Task<bool> HasAssistantConflictingAppoinmentsAsync(DateTimeRange range, int idAssistant);
        Task<bool> IsAppointmentTimeSlotAvailableAsync(DateTimeRange range);
        Task<bool> IsAvailabilityTimeSlotRegisteredAsync(DateTimeRange range, int idAssistant);
        Task<bool> AddAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot);
        Task<bool> AddAppointmentAsync(Appointment appointment);

    }
}