using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;


namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IServiceRepository
    {
        Task<bool> RegisterService(Service service);
        // bool ChangeServiceStatusType(ServiceStatusType status);
        // bool DeleteService(int idService);
        // bool EditService(Service service);
        // List<Service> GetServices();
        // List<Service> GetServicesDetailsByIds(List<int> serviceIds);
        // ServiceStatusType GetServiceStatusType(int idService);
        // bool IsServiceInSpecificStatusType(int idService, ServiceStatusType expected);
    }
}