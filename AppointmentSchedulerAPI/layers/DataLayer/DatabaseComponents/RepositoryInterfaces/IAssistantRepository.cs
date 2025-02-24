
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IAssistantRepository
    {
        Task<bool> AddAssistantAsync(Assistant assistant);
        Task<bool> UpdateAssistantAsync(Assistant assistant);
        Task<bool> AddServicesToAssistantAsync(int idAssistant, List<int> idServices);
        Task<IEnumerable<Assistant>> GetAllAssistantsAsync();
        Task<Assistant?> GetAssistantByUuidAsync(Guid uuid);
        Task<IEnumerable<ServiceOffer>> GetServicesAssignedToAssistantByUuidAsync(Guid uuid);
        Task<int?> GetServiceIdByAssistantServiceUuidAsync(Guid uuid);
        Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid);
        Task<int?> GetAssistantIdByUuidAsync(Guid uuid);
        Task<bool> IsAssistantOfferingServiceByUuidAsync(int idService, int idAssistant);

    }
}