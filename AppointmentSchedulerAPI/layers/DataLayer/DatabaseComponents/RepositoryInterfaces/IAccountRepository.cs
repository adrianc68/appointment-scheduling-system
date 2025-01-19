using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IAccountRepository
    {
        Task<bool> ChangeAccountStatusTypeAsync(int idAccount, AccountStatusType status);
        Task<RoleType?> GetRoleTypeByUuidAsync(Guid accountUuid);
        Task<AccountData?> GetAccountDataByEmailOrUsernameOrPhoneNumberAsync(string account, string password);
        Task<int?> GetAccountIdByUuidAsync(Guid uuid);
        Task<List<AccountData>> GetAllAccountDataAsync();
        Task<bool> IsUsernameRegisteredAsync(string username);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
    }
}