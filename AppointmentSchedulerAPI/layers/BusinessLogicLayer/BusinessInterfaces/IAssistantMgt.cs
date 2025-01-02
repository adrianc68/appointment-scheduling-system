using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IAssistantMgt
    {
        // Task<bool> UpdateAssistant(Assistant assistant);
        Task<bool> ChangeAssistantStatusAsync(int idAssistant, AssistantStatusType status);
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
        Task<bool> AssignListServicesToAssistantAsync(int idAssistant, List<int> idServices);
    }
}