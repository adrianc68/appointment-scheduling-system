using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface ICancelAppointmentClientSelf
    {
        Task<OperationResult<bool, GenericError>> CancelAppointmentClientSelf(Guid appointmentUuid, Guid clientUuid);
    }
}