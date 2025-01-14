using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IClientMgt
    {
        Task<bool> UpdateClient(Client client);
        Task<Client?> GetClientByUuidAsync(Guid uuid);
        Task<bool> IsClientRegisteredByUuidAsync(Guid uuid);
        Task<Guid?> RegisterClientAsync(Client client);
        Task<List<Client>> GetAllClientsAsync();
    }
}