using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class ServiceMgr : IServiceMgt, IServiceEvent
    {
        private readonly IServiceRepository serviceRepository;
        private static readonly List<IServiceObserver> observers = new();

        public ServiceMgr(IServiceRepository serviceRepository)
        {
            this.serviceRepository = serviceRepository;
        }

        public async Task<bool> ChangeServiceStatusTypeAsync(int idService, ServiceStatusType status)
        {
            bool isStatusChanged = await serviceRepository.ChangeServiceStatusTypeAsync(idService, status);
            if (isStatusChanged)
            {
                ServiceEvent serviceEvent = new ServiceEvent
                {
                    ServiceId = idService,
                    EventType = ServiceEventType.UPDATED,
                };

                if (status == ServiceStatusType.DISABLED)
                {
                    serviceEvent.EventType = ServiceEventType.DISABLED;
                }
                else if (status == ServiceStatusType.ENABLED)
                {
                    serviceEvent.EventType = ServiceEventType.ENABLED;
                }
                else if (status == ServiceStatusType.DELETED)
                {
                    serviceEvent.EventType = ServiceEventType.DELETED;
                }
                this.NotifySuscribers(serviceEvent);
            }
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

        public async Task<bool> UpdateServiceAsync(Service service)
        {
            bool isUpdated = await serviceRepository.UpdateServiceAsync(service);
            return isUpdated;
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

        public void Suscribe(IServiceObserver serviceObserver)
        {
            if (!observers.Contains(serviceObserver))
            {
                observers.Add(serviceObserver);
            }
        }

        public void Unsuscribe(IServiceObserver serviceObserver)
        {
            if (observers.Contains(serviceObserver))
            {
                observers.Remove(serviceObserver);
            }
        }

        public void NotifySuscribers(ServiceEvent eventType)
        {
            foreach (var observer in observers)
            {
                observer.UpdateOnServiceChanged(eventType);
            }
        }
    }
}