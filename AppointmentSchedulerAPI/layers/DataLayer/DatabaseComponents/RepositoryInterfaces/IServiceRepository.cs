using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;


namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<Service?> GetServiceByUuidAsync(Guid uuid);
        Task<Service?> GetServiceByIdAsync(int id);
        Task<int?> GetServiceIdByUuidAsync(Guid uuid);
        Task<bool> IsServiceNameRegisteredAsync(string name);
        Task<bool> AddServiceAsync(Service service);
        Task<bool> ChangeServiceStatusType(int idService, ServiceStatusType status);
        // bool DeleteService(int idService);
        // bool EditService(Service service);
    }
}