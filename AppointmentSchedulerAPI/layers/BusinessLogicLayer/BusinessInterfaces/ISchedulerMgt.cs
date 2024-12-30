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
        Task<IEnumerable<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlots(DateOnly startDate, DateOnly endDate);
        Task<List<AssistantService>> GetAvailableServicesAsync(DateOnly date);
        Task<Guid?> RegisterAvailabilityTimeSlot(AvailabilityTimeSlot availabilityTimeSlot);
        Task<Guid?> ScheduleAppointment(Appointment appointment);
        Task<bool> IsAppointmentTimeSlotAvailable(DateTimeRange range);
        Task<bool> IsAvailabilityTimeSlotAvailable(DateTimeRange range, int idAssistant);
        Task<bool> IsAssistantAvailableInTimeRange(DateTimeRange range, int idAssistant);
    }
}