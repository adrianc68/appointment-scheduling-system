using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class AssistantMgr : IAssistantMgt
    {
        private readonly IAssistantRepository assistantRepository;

        public AssistantMgr(IAssistantRepository assistantRepository)
        {
            this.assistantRepository = assistantRepository;
        }

        public async Task<bool> AssignServicesToAssistant(Guid assistantUuid, List<Guid?> servicesUuid)
        {
            bool areAllServicesRegistered = await assistantRepository.AddServicesToAssistantAsync(assistantUuid, servicesUuid);
            return areAllServicesRegistered;
        }

        public async Task<List<Assistant>> GetAllAssistantsAsync()
        {
            return (List<Assistant>)await assistantRepository.GetAllAssistantsAsync();
        }

        public async Task<Assistant?> GetAssistantByUuidAsync(Guid uuid)
        {
            Assistant? assistant = await assistantRepository.GetAssistantByUuidAsync(uuid);
            return assistant;
        }

        public async Task<int?> GetServiceIdByServiceOfferUuidAsync(Guid uuid)
        {
            int? assistantId = await assistantRepository.GetServiceIdByAssistantServiceUuid(uuid);
            return assistantId;
        }

        public async Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid)
        {
            ServiceOffer? serviceOffer = await assistantRepository.GetServiceOfferByUuid(uuid);
            return serviceOffer;
        }

        public async Task<OperationResult<bool?>> IsAccountDataRegisteredAsync(Assistant assistant)
        {
            if (string.IsNullOrWhiteSpace(assistant.Username) ||
                 string.IsNullOrWhiteSpace(assistant.Email) ||
                 string.IsNullOrWhiteSpace(assistant.PhoneNumber))
            {
                return new OperationResult<bool?>(false, MessageCodeType.NULL_VALUE_IS_PRESENT);
            }

            // 1. Check if username is is registered
            bool isUsernameRegistered = await assistantRepository.IsUsernameRegisteredAsync(assistant.Username);
            if (isUsernameRegistered)
            {
                return new OperationResult<bool?>(true, MessageCodeType.USERNAME_ALREADY_REGISTERED, isUsernameRegistered);
            }

            // 2. Check if email is registered
            bool isEmailRegistered = await assistantRepository.IsEmailRegisteredAsync(assistant.Email);
            if (isEmailRegistered)
            {
                return new OperationResult<bool?>(true, MessageCodeType.EMAIL_ALREADY_REGISTERED, isEmailRegistered);
            }

            // 3. Check if phoneNumber is registered
            bool isPhoneNumberRegistered = await assistantRepository.IsPhoneNumberRegisteredAsync(assistant.PhoneNumber);
            if (isPhoneNumberRegistered)
            {
                return new OperationResult<bool?>(true, MessageCodeType.PHONE_NUMBER_ALREADY_REGISTERED, isPhoneNumberRegistered);
            }
            return new OperationResult<bool?>(true, MessageCodeType.OK, false);
        }

        public async Task<bool> IsAssistantRegisteredByUuidAsync(Guid uuid)
        {
            int? assistantId = await assistantRepository.GetAssistantIdByUuidAsync(uuid);
            return assistantId != null;
        }

        public async Task<Guid?> RegisterAssistantAsync(Assistant assistant)
        {
            assistant.Uuid = Guid.CreateVersion7();
            bool isRegistered = await assistantRepository.AddAssistantAsync(assistant);
            if (!isRegistered)
            {
                assistant.Uuid = null;
                return null;
            }
            return assistant.Uuid.Value;
        }

    }
}