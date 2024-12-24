using System;
using System.Collections.Generic;
using System.Linq;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class ClientMgr : IClientMgt
    {
        private readonly IClientRepository clientRepository;

        public ClientMgr(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }
        public async Task<List<Client>> GetAllClientsAsync()
        {
            return (List<Client>)await clientRepository.GetAllClientsAsync();
        }

        public async Task<Guid?> RegisterClientAsync(Client client)
        {
            client.Uuid = Guid.CreateVersion7();
            bool isRegistered = await clientRepository.RegisterClientAsync(client);
            if (!isRegistered)
            {
                return null;
            }
            return client.Uuid.Value;
        }
    }
}