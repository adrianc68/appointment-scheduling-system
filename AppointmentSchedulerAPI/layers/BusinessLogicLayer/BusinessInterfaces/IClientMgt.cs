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
        Task<Client?> GetClientByUuidAsync(Guid uuid);
        Task<bool> IsUsernameRegisteredAsync(string username);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
        Task<bool> IsClientRegisteredByUuidAsync(Guid uuid);
        Task<Guid?> RegisterClientAsync(Client client);
        Task<List<Client>> GetAllClientsAsync();
    }
}