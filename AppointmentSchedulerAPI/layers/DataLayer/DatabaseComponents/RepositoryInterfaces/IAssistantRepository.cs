
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IAssistantRepository
    {

        Task<bool> AddAssistantAsync(Assistant assistant);
        Task<bool> AddServicesToAssistantAsync(Guid assistantUuid, List<Guid?> servicesUuid);
        Task<IEnumerable<Assistant>> GetAllAssistantsAsync();
        Task<Assistant?> GetAssistantByUuidAsync(Guid uuid);
        Task<int?> GetAssistantIdByUuidAsync(Guid uuid);
        Task<List<Service>> GetServicesAssignedToAssistantByUuidAsync(Guid uuid);

        Task<bool> isUsernameRegisteredAsync(string username);
        Task<bool> isEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);

        // Task<bool> UpdateAssistantAsync(Assistant assistant);
        // Task<bool> DeleteAssistantAsync(Guid uuid);


        // bool ChangeAssistantStatus(int idAssistant, AssistantStatusType status);
        // AssistantStatusType GetAssistantStatus(int idAssistant);
        // bool UpdateAssistant(int idAssistant, Assistant assistant);
    }
}