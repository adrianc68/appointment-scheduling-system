using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
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

        public async Task<List<Service>> GetAllServicesAsync()
        {
            return (List<Service>)await serviceRepository.GetAllServicesAsync();
        }
        public async Task<OperationResult<Service?>> GetServiceByUuidAsync(Guid uuid)
        {
            Service? service = await serviceRepository.GetServiceByUuidAsync(uuid);
            if (service == null)
            {
                return new OperationResult<Service?>(false, MessageCodeType.DATA_NOT_FOUND);
            }
            return new OperationResult<Service?>(true, MessageCodeType.DATA_FOUND, service);
        }

        public async Task<OperationResult<int?>> GetServiceIdByUuidAsync(Guid uuid)
        {
            int? serviceId = await serviceRepository.GetServiceIdByUuidAsync(uuid);
            if (serviceId == null)
            {
                return new OperationResult<int?>(false, MessageCodeType.DATA_NOT_FOUND);
            }
            return new OperationResult<int?>(true, MessageCodeType.DATA_FOUND, serviceId);
        }

        public async Task<OperationResult<bool?>> IsServiceDataRegisteredAsync(Service service)
        {
            if (string.IsNullOrWhiteSpace(service.Name))
            {
                return new OperationResult<bool?>(false, MessageCodeType.NULL_VALUE_IS_PRESENT, true);
            }

            bool isServiceNameRegistered = await serviceRepository.IsServiceNameRegistered(service.Name);
            if (isServiceNameRegistered)
            {
                return new OperationResult<bool?>(true, MessageCodeType.SERVICE_NAME_ALREADY_REGISTERED, true);
            }
            return new OperationResult<bool?>(true, MessageCodeType.SUCCESS_OPERATION, false);
        }

        public async Task<bool> IsServiceRegisteredByUuidAsync(Guid uuid)
        {
            int? serviceId = await serviceRepository.GetServiceIdByUuidAsync(uuid);
            return serviceId != null;
        }

        public async Task<OperationResult<Guid?>> RegisterService(Service service)
        {
            service.Uuid = Guid.CreateVersion7();
            bool isRegistered = await serviceRepository.AddServiceAsync(service);
            if (isRegistered)
            {
                return new OperationResult<Guid?>(false, MessageCodeType.SUCCESS_OPERATION, service.Uuid.Value);
            }
            return new OperationResult<Guid?>(true, MessageCodeType.REGISTER_ERROR);
        }
    }
}