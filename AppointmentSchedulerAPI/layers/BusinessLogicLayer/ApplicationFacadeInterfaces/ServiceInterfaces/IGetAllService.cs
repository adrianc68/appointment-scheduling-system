using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces
{
    public interface IGetAllService
    {
        Task<List<Service>> GetAllServicesAsync();
    }
}