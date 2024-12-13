using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.SchedulingInterfaces
{
    public interface IFinalizeAppointment
    {
        bool FinalizeAppointment(int idAppointment);
    }
}