using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces
{
    public interface IDeleteClient
    {
         Task<OperationResult<bool, GenericError>> DeleteClientAsync(Guid clientUuid);
    }
}