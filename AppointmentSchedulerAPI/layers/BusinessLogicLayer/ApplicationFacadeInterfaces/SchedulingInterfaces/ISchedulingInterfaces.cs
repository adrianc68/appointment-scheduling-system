namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface ISchedulingInterfaces :
        IScheduleAppointmentStaffAssisted,
        IScheduleAppointmentClientSelf,
        IAssignAvailabilityTimeSlot,
        IEditAppointment,
        IFinalizeAppointment,
        ICancelAppointmentStaffAssisted,
        ICancelAppointmentClientSelf
    {
    }
}
