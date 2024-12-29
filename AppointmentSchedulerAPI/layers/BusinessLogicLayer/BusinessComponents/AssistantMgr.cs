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

        public bool ChangeAssistantStatus(int idAssistant, AssistantStatusType status)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Assistant>> GetAllAssistantsAsync()
        {
            return (List<Assistant>)await assistantRepository.GetAllAssistantsAsync();
        }

        public AssistantStatusType GetAssistantStatus(int idAssistant)
        {
            throw new NotImplementedException();
        }

        public bool GetServicesAssignedToAssistant(int idAssistant)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<bool>> IsAccountDataRegisteredAsync(Assistant assistant)
        {
            if (string.IsNullOrWhiteSpace(assistant.Username) ||
                 string.IsNullOrWhiteSpace(assistant.Email) ||
                 string.IsNullOrWhiteSpace(assistant.PhoneNumber))
            {
                return new OperationResult<bool>
                {
                    IsSuccessful = false,
                    Code = MessageCodeType.NULL_VALUE_IS_PRESENT
                };
            }

            // 1. Check if username is is registered
            bool isUsernameRegistered = await assistantRepository.isUsernameRegistered(assistant.Username);
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
            bool isEmailRegistered = await assistantRepository.isEmailRegistered(assistant.Email);
            if (isEmailRegistered)
            {
                return new OperationResult<bool>
                {
                    IsSuccessful = false,
                    Code = MessageCodeType.EMAIL_ALREADY_REGISTERED,
                    Data = true,
                };
            }

            // 3. Check if phoneNumber is registered
            bool isPhoneNumberRegistered = await assistantRepository.IsPhoneNumberRegistered(assistant.PhoneNumber);
            if (isPhoneNumberRegistered)
            {
                return new OperationResult<bool>
                {
                    IsSuccessful = false,
                    Code = MessageCodeType.PHONE_NUMBER_ALREADY_REGISTERED,
                    Data = true
                };
            }
            return new OperationResult<bool>
            {
                IsSuccessful = true,
                Data = false,
            };
        }

        public async Task<bool> IsAssistantRegisteredByUuidAsync(Guid uuid)
        {
            int? assistantId = await assistantRepository.GetAssistantIdByUuidAsync(uuid);
            return assistantId != null;
        }

        public async Task<OperationResult<Guid>> RegisterAssistantAsync(Assistant assistant)
        {
            assistant.Uuid = Guid.CreateVersion7();
            bool isRegistered = await assistantRepository.AddAssistantAsync(assistant);
            if (isRegistered)
            {
                return new OperationResult<Guid>
                {
                    IsSuccessful = true,
                    Data = assistant.Uuid.Value,
                    Code = MessageCodeType.SUCCESS_OPERATION

                };
            }
            return new OperationResult<Guid>
            {
                IsSuccessful = false,
                Code = MessageCodeType.REGISTER_ERROR
            };
        }

        public bool UpdateAssistant(int idAssistant, Assistant assistant)
        {
            throw new NotImplementedException();
        }
    }
}