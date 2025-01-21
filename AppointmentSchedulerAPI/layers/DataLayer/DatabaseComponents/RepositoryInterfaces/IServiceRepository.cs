using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;


namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IServiceRepository
    {
        Task<bool> UpdateServiceAsync(Service service);
        Task<bool> AddServiceAsync(Service service);
        Task<bool> ChangeServiceStatusTypeAsync(int idService, ServiceStatusType status);
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<Service?> GetServiceByUuidAsync(Guid uuid);
        Task<Service?> GetServiceByIdAsync(int id);
        Task<int?> GetServiceIdByUuidAsync(Guid uuid);
        Task<bool> IsServiceNameRegisteredAsync(string name);
    }
}