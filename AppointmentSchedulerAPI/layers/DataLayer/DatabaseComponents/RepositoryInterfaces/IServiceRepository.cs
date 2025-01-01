using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;


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
        // bool ChangeServiceStatusType(ServiceStatusType status);
        // bool DeleteService(int idService);
        // bool EditService(Service service);
        // List<Service> GetServices();
        // List<Service> GetServicesDetailsByIds(List<int> serviceIds);
        // ServiceStatusType GetServiceStatusType(int idService);
        // bool IsServiceInSpecificStatusType(int idService, ServiceStatusType expected);
    }
}