using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces
{
    public interface IEditService
    {
        Task<OperationResult<bool, GenericError>> UpdateService(Model.Service service);
    }
}