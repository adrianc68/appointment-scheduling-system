using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IGetAppointment
    {
        Task<OperationResult<Appointment,GenericError>> GetAppointmentDetailsAsync(Guid appointmentUuid);
        Task<List<Appointment>> GetScheduledOrConfirmedAppoinmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<List<Appointment>> GetAllAppoinmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<List<ServiceOffer>> GetAvailableServicesClientAsync(DateOnly date);
        Task<List<ScheduledService>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range);
    }
}