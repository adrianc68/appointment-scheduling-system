using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayerLayer.SchedulingInterfaces
{
    public interface ICancelAppointmentClientSelf
    {
        bool CancelAppointmentClientSelf(int idAppointment);
    }
}