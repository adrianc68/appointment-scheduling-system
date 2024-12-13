using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.ClientInterfaces
{
    public interface IDisableClient
    {
        bool DisableClient(int idClient);
    }
}