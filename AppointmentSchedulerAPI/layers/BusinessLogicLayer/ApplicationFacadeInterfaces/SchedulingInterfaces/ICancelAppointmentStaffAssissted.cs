using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface ICancelAppointmentStaffAssisted
    {
        Task<OperationResult<bool,GenericError>> CancelAppointmentStaffAssisted(Guid appointmentUuid);
    }
}