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

        public async Task<OperationResult<bool>> IsAccountDataRegisteredAsync(Client client)
        {
            // 1. Check if username is is registered
            if (string.IsNullOrWhiteSpace(client.Username) ||
                 string.IsNullOrWhiteSpace(client.Email) ||
                 string.IsNullOrWhiteSpace(client.PhoneNumber))
            {
                return new OperationResult<bool>
                {
                    IsSuccessful = false,
                    Code = MessageCodeType.NULL_VALUE_IS_PRESENT
                };
            }

            bool isUsernameRegistered = await clientRepository.isUsernameRegisteredAsync(client.Username);
            if (isUsernameRegistered)
            {
                return new OperationResult<bool>
                {
                    IsSuccessful = true,
                    Code = MessageCodeType.USERNAME_ALREADY_REGISTERED,
                    Data = true
                };
            }

            // 2. Check if email is registered
            bool isEmailRegistered = await clientRepository.isEmailRegisteredAsync(client.Email);
            if (isEmailRegistered)
            {
                return new OperationResult<bool>
                {
                    IsSuccessful = true,
                    Code = MessageCodeType.EMAIL_ALREADY_REGISTERED,
                    Data = true
                };
            }

            // 3. Check if phoneNumber is registered
            bool isPhoneNumberRegistered = await clientRepository.IsPhoneNumberRegisteredAsync(client.PhoneNumber);
            if (isPhoneNumberRegistered)
            {
                return new OperationResult<bool>
                {
                    IsSuccessful = true,
                    Code = MessageCodeType.PHONE_NUMBER_ALREADY_REGISTERED
                };
            }
            return new OperationResult<bool>
            {
                IsSuccessful = true,
                Data = false,
            };
        }

        public async Task<bool> IsClientRegisteredByUuidAsync(Guid uuid)
        {
            int? clientId = await clientRepository.GetClientIdByUuidAsync(uuid);
            return clientId != null;
        }

        public async Task<OperationResult<Guid>> RegisterClientAsync(Client client)
        {
            client.Uuid = Guid.CreateVersion7();

            bool isRegistered = await clientRepository.AddClientAsync(client);
            if (isRegistered)
            {
                return new OperationResult<Guid>
                {
                    IsSuccessful = true,
                    Data = client.Uuid.Value,
                    Code = MessageCodeType.SUCCESS_OPERATION

                };
            }
            return new OperationResult<Guid>
            {
                IsSuccessful = true,
                Code = MessageCodeType.REGISTER_ERROR
            };

        }
    }
}