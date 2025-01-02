using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IClientMgt
    {
        Task<bool> UpdateClient(Client client);
        Task<bool> ChangeClientStatusTypeAsync(int idClient, ClientStatusType status);
        Task<Client?> GetClientByUuidAsync(Guid uuid);
        Task<bool> IsUsernameRegisteredAsync(string username);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
        Task<bool> IsClientRegisteredByUuidAsync(Guid uuid);
        Task<Guid?> RegisterClientAsync(Client client);
        Task<List<Client>> GetAllClientsAsync();
    }
}