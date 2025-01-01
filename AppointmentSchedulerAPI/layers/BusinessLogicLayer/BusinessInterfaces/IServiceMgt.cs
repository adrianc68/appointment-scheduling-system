using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IServiceMgt
    {
        // bool ChangeServiceStatusType(ServiceStatusType status);
        // bool DeleteService(int idService);
        // bool EditService(Service service);
        
        Task<bool> IsServiceNameRegistered(string serviceName);
        Task<bool> IsServiceRegisteredByUuidAsync(Guid uuid);
        Task<List<Service>> GetAllServicesAsync();
        Task<Service?> GetServiceByUuidAsync(Guid uuid);
        Task<Service?> GetServiceByIdAsync(int id);
        Task<int?> GetServiceIdByUuidAsync(Guid uuid);
        Task<Guid?> RegisterService(Service service);
        // List<Service> GetServicesDetailsByIds(List<int> serviceIds);
        // ServiceStatusType GetServiceStatusType(int idService);
        // bool IsServiceInSpecificStatusType(int idService, ServiceStatusType expected);
    }
}