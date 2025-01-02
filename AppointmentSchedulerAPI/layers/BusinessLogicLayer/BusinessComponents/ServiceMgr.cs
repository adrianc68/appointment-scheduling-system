using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class ServiceMgr : IServiceMgt
    {
        private readonly IServiceRepository serviceRepository;
        public ServiceMgr(IServiceRepository serviceRepository)
        {
            this.serviceRepository = serviceRepository;
        }

        public async Task<bool> ChangeServiceStatusType(int idService, ServiceStatusType status)
        {
            bool isStatusChanged = await serviceRepository.ChangeServiceStatusType(idService, status);
            return isStatusChanged;
        }

        public async Task<List<Service>> GetAllServicesAsync()
        {
            return (List<Service>)await serviceRepository.GetAllServicesAsync();
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            Service? service = await serviceRepository.GetServiceByIdAsync(id);
            return service;
        }

        public async Task<Service?> GetServiceByUuidAsync(Guid uuid)
        {
            Service? service = await serviceRepository.GetServiceByUuidAsync(uuid);
            return service;
        }

        public async Task<int?> GetServiceIdByUuidAsync(Guid uuid)
        {
            int? serviceId = await serviceRepository.GetServiceIdByUuidAsync(uuid);
            return serviceId;
        }

        public async Task<bool> IsServiceNameRegisteredAsync(string name)
        {
            bool isServiceNameRegistered = await serviceRepository.IsServiceNameRegisteredAsync(name);
            return isServiceNameRegistered;
        }

        public async Task<bool> IsServiceRegisteredByUuidAsync(Guid uuid)
        {
            int? serviceId = await serviceRepository.GetServiceIdByUuidAsync(uuid);
            return serviceId != null;
        }

        public async Task<Guid?> RegisterServiceAsync(Service service)
        {
            service.Uuid = Guid.CreateVersion7();
            bool isRegistered = await serviceRepository.AddServiceAsync(service);
            if (isRegistered)
            {
                return service.Uuid.Value;
            }
            return null;
        }

        public async Task<bool> UpdateService(Service service)
        {
            bool isUpdated = await serviceRepository.UpdateService(service);
            return isUpdated;
        }

    }
}