using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IServiceMgt
    {
        // bool ChangeServiceStatusType(ServiceStatusType status);
        // bool DeleteService(int idService);
        // bool EditService(Service service);
        Task<List<Service>> GetAllServicesAsync();
        // List<Service> GetServicesDetailsByIds(List<int> serviceIds);
        // ServiceStatusType GetServiceStatusType(int idService);
        // bool IsServiceInSpecificStatusType(int idService, ServiceStatusType expected);
        Task<Guid?> RegisterService(Service service);
    }
}