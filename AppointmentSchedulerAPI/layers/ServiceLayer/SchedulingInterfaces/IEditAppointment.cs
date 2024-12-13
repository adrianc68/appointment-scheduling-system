using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.ServiceLayerLayer.SchedulingInterfaces
{
    public interface IEditAppointment
    {
        bool EditAppointment(Appointment appointment);
    }
}