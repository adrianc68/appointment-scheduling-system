using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class ServiceMgr : IServiceMgt
    {
        public bool ChangeServiceStatusType(ServiceStatusType status)
        {
            throw new NotImplementedException();
        }

        public bool DeleteService(int idService)
        {
            throw new NotImplementedException();
        }

        public bool EditService(Service service)
        {
            throw new NotImplementedException();
        }

        public List<Service> GetServices()
        {
            throw new NotImplementedException();
        }

        public List<Service> GetServicesDetailsByIds(List<int> serviceIds)
        {
            throw new NotImplementedException();
        }

        public ServiceStatusType GetServiceStatusType(int idService)
        {
            throw new NotImplementedException();
        }

        public bool IsServiceInSpecificStatusType(int idService, ServiceStatusType expected)
        {
            throw new NotImplementedException();
        }

        public bool RegisterService(List<Service> services)
        {
            throw new NotImplementedException();
        }
    }
}