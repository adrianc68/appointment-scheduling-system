using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IClientMgt
    {
        // bool ChangeClientStatusType(int idClient, ClientStatusType status);
        // Client GetClientDetails(int idClient);
        // ClientStatusType GetClientStatusType(int idClient);
        // bool IsClientAvailable(int idClient);
        Task<List<Client>> GetAllClientsAsync();
        Task<Guid?> RegisterClientAsync(Client client);
    }
}