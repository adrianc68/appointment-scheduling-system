using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IGetAppointment
    {
        // Appointment GetAppointmentDetails(int idAppointment);
        // List<Appointment> GetAppoinments(DateTime startDate, DateTime endDate);
        // Appointment GetAppointmentDetails(int idAppointment);
        // List<Appointment> GetAppointments(DateTime startDate, DateTime endDate);
        // Task<List<AssistantService>> GetAvailableServices(DateOnly date);

        Task<List<AssistantService>> GetAvailableServicesClientAsync(DateOnly date);
        Task<List<ServiceOffer>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range);
    }
}