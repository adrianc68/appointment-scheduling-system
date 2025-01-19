using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IServiceMgt
    {
        Task<Guid?> RegisterServiceAsync(Service service);
        Task<bool> UpdateServiceAsync(Service service);
        Task<bool> ChangeServiceStatusTypeAsync(int idService, ServiceStatusType status);
        Task<List<Service>> GetAllServicesAsync();
        Task<Service?> GetServiceByUuidAsync(Guid uuid);
        Task<Service?> GetServiceByIdAsync(int id);
        Task<int?> GetServiceIdByUuidAsync(Guid uuid);
        Task<bool> IsServiceNameRegisteredAsync(string name);
        Task<bool> IsServiceRegisteredByUuidAsync(Guid uuid);
    }
}