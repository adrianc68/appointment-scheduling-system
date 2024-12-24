namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces
{
    public interface IRegisterService
    {
        Task<Guid?> RegisterService(Model.Service service);
    }
}