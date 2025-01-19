using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;



namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface ISchedulerRepository
    {
        Task<bool> DeleteAvailabilityTimeSlotAsync(int idAvailabilityTimeSlot);
        Task<bool> UpdateAvailabilityTimeSlotAsync(AvailabilityTimeSlot availabilityTimeSlot);
        Task<IEnumerable<ServiceOffer>> GetAvailableServicesAsync(DateOnly date);
        Task<Appointment?> GetAppointmentFullDetailsByUuidAsync(Guid uuid);
        Task<Appointment?> GetAppointmentByUuidAsync(Guid uuid);
        Task<int?> GetAppointmentIdByUuidAsync(Guid uuid);
        Task<IEnumerable<Appointment>> GetScheduledOrConfirmedAppoinmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Appointment>> GetAllAppoinmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<AvailabilityTimeSlot>> GetAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate);
        Task<bool> HasAvailabilityTimeSlotConflictingSlotsAsync(DateTimeRange date, int idAvailabilityTimeSlot, int idAssistant);
        Task<int?> GetAvailabilityTimeSlotIdByUuidAsync(Guid uuid);
        Task<AvailabilityTimeSlot?> GetAvailabilityTimeSlotByUuidAsync(Guid uuid);
        Task<AvailabilityTimeSlot?> GetAvailabilityTimeSlotByIdAsync(int idAvailabilityTimeSlot);
        Task<IEnumerable<ScheduledService>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range);
        Task<IEnumerable<DateTimeRange>> GetAppointmentDateTimeRangeConflictsByRangeAsync(DateTimeRange range);
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


        Task<List<int>> GetServiceOfferIdsByServiceIdAsync(int idService);
        Task<List<int>> GetServiceOfferIdsByAssistantIdAsync(int idAssistant);
        Task<List<ServiceOffer>> GetServiceOffersByAssistantIdAsync(int idAssistant);
        Task<List<ServiceOffer>> GetServiceOffersByServiceIdAsync(int idService);

 

        Task<int> GetAppointmentsScheduledCountByClientUuidAsync(int idClient);

        Task<List<Appointment>> GetScheduledOrConfirmedAppointmentsOfAsssistantByIdAsync(int idAssistant);
        Task<List<Appointment>> GetScheduledOrConfirmedAppointmentsOfAsssistantByIdAndRangeAsync(int idAssistant, DateTimeRange range);
        Task<List<Appointment>> GetScheduledOrConfirmedAppointmentsOfClientByIdAsync(int idClient);
        Task<List<int>> GetScheduledOrConfirmedAppoinmentsIdsOfClientByIdAsync(int idClient);


        Task<bool> UpdateAppointmentAsync(Appointment appointment);
    }
}