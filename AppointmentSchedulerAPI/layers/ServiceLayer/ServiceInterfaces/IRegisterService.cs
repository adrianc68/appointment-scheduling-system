namespace AppointmentSchedulerAPI.layers.ServiceLayer.ServiceInterfaces
{
    public interface IRegisterService
    {
        bool RegisterService(AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Service service);
    }
}