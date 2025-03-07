using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces
{
    public interface IRegisterService
    {
        Task<OperationResult<Guid, GenericError>> RegisterServiceAsync(Model.Service service);
    }
}