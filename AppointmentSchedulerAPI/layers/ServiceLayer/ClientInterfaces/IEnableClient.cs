using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.ClientInterfaces
{
    public interface IEnableClient
    {
        bool EnableClient(int idClient);
    }
}