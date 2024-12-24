
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IAssistantRepository
    {

        Task<bool> RegisterAssistantAsync(Assistant assistant);
        // Task<Assistant?> GetAssistantByUuidAsync(Guid uuid);
        Task<IEnumerable<Assistant>> GetAllAssistantsAsync();
        // Task<bool> UpdateAssistantAsync(Assistant assistant);
        // Task<bool> DeleteAssistantAsync(Guid uuid);


        // bool ChangeAssistantStatus(int idAssistant, AssistantStatusType status);
        // AssistantStatusType GetAssistantStatus(int idAssistant);
        // bool GetServicesAssignedToAssistant(int idAssistant);
        // bool IsAssistantRegistered(Assistant assistant);
        // bool UpdateAssistant(int idAssistant, Assistant assistant);
    }
}