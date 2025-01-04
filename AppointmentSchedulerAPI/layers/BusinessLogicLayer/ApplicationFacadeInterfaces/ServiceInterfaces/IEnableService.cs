using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces
{
    public interface IEnableService
    {
        Task<OperationResult<bool, GenericError>> EnableServiceAsync(Guid ServiceUuid);
    }
}