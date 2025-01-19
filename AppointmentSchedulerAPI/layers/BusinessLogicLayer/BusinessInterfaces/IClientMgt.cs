using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IClientMgt
    {
        Task<Guid?> RegisterClientAsync(Client client);
        Task<bool> UpdateClientAsync(Client client);
        Task<Client?> GetClientByUuidAsync(Guid uuid);
        Task<List<Client>> GetAllClientsAsync();
        Task<bool> IsClientRegisteredByUuidAsync(Guid uuid);
    }
}