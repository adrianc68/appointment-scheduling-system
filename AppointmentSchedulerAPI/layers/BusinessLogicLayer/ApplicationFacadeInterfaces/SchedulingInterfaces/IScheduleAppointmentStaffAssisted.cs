using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IScheduleAppointmentStaffAssisted
    {
        Appointment GetAppointmentDetails(int idAppointment);
        List<Appointment> GetAppointments(DateTime startDate, DateTime endDate);
        Task<List<AssistantService>> GetAvailableServices(DateOnly date);
        bool ScheduleAppointmentAsStaff(DateTimeRange range, List<Service> services, int idClient);
    }
}