using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class ClientMgr : IClientMgt, IClientEvent
    {
        private readonly IClientRepository clientRepository;
        private static readonly List<IClientObserver> observers = new();

        public ClientMgr(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            return (List<Client>)await clientRepository.GetAllClientsAsync();
        }

        public async Task<Client?> GetClientByUuidAsync(Guid uuid)
        {
            Client? client = await clientRepository.GetClientByUuidAsync(uuid);
            return client;
        }

        public async Task<bool> IsClientRegisteredByUuidAsync(Guid uuid)
        {
            int? clientId = await clientRepository.GetClientIdByUuidAsync(uuid);
            return clientId != null;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            bool isEmailRegistered = await clientRepository.IsEmailRegisteredAsync(email);
            return isEmailRegistered;
        }

        public async Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber)
        {
            bool isPhoneNumberRegistered = await clientRepository.IsPhoneNumberRegisteredAsync(phoneNumber);
            return isPhoneNumberRegistered;
        }

        public async Task<bool> IsUsernameRegisteredAsync(string username)
        {
            bool isUsernameRegistered = await clientRepository.IsUsernameRegisteredAsync(username);
            return isUsernameRegistered;
        }

        public async Task<bool> ChangeClientStatusTypeAsync(int idClient, ClientStatusType status)
        {
            bool isStatusChanged = await clientRepository.ChangeClientStatusTypeAsync(idClient, status);
            if (isStatusChanged && (status == ClientStatusType.DISABLED || status == ClientStatusType.DELETED))
            {
                ClientEvent clientEvent = new()
                {
                    EventType = status == ClientStatusType.DISABLED ? ClientEventType.DISABLED : ClientEventType.DELETED,
                    ClientId = idClient,
                };
                this.NotifySuscribers(clientEvent);
            }
            return isStatusChanged;
        }

        public async Task<Guid?> RegisterClientAsync(Client client)
        {
            client.Uuid = Guid.CreateVersion7();

            bool isRegistered = await clientRepository.AddClientAsync(client);
            if (!isRegistered)
            {
                client.Uuid = null;
                return null;
            }
            return client.Uuid.Value;

        }

        public async Task<bool> UpdateClient(Client client)
        {
            bool isUpdated = await clientRepository.UpdateClientAsync(client);
            return isUpdated;
        }

        public void NotifySuscribers(ClientEvent eventType)
        {
            eventType.EventDate = DateTime.UtcNow;
            foreach (var observer in observers)
            {
                observer.UpdateOnClientChanged(eventType);
            }
        }

        public void Suscribe(IClientObserver clientObserver)
        {
            if (!observers.Contains(clientObserver))
            {
                observers.Add(clientObserver);
            }
        }

        public void Unsuscribe(IClientObserver clientObserver)
        {
            if (observers.Contains(clientObserver))
            {
                observers.Remove(clientObserver);
            }
        }
    }
}