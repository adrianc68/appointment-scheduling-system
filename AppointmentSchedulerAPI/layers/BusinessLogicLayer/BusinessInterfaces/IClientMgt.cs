using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IClientMgt
    {
        // bool ChangeClientStatusType(int idClient, ClientStatusType status);
        // Client GetClientDetails(int idClient);
        // ClientStatusType GetClientStatusType(int idClient);
        // bool IsClientAvailable(int idClient);
        Task<OperationResult<bool?>> IsAccountDataRegisteredAsync(Client client);
        Task<bool> IsClientRegisteredByUuidAsync(Guid uuid);
        Task<OperationResult<Guid?>> RegisterClientAsync(Client client);
        Task<List<Client>> GetAllClientsAsync();
    }
}