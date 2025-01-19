using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IClientRepository
    {
        Task<bool> AddClientAsync(Client client);
        Task<bool> UpdateClientAsync(Client assistant);
        Task<Client?> GetClientByUuidAsync(Guid uuid);
        Task<int?> GetClientIdByUuidAsync(Guid uuid);
        Task<IEnumerable<Client>> GetAllClientsAsync();
    }
}