using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface ISchedulerMgt
    {
        Task<bool> UpdateAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot);
        Task<List<AvailabilityTimeSlot>> GetAllAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate);
        Task<AvailabilityTimeSlot?> GetAvailabilityTimeSlotByUuidAsync(Guid uuid);
        Task<List<ServiceOffer>> GetAvailableServicesAsync(DateOnly date);
        Task<List<ScheduledService>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range);
        Task<Appointment?> GetAppointmentByUuidAsync(Guid uuid);
        Task<int?> GetAppointmentIdByUuidAsync(Guid uuid);
        Task<Appointment?> GetAppointmentDetailsByUuidAsync(Guid uuid);
        Task<List<Appointment>> GetScheduledOrConfirmedAppointmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<List<Appointment>> GetAllAppoinmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid);
        Task<int> GetAppointmentsScheduledCountByClientId(int idClient);
        Task<List<DateTimeRange>> GetAppointmentDateTimeRangeConflictsByRangeAsync(DateTimeRange range);
        Task<Guid?> RegisterAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot);
        Task<Guid?> ScheduleAppointmentAsync(Appointment appointment);
        Task<bool> ChangeAppointmentStatusTypeAsync(int idAppointment, AppointmentStatusType status);
        Task<bool> ChangeServiceOfferStatusTypeAsync(int idServiceOffer, ServiceOfferStatusType status);
        Task<bool> ChangeAvailabilityStatusTypeAsync(int idAvailabilityTimeSlot, AvailabilityTimeSlotStatusType status);
        Task<bool> IsAppointmentTimeSlotAvailableAsync(DateTimeRange range);
        Task<bool> IsAvailabilityTimeSlotAvailableAsync(DateTimeRange range, int idAssistant);
        Task<bool> IsAssistantAvailableInAvailabilityTimeSlotsAsync(DateTimeRange range, int idAssistant);
        Task<bool> HasAssistantConflictingAppoinmentsAsync(DateTimeRange range, int idAssistant);
        Task<bool> HasAvailabilityTimeSlotConflictingSlotsAsync(DateTimeRange range, int idAvailabilityTimeSlot, int idAssistant);




    }
}