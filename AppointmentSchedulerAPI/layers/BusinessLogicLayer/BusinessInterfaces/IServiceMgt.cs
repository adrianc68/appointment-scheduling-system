using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IServiceMgt
    {
        Task<bool> ChangeServiceStatusType(int idService, ServiceStatusType status);
        // bool DeleteService(int idService);
        // bool EditService(Service service);
        
        Task<bool> IsServiceNameRegisteredAsync(string serviceName);
        Task<bool> IsServiceRegisteredByUuidAsync(Guid uuid);
        Task<List<Service>> GetAllServicesAsync();
        Task<Service?> GetServiceByUuidAsync(Guid uuid);
        Task<Service?> GetServiceByIdAsync(int id);
        Task<int?> GetServiceIdByUuidAsync(Guid uuid);
        Task<Guid?> RegisterServiceAsync(Service service);
        // ServiceStatusType GetServiceStatusType(int idService);
        // bool IsServiceInSpecificStatusType(int idService, ServiceStatusType expected);
    }
}