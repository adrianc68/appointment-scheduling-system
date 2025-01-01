using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces
{
    public interface IRegisterClient
    {
        Task<OperationResult<Guid,GenericError>> RegisterClientAsync(Client client);
    }
}