using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces
{
    public interface IRegisterClient
    {
        Task<RegistrationResponse<Guid>> RegisterClientAsync(Client client);
    }
}