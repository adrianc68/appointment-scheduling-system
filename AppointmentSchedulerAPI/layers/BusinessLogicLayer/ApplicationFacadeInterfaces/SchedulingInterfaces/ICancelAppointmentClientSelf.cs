using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface ICancelAppointmentClientSelf
    {
        Task<OperationResult<bool, GenericError>> CancelAppointmentClientSelfAsync(Guid appointmentUuid, Guid clientUuid);
    }
}