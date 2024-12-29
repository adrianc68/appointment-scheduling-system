using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;


namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IClientRepository
    {
        // bool ChangeClientStatusType(int idClient, ClientStatusType status);
        // Client GetClientDetails(int idClient);
        // ClientStatusType GetClientStatusType(int idClient);
        // bool IsClientAvailable(int idClient);
        Task<Client?> GetClientByUuidAsync(Guid uuid);
        Task<int?> GetClientIdByUuidAsync(Guid uuid);
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<bool> AddClientAsync(Client client);
        Task<bool> IsUsernameRegisteredAsync(string username);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
    }
}