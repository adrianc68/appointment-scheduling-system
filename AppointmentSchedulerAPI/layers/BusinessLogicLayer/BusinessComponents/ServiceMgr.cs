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

        public async Task<List<Service>> GetAllServicesAsync()
        {
            return (List<Service>)await serviceRepository.GetAllServicesAsync();
        }

        public List<Service> GetServicesDetailsByIds(List<int> serviceIds)
        {
            throw new NotImplementedException();
        }

        public ServiceStatusType GetServiceStatusType(int idService)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<bool>> IsServiceDataRegisteredAsync(Service service)
        {
            if (string.IsNullOrWhiteSpace(service.Name))
            {
                return new OperationResult<bool>
                {
                    IsSuccessful = false,
                    Code = MessageCodeType.NULL_VALUE_IS_PRESENT,
                    Data = true
                };
            }

            bool isServiceNameRegistered = await serviceRepository.IsServiceNameRegistered(service.Name);
            if (isServiceNameRegistered)
            {
                return new OperationResult<bool>
                {
                    IsSuccessful = true,
                    Code = MessageCodeType.SERVICE_NAME_ALREADY_REGISTERED,
                    Data = true
                };
            }
            return new OperationResult<bool>
            {
                IsSuccessful = true,
                Data = false,
            };

        }

        public async Task<bool> IsServiceRegisteredByUuidAsync(Guid uuid)
        {
            int? serviceId = await serviceRepository.GetServiceIdByUuidAsync(uuid);
            return serviceId != null;
        }

        public async Task<OperationResult<Guid>> RegisterService(Service service)
        {

            service.Uuid = Guid.CreateVersion7();
            bool isRegistered = await serviceRepository.AddServiceAsync(service);
            if (isRegistered)
            {
                return new OperationResult<Guid>
                {
                    IsSuccessful = true,
                    Data = service.Uuid.Value,
                    Code = MessageCodeType.SUCCESS_OPERATION
                };
            }
            return new OperationResult<Guid>
            {
                IsSuccessful = true,
                Code = MessageCodeType.REGISTER_ERROR
            };
        }
    }
}