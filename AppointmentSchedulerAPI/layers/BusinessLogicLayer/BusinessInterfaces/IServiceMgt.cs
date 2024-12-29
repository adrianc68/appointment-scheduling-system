using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IServiceMgt
    {
        // bool ChangeServiceStatusType(ServiceStatusType status);
        // bool DeleteService(int idService);
        // bool EditService(Service service);
        
        Task<OperationResult<bool?>> IsServiceDataRegisteredAsync(Service service);
        Task<bool> IsServiceRegisteredByUuidAsync(Guid uuid);
        Task<List<Service>> GetAllServicesAsync();
        Task<OperationResult<Service?>> GetServiceByUuidAsync(Guid uuid);
        Task<OperationResult<int?>> GetServiceIdByUuidAsync(Guid uuid);
        Task<OperationResult<Guid?>> RegisterService(Service service);
        // List<Service> GetServicesDetailsByIds(List<int> serviceIds);
        // ServiceStatusType GetServiceStatusType(int idService);
        // bool IsServiceInSpecificStatusType(int idService, ServiceStatusType expected);
    }
}