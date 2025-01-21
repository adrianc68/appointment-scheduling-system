using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces
{
    public interface IDisableClient
    {
        Task<OperationResult<bool, GenericError>> DisableClientAsync(Guid clientUuid);
    }
}