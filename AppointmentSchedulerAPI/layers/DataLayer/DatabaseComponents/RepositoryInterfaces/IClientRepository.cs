using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IClientRepository
    {
        Task<Client?> GetClientByUuidAsync(Guid uuid);
        Task<int?> GetClientIdByUuidAsync(Guid uuid);
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<bool> AddClientAsync(Client client);
        Task<bool> UpdateClientAsync(Client assistant);
    }
}