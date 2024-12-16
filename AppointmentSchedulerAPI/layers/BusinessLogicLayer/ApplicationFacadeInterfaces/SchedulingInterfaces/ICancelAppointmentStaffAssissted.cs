namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface ICancelAppointmentStaffAssisted
    {
        bool CancelAppointmentStaffAssisted(int idAppointment);
    }
}