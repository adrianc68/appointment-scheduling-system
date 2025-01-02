using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces
{
    public interface IEnableClient
    {
        Task<OperationResult<bool,GenericError>> EnableClientAsync(Guid uuidClient);
    }
}