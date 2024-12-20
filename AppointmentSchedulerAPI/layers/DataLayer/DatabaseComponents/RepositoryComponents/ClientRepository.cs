using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class ClientRepository : IClientRepository
    {
        // private readonly Model.AppointmentDbContext context;

        // public ClientRepository(Model.AppointmentDbContext context)
        // {
        //     this.context = context;
        // }

        public bool ChangeClientStatusType(int idClient, ClientStatusType status)
        {
            throw new NotImplementedException();
        }

        public Client GetClientDetails(int idClient)
        {
            throw new NotImplementedException();
        }

        public ClientStatusType GetClientStatusType(int idClient)
        {
            throw new NotImplementedException();
        }

        public bool IsClientAvailable(int idClient)
        {
            throw new NotImplementedException();
        }

        public bool RegisterClient(Client client)
        {
            throw new NotImplementedException();
        }
    }
}