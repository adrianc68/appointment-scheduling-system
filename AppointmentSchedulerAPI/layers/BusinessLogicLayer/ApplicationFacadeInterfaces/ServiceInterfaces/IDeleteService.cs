using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces
{
    public interface IDeleteService
    {
        Task<OperationResult<bool, GenericError>> DeleteServiceAsync(Guid uuid);
    }
}