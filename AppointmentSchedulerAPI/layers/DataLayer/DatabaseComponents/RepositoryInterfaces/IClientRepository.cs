using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;


namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IClientRepository
    {
        bool ChangeClientStatusType(int idClient, ClientStatusType status);
        Client GetClientDetails(int idClient);
        ClientStatusType GetClientStatusType(int idClient);
        bool IsClientAvailable(int idClient);
        bool RegisterClient(Client client);
    }
}