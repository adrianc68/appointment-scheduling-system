using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IScheduleAppointmentClientSelf
    {
        bool ConfirmAppointment(int idAppointment);
        Appointment GetAppointmentDetails(int idAppointment);
        List<Appointment> GetAppoinments(DateTime startDate, DateTime endDate);
        List<Service> GetAvailableServices(DateTimeRange range);
        bool ScheduleAppointmentAsClient(DateTimeRange range, List<Service> services, int idClient);
    }
}