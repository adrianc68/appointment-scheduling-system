using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayerLayer.SchedulingInterfaces
{
    public interface ICancelAppointmentStaffAssisted
    {
        bool CancelAppointmentStaffAssisted(int idAppointment);
    }
}