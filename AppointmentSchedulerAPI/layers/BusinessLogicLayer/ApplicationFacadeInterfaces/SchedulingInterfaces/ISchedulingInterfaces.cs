namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface ISchedulingInterfaces :
        IScheduleAppointmentStaffAssisted,
        IScheduleAppointmentClientSelf,
        IAssignAvailabilityTimeSlot,
        IEditAppointment,
        IEditAvailabilityTimeSlot,
        IFinalizeAppointment,
        ICancelAppointmentStaffAssisted,
        ICancelAppointmentClientSelf,
        IGetAvailabilityTimeSlot,
        IDisableServiceOffer,
        IDeleteServiceOffer,
        IEnableServiceOffer,
        IDeleteAvailabilityTimeSlot,
        IEnableAvailabilityTimeSlot,
        IDisableAvailabilityTimeSlot,
        IBlockDateTimeRange,
        IUnblockDateTimeRange,
        IGetSchedulingBlock,
        IGetAppointment
    {
    }
}
