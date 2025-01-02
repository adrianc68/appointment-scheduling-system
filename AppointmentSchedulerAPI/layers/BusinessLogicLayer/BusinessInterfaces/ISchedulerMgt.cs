using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;



namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface ISchedulerMgt
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
        // bool IsAssistantAvailableInTimeRange(int idAssistant, DateTimeRange range);
        // bool IsAvailabilityTimeSlotAvailable(DateTimeRange range);
        Task<List<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate);
        Task<List<AssistantService>> GetAvailableServicesAsync(DateOnly date);
        Task<List<ServiceOffer>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range);
        Task<Appointment?> GetAppointmentByUuidAsync(Guid uuid); 
        Task<Guid?> RegisterAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot);
        Task<Guid?> ScheduleAppointmentAsync(Appointment appointment);
        Task<bool> ChangeAppointmentStatusTypeAsync(int idAppointment, AppointmentStatusType status);
        Task<bool> IsAppointmentInSpecificStatusType(int idAppointment, AppointmentStatusType expectedStatus);
        Task<bool> IsAppointmentTimeSlotAvailableAsync(DateTimeRange range);
        Task<bool> IsAvailabilityTimeSlotAvailableAsync(DateTimeRange range, int idAssistant);
        Task<bool> IsAssistantAvailableInAvailabilityTimeSlotsAsync(DateTimeRange range, int idAssistant);
        Task<bool> HasAssistantConflictingAppoinmentsAsync(DateTimeRange range, int idAssistant);


    }
}