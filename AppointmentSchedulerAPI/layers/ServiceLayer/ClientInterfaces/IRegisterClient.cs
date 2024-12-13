using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.ClientInterfaces
{
    public interface IRegisterClient
    {
        bool RegisterClient(Client client);
    }
}