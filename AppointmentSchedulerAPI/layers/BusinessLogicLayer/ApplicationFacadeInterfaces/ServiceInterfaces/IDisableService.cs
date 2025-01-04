using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces
{
    public interface IDisableService
    {
        Task<OperationResult<bool, GenericError>> DisableServiceAsync(Guid ServiceUuid);
    }
}