using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
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

        public async Task<OperationResult<bool?>> IsAccountDataRegisteredAsync(Client client)
        {
            // 1. Check if username is is registered
            if (string.IsNullOrWhiteSpace(client.Username) ||
                 string.IsNullOrWhiteSpace(client.Email) ||
                 string.IsNullOrWhiteSpace(client.PhoneNumber))
            {
                return new OperationResult<bool?>(false, MessageCodeType.NULL_VALUE_IS_PRESENT);
            }

            bool isUsernameRegistered = await clientRepository.IsUsernameRegisteredAsync(client.Username);
            if (isUsernameRegistered)
            {
                return new OperationResult<bool?>(true, MessageCodeType.USERNAME_ALREADY_REGISTERED, isUsernameRegistered);
            }

            // 2. Check if email is registered
            bool isEmailRegistered = await clientRepository.IsEmailRegisteredAsync(client.Email);
            if (isEmailRegistered)
            {
                return new OperationResult<bool?>(true, MessageCodeType.EMAIL_ALREADY_REGISTERED, isEmailRegistered);
            }

            // 3. Check if phoneNumber is registered
            bool isPhoneNumberRegistered = await clientRepository.IsPhoneNumberRegisteredAsync(client.PhoneNumber);
            if (isPhoneNumberRegistered)
            {
                return new OperationResult<bool?>(true, MessageCodeType.PHONE_NUMBER_ALREADY_REGISTERED);
            }
            return new OperationResult<bool?>(true, MessageCodeType.OK, false);
        }

        public async Task<bool> IsClientRegisteredByUuidAsync(Guid uuid)
        {
            int? clientId = await clientRepository.GetClientIdByUuidAsync(uuid);
            return clientId != null;
        }

        public async Task<Guid?> RegisterClientAsync(Client client)
        {
            client.Uuid = Guid.CreateVersion7();

            bool isRegistered = await clientRepository.AddClientAsync(client);
            if (isRegistered)
            {
                return client.Uuid.Value;
            }
            return null;
        }
    }
}