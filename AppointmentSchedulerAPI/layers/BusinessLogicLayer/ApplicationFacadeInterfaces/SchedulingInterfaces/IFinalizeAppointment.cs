using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IFinalizeAppointment
    {
        Task<OperationResult<bool, GenericError>> FinalizeAppointmentAsync(Guid uuidAppointment);
    }
}