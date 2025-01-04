using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces
{
    public interface IEditService
    {
        Task<OperationResult<bool, GenericError>> UpdateServiceAsync(Model.Service service);
    }
}