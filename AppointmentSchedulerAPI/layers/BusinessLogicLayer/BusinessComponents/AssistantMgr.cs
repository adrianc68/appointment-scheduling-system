using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class AssistantMgr : IAssistantMgt, IAssistantEvent
    {
        private readonly IAssistantRepository assistantRepository;
        private static readonly List<IAssistantObserver> observers = new();

        public AssistantMgr(IAssistantRepository assistantRepository)
        {
            this.assistantRepository = assistantRepository;
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
            int? assistantId = await assistantRepository.GetServiceIdByAssistantServiceUuidAsync(uuid);
            return assistantId;
        }

        public async Task<ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid)
        {
            ServiceOffer? serviceOffer = await assistantRepository.GetServiceOfferByUuidAsync(uuid);
            return serviceOffer;
        }

        public async Task<bool> IsAssistantOfferingServiceByUuidAsync(int idService, int idAssistant)
        {
            bool isAssistantOfferingService = await assistantRepository.IsAssistantOfferingServiceByUuidAsync(idService, idAssistant);
            return isAssistantOfferingService;
        }

        public async Task<bool> IsAssistantRegisteredByUuidAsync(Guid uuid)
        {
            int? assistantId = await assistantRepository.GetAssistantIdByUuidAsync(uuid);
            return assistantId != null;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            bool isEmailRegistered = await assistantRepository.IsEmailRegisteredAsync(email);
            return isEmailRegistered;
        }

        public async Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber)
        {
            bool isPhoneNumberRegistered = await assistantRepository.IsPhoneNumberRegisteredAsync(phoneNumber);
            return isPhoneNumberRegistered;
        }

        public async Task<bool> IsUsernameRegisteredAsync(string username)
        {
            bool isUsernameRegistered = await assistantRepository.IsUsernameRegisteredAsync(username);
            return isUsernameRegistered;
        }

        public async Task<bool> AssignListServicesToAssistantAsync(int idAssistant, List<int> idServices)
        {
            bool areAllServicesRegistered = await assistantRepository.AddServicesToAssistantAsync(idAssistant, idServices);
            return areAllServicesRegistered;
        }

        public async Task<bool> ChangeAssistantStatusAsync(int idAssistant, AssistantStatusType status)
        {
            bool isStatusChanged = await assistantRepository.ChangeAssistantStatus(idAssistant, status);

            if (isStatusChanged)
            {
                AssistantEvent assistantEvent = new AssistantEvent
                {
                    AssistantId = idAssistant,
                    EventType = AssistantEventType.UPDATED
                };
                if (status == AssistantStatusType.ENABLED)
                {
                    assistantEvent.EventType = AssistantEventType.ENABLED;
                }
                else if (status == AssistantStatusType.DISABLED)
                {
                    assistantEvent.EventType = AssistantEventType.DISABLED;
                }
                else if (status == AssistantStatusType.DELETED)
                {
                    assistantEvent.EventType = AssistantEventType.DELETED;
                }
                this.NotifySuscribers(assistantEvent);
            }
            return isStatusChanged;
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

        public async Task<bool> UpdateAssistant(Assistant assistant)
        {
            bool isUpdated = await assistantRepository.UpdateAssistantAsync(assistant);
            return isUpdated;
        }

        public void NotifySuscribers(AssistantEvent eventType)
        {
            foreach (var observer in observers)
            {
                observer.UpdateOnAssistantChanged(eventType);
            }
        }

        public void Suscribe(IAssistantObserver assistantObserver)
        {
            if (!observers.Contains(assistantObserver))
            {
                observers.Add(assistantObserver);
            }
        }

        public void Unsuscribe(IAssistantObserver assistantObserver)
        {
            if (observers.Contains(assistantObserver))
            {
                observers.Remove(assistantObserver);
            }
        }
    }
}