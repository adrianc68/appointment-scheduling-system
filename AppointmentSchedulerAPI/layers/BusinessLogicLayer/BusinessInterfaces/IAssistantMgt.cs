using System.Collections.Generic;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IAssistantMgt
    {
        // bool ChangeAssistantStatus(int idAssistant, AssistantStatusType status);
        // AssistantStatusType GetAssistantStatus(int idAssistant);
        // bool GetServicesAssignedToAssistant(int idAssistant);
        // bool IsAssistantRegistered(Assistant assistant);
        Task<Guid?> RegisterAssistantAsync(Assistant assistant);
        // bool UpdateAssistant(int idAssistant, Assistant assistant);
        Task<List<Assistant>> GetAllAssistantsAsync();
        Task<bool> AssignServicesToAssistant(Guid assistantUuid, List<Guid?> servicesUuid);
    }
}