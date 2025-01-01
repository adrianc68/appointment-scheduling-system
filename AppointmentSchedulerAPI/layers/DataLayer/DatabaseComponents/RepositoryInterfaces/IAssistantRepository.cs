
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IAssistantRepository
    {

        Task<bool> AddAssistantAsync(Assistant assistant);
        Task<bool> AddServicesToAssistantAsync(Guid assistantUuid, List<Guid?> servicesUuid);
        Task<IEnumerable<Assistant>> GetAllAssistantsAsync();
        Task<Assistant?> GetAssistantByUuidAsync(Guid uuid);
        Task<int?> GetAssistantIdByUuidAsync(Guid uuid);
        Task<bool> IsUsernameRegisteredAsync(string username);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
        Task<List<Service>> GetServicesAssignedToAssistantByUuidAsync(Guid uuid);
         Task<int?> GetServiceIdByAssistantServiceUuid(Guid uuid);
         Task<ServiceOffer?> GetServiceOfferByUuid(Guid uuid);

        // Task<bool> UpdateAssistantAsync(Assistant assistant);
        // Task<bool> DeleteAssistantAsync(Guid uuid);
        // bool ChangeAssistantStatus(int idAssistant, AssistantStatusType status);
        // AssistantStatusType GetAssistantStatus(int idAssistant);
        // bool UpdateAssistant(int idAssistant, Assistant assistant);
    }
}