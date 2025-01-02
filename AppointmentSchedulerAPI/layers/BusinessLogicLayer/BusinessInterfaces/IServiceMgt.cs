using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IServiceMgt
    {
        Task<bool> UpdateService(Service service);
        Task<bool> ChangeServiceStatusType(int idService, ServiceStatusType status);
        Task<bool> IsServiceNameRegisteredAsync(string name);
        Task<bool> IsServiceRegisteredByUuidAsync(Guid uuid);
        Task<List<Service>> GetAllServicesAsync();
        Task<Service?> GetServiceByUuidAsync(Guid uuid);
        Task<Service?> GetServiceByIdAsync(int id);
        Task<int?> GetServiceIdByUuidAsync(Guid uuid);
        Task<Guid?> RegisterServiceAsync(Service service);
    }
}