using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class AccountMgr : IAccountMgt, IAccountEvent<AssistantEvent>, IAccountEvent<ClientEvent>
    {
        private readonly IAccountRepository accountRepository;
        private static readonly List<IAccountObserver<ClientEvent>> clientObservers = new();
        private static readonly List<IAccountObserver<AssistantEvent>> assistantObservers = new();

        public AccountMgr(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<bool> ChangeAccountStatusAsync(int idAccount, AccountStatusType status, AccountType accountType)
        {
            bool isSuccesful = await accountRepository.ChangeAccountStatusTypeAsync(idAccount, status);
            if (isSuccesful)
            {
                if (accountType == AccountType.ASSISTANT)
                {
                    var eventTypeMapping = new Dictionary<AccountStatusType, AssistantEventType>
                    {
                        { AccountStatusType.ENABLED, AssistantEventType.ENABLED },
                        { AccountStatusType.DISABLED, AssistantEventType.DISABLED },
                        { AccountStatusType.DELETED, AssistantEventType.DELETED }
                    };

                    AssistantEvent assistantEvent = new AssistantEvent
                    {
                        AssistantId = idAccount,
                        EventType = eventTypeMapping.ContainsKey(status) ? eventTypeMapping[status] : AssistantEventType.UPDATED
                    };

                    NotifySubscribers(assistantEvent);
                }
                else if (accountType == AccountType.CLIENT)
                {
                    var eventTypeMapping = new Dictionary<AccountStatusType, ClientEventType>
                    {
                        { AccountStatusType.ENABLED, ClientEventType.ENABLED },
                        { AccountStatusType.DISABLED, ClientEventType.DISABLED },
                        { AccountStatusType.DELETED, ClientEventType.DELETED }
                    };

                    ClientEvent clientEvent = new ClientEvent
                    {
                        ClientId = idAccount,
                        EventType = eventTypeMapping.ContainsKey(status) ? eventTypeMapping[status] : ClientEventType.UPDATED
                    };

                    NotifySubscribers(clientEvent);
                }
            }
            return isSuccesful;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            bool isEmailRegistered = await accountRepository.IsEmailRegisteredAsync(email);
            return isEmailRegistered;
        }

        public async Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber)
        {
            bool isPhoneNumberRegistered = await accountRepository.IsPhoneNumberRegisteredAsync(phoneNumber);
            return isPhoneNumberRegistered;
        }

        public async Task<bool> IsUsernameRegisteredAsync(string username)
        {
            bool isUsernameRegistered = await accountRepository.IsUsernameRegisteredAsync(username);
            return isUsernameRegistered;
        }

        public async Task<AccountData?> GetAccountDataByEmailOrUsernameOrPhoneNumberAsync(string account, string password)
        {
            AccountData? result = await accountRepository.GetAccountDataByEmailOrUsernameOrPhoneNumberAsync(account, password);
            return result;
        }

        public async Task<List<AccountData>> GetAllAccountsDataAsync()
        {
            List<AccountData> accountsData = await accountRepository.GetAllAccountDataAsync();
            return accountsData;
        }

        public async Task<int?> GetAccountIdByUuidAsync(Guid uuid)
        {
            int? id = await accountRepository.GetAccountIdByUuidAsync(uuid);
            return id;
        }

        public async Task<RoleType?> GetRoleTypeByUuidAsync(Guid accountUuid)
        {
            RoleType? role = await accountRepository.GetRoleTypeByUuidAsync(accountUuid);
            return role;
        }

        public void NotifySubscribers(AssistantEvent eventType)
        {
            foreach (var observer in assistantObservers)
            {
                observer.UpdateOnAccountChanged(eventType);
            }
        }

        public void Subscribe(IAccountObserver<AssistantEvent> observer)
        {
            if (!assistantObservers.Contains(observer))
            {
                assistantObservers.Add(observer);
            }
        }

        public void Unsubscribe(IAccountObserver<AssistantEvent> observer)
        {
            if (assistantObservers.Contains(observer))
            {
                assistantObservers.Remove(observer);
            }
        }

        public void NotifySubscribers(ClientEvent eventType)
        {
            foreach (var observer in clientObservers)
            {
                observer.UpdateOnAccountChanged(eventType);
            }
        }

        public void Subscribe(IAccountObserver<ClientEvent> observer)
        {
            if (!clientObservers.Contains(observer))
            {
                clientObservers.Add(observer);
            }
        }

        public void Unsubscribe(IAccountObserver<ClientEvent> observer)
        {
            if (clientObservers.Contains(observer))
            {
                clientObservers.Remove(observer);
            }
        }

    }
}