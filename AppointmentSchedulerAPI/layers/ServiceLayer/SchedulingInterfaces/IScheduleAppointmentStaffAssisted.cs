using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.SchedulingInterfaces
{
    public interface IScheduleAppointmentStaffAssisted
    {
        Appointment GetAppointmentDetails(int idAppointment);
        List<Appointment> GetAppointments(DateTime startDate, DateTime endDate);
        List<Service> GetAvailableServices(DateTimeRange range);
        bool ScheduleAppointmentAsStaff(DateTimeRange range, List<Service> services, int idClient);
    }
}