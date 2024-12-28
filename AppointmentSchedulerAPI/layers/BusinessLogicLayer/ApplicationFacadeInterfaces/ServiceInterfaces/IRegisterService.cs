using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces
{
    public interface IRegisterService
    {
        Task<RegistrationResponse<Guid>> RegisterService(Model.Service service);
    }
}