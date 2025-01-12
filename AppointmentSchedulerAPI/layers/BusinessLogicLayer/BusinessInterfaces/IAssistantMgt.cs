using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IAssistantMgt
    {
        Task<bool> UpdateAssistant(Assistant assistant);
        Task<List<Assistant>> GetAllAssistantsAsync();
        Task<Assistant?> GetAssistantByUuidAsync(Guid uuid);
        Task<int?> GetServiceIdByServiceOfferUuidAsync(Guid uuid);
        Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid);
        Task<bool> IsAssistantRegisteredByUuidAsync(Guid uuid);
         Task<bool> IsAssistantOfferingServiceByUuidAsync(int idService, int idAssistant);
        Task<Guid?> RegisterAssistantAsync(Assistant assistant);
        Task<bool> AssignListServicesToAssistantAsync(int idAssistant, List<int> idServices);
    }
}