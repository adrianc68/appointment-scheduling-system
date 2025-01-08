using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;



namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface ISchedulerRepository
    {
        Task<bool> DeleteAvailabilityTimeSlot(int idAvailabilityTimeSlot);
        Task<bool> UpdateAvailabilityTimeSlot(AvailabilityTimeSlot availabilityTimeSlot);
        Task<IEnumerable<ServiceOffer>> GetAvailableServicesAsync(DateOnly date);
        Task<Appointment?> GetAppointmentFullDetailsByUuidAsync(Guid uuid);
        Task<Appointment?> GetAppointmentByUuidAsync(Guid uuid);
        Task<int?> GetAppointmentIdByUuidAsync(Guid uuid);
        Task<IEnumerable<Appointment>> GetScheduledOrConfirmedAppoinmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Appointment>> GetAllAppoinments(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<AvailabilityTimeSlot>> GetAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate);
        Task<bool> HasAvailabilityTimeSlotConflictingSlotsAsync(DateTimeRange date, int idAvailabilityTimeSlot, int idAssistant);
        Task<int?> GetAvailabilityTimeSlotIdByUuidAsync(Guid uuid);
        Task<AvailabilityTimeSlot?> GetAvailabilityTimeSlotByUuidAsync(Guid uuid);
        Task<IEnumerable<ScheduledService>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range);
        Task<IEnumerable<DateTimeRange>> GetAppointmentDateTimeRangeConflictsByRange(DateTimeRange range);
        Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid);
        Task<bool> ChangeAppointmentStatusTypeAsync(int idApointment, AppointmentStatusType status);
        Task<bool> ChangeServiceOfferStatusTypeAsync(int idServiceOffer, ServiceOfferStatusType status);
        Task<bool> ChangeAvailabilityStatusTypeAsync(int idAvailabilityTimeSlot, AvailabilityTimeSlotStatusType status);
        Task<bool> IsAssistantAvailableInAvailabilityTimeSlotsAsync(DateTimeRange range, int idAssistant);
        Task<bool> HasAssistantConflictingAppoinmentsAsync(DateTimeRange range, int idAssistant);
        Task<bool> IsAppointmentTimeSlotAvailableAsync(DateTimeRange range);
        Task<bool> IsAvailabilityTimeSlotRegisteredAsync(DateTimeRange range, int idAssistant);
        Task<bool> AddAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot);
        Task<bool> AddAppointmentAsync(Appointment appointment);
        Task<int> GetAppointmentsScheduledCountByClientUuid(int idClient);
        Task<bool> CancelScheduledOrConfirmedAppointmentsOfClientById(int idClient);

    }
}