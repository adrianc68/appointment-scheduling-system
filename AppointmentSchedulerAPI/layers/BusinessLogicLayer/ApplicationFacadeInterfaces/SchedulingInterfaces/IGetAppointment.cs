using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IGetAppointment
    {
        Task<OperationResult<Appointment,GenericError>> GetAppointmentDetailsAsync(Guid appointmentUuid);
        Task<List<Appointment>> GetScheduledOrConfirmedAppoinmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<List<Appointment>> GetAllAppoinmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<List<ServiceOffer>> GetAvailableServicesClientAsync(DateOnly date);

        Task<List<Appointment>> GetAppointmentsOfUserByUuidAndRange(DateOnly startDate, DateOnly endDate, Guid uuid);
        Task<List<Appointment>> GetAppointmentsOfUserByUuid(Guid uuid);
        Task<List<ScheduledService>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range);
    }
}