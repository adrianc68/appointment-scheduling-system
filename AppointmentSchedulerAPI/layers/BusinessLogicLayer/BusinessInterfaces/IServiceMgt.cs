using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IServiceMgt
    {
        bool ChangeServiceStatusType(ServiceStatusType status);
        bool DeleteService(int idService);
        bool EditService(Service service);
        List<Service> GetServices();
        List<Service> GetServicesDetailsByIds(List<int> serviceIds);
        ServiceStatusType GetServiceStatusType(int idService);
        bool IsServiceInSpecificStatusType(int idService, ServiceStatusType expected);
        bool RegisterService(List<Service> services);
    }
}