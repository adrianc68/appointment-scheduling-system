using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IGetAppointment
    {
        Task<OperationResult<Appointment,GenericError>> GetAppointmentDetailsAsync(Guid uuidAppointment);
        Task<List<Appointment>> GetScheduledOrConfirmedAppoinmentsAsync(DateOnly startDate, DateOnly endDate);
        Task<List<AssistantService>> GetAvailableServicesClientAsync(DateOnly date);
        Task<List<ServiceOffer>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range);
    }
}