using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface ISchedulerMgt
    {
        // bool BlockTimeRange(DateTimeRange range);
        // bool DeleteAssistantAppointments(int idAssistant);
        // bool DeleteAvailabilityTimeSlot(int idAvailabilityTimeSlot);
        // bool DeleteAssistantAvailabilityTimeSlots(int idAssistant);
        // bool EditAvailabilityTimeSlot(int idAvailabilityTimeSlot, AvailabilityTimeSlot newAvailabilityTimeSlot);
        Task<List<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate);
        Task<List<ServiceOffer>> GetAvailableServicesAsync(DateOnly date);
        Task<List<ServiceOffer>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range);
        Task<Appointment?> GetAppointmentByUuidAsync(Guid uuid);
        Task<Appointment?> GetAppointmentDetailsByUuidAsync(Guid uuid);
        Task<List<Appointment>> GetScheduledOrConfirmedAppointmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<List<Appointment>> GetAllAppoinments(DateOnly startDate, DateOnly endDate);
        Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid);
        Task<Guid?> RegisterAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot);
        Task<Guid?> ScheduleAppointmentAsync(Appointment appointment);
        Task<bool> ChangeAppointmentStatusTypeAsync(int idAppointment, AppointmentStatusType status);
        Task<bool> ChangeServiceOfferStatusTypeAsync(int idServiceOffer, ServiceOfferStatusType status);
        Task<bool> IsAppointmentTimeSlotAvailableAsync(DateTimeRange range);
        Task<bool> IsAvailabilityTimeSlotAvailableAsync(DateTimeRange range, int idAssistant);
        Task<bool> IsAssistantAvailableInAvailabilityTimeSlotsAsync(DateTimeRange range, int idAssistant);
        Task<bool> HasAssistantConflictingAppoinmentsAsync(DateTimeRange range, int idAssistant);


    }
}