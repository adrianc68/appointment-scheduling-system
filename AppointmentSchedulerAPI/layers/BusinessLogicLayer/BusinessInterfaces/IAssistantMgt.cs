using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IAssistantMgt
    {
        Task<Guid?> RegisterAssistantAsync(Assistant assistant);
        Task<bool> UpdateAssistantAsync(Assistant assistant);
        Task<bool> AssignListServicesToAssistantAsync(int idAssistant, List<int> idServices);
        Task<List<Assistant>> GetAllAssistantsAsync();
        Task<Assistant?> GetAssistantByUuidAsync(Guid uuid);
        Task<int?> GetServiceIdByServiceOfferUuidAsync(Guid uuid);
        Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid);
        Task<bool> IsAssistantRegisteredByUuidAsync(Guid uuid);
         Task<bool> IsAssistantOfferingServiceByUuidAsync(int idService, int idAssistant);
    }
}