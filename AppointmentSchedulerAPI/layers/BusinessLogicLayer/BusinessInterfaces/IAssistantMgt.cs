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
        Task<List<Assistant>> GetAllAssistantsAsync();
        Task<Assistant?> GetAssistantByUuidAsync(Guid uuid);
        Task<int?> GetServiceIdByServiceOfferUuidAsync(Guid uuid);
        Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid);
        Task<bool> IsUsernameRegisteredAsync(string username);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
        Task<bool> IsAssistantRegisteredByUuidAsync(Guid uuid);
         Task<bool> IsAssistantOfferingServiceByUuidAsync(int idService, int idAssistant);
        Task<Guid?> RegisterAssistantAsync(Assistant assistant);
        Task<bool> AssignServicesToAssistantAsync(int idAssistant, List<int> idServices);
    }
}