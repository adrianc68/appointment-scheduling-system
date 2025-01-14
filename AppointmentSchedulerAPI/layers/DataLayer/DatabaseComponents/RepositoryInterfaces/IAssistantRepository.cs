
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IAssistantRepository
    {
        Task<IEnumerable<Assistant>> GetAllAssistantsAsync();
        Task<Assistant?> GetAssistantByUuidAsync(Guid uuid);
        Task<List<Service>> GetServicesAssignedToAssistantByUuidAsync(Guid uuid);
        Task<int?> GetServiceIdByAssistantServiceUuidAsync(Guid uuid);
        Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid);
        Task<bool> AddAssistantAsync(Assistant assistant);
        Task<bool> AddServicesToAssistantAsync(int idAssistant, List<int> idServices);
        Task<int?> GetAssistantIdByUuidAsync(Guid uuid);
        Task<bool> IsAssistantOfferingServiceByUuidAsync(int idService, int idAssistant);
        Task<bool> UpdateAssistantAsync(Assistant assistant);
    }
}