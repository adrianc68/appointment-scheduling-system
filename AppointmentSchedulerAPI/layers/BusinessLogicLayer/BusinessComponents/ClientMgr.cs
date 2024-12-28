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

        public async Task<RegistrationResponse<Guid>> RegisterClientAsync(Client client)
        {

            // 1. Check if username is is registered
            if (string.IsNullOrWhiteSpace(client.Username) ||
                 string.IsNullOrWhiteSpace(client.Email) ||
                 string.IsNullOrWhiteSpace(client.PhoneNumber))
            {
                return new RegistrationResponse<Guid>
                {
                    IsSuccessful = false,
                    Code = MessageCodeType.NULL_VALUE_IS_PRESENT
                };
            }

            bool isUsernameRegistered = await clientRepository.isUsernameRegistered(client.Username);
            if (isUsernameRegistered)
            {
                return new RegistrationResponse<Guid>
                {
                    IsSuccessful = false,
                    Code = MessageCodeType.USERNAME_ALREADY_REGISTERED
                };
            }

            // 2. Check if email is registered
            bool isEmailRegistered = await clientRepository.isEmailRegistered(client.Email);
            if (isEmailRegistered)
            {
                return new RegistrationResponse<Guid>
                {
                    IsSuccessful = false,
                    Code = MessageCodeType.EMAIL_ALREADY_REGISTERED
                };
            }

            // 3. Check if phoneNumber is registered
            bool isPhoneNumberRegistered = await clientRepository.IsPhoneNumberRegistered(client.PhoneNumber);
            if (isPhoneNumberRegistered)
            {
                return new RegistrationResponse<Guid>
                {
                    IsSuccessful = false,
                    Code = MessageCodeType.PHONE_NUMBER_ALREADY_REGISTERED
                };
            }
            // 4. Create Guid UUID
            client.Uuid = Guid.CreateVersion7();

            bool isRegistered = await clientRepository.AddClientAsync(client);
            if (isRegistered)
            {
                return new RegistrationResponse<Guid>
                {
                    IsSuccessful = true,
                    Data = client.Uuid.Value,
                    Code = MessageCodeType.SUCCESS_OPERATION

                };
            }
            return new RegistrationResponse<Guid>
            {
                IsSuccessful = true,
                Code = MessageCodeType.REGISTER_ERROR
            };

        }
    }
}