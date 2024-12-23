
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IAssistantRepository
    {
        bool ChangeAssistantStatus(int idAssistant, AssistantStatusType status);
        AssistantStatusType GetAssistantStatus(int idAssistant);
        bool GetServicesAssignedToAssistant(int idAssistant);
        bool IsAssistantRegistered(Assistant assistant);
        bool RegisterAssistant(Assistant assistant);
        bool UpdateAssistant(int idAssistant, Assistant assistant);
    }
}