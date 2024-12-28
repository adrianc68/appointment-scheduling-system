namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IFinalizeAppointment
    {
        bool FinalizeAppointment(int idAppointment);
    }
}