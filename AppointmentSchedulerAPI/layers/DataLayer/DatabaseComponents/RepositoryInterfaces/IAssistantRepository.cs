
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IAssistantRepository
    {
        Task<IEnumerable<Assistant>> GetAllAssistantsAsync();
        Task<Assistant?> GetAssistantByUuidAsync(Guid uuid);
        Task<List<Service>> GetServicesAssignedToAssistantByUuidAsync(Guid uuid);
        Task<int?> GetServiceIdByAssistantServiceUuidAsync(Guid uuid);
        Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid);
        Task<bool> AddAssistantAsync(Assistant assistant);
        Task<bool> AddServicesToAssistantAsync(int idAssistant, List<int> idServices);
        Task<int?> GetAssistantIdByUuidAsync(Guid uuid);
        Task<bool> IsUsernameRegisteredAsync(string username);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
        Task<bool> IsAssistantOfferingServiceByUuidAsync(int idService, int idAssistant);
        Task<bool> ChangeAssistantStatus(int idAssistant, AssistantStatusType status);
        Task<bool> UpdateAssistantAsync(Assistant assistant);
        // Task<bool> DeleteAssistantAsync(Guid uuid);
    }
}