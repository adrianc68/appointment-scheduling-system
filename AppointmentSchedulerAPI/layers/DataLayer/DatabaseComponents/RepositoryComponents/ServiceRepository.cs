using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class ServiceRepository : IServiceRepository
    {
        // private readonly Model.AppointmentDbContext context;

        // public ServiceRepository(Model.AppointmentDbContext context)
        // {
        //     this.context = context;
        // }

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