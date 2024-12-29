using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IAssistantMgt
    {
        // bool ChangeAssistantStatus(int idAssistant, AssistantStatusType status);
        // AssistantStatusType GetAssistantStatus(int idAssistant);
        // bool GetServicesAssignedToAssistant(int idAssistant);
        // bool UpdateAssistant(int idAssistant, Assistant assistant);
        Task<OperationResult<bool>> IsAccountDataRegisteredAsync(Assistant assistant);
        Task<bool> IsAssistantRegisteredByUuidAsync(Guid uuid);
        Task<OperationResult<Guid>> RegisterAssistantAsync(Assistant assistant);
        Task<List<Assistant>> GetAllAssistantsAsync();
        Task<bool> AssignServicesToAssistant(Guid assistantUuid, List<Guid?> servicesUuid);
    }
}