using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface IAccountMgt
    {
        Task<bool> IsUsernameRegisteredAsync(string username);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
        Task<List<(int, Guid)>> GetAllAccountsIdsAndUuids();
        Task<AccountData?> GetAccountDataByEmailOrUsernameOrPhoneNumber(string account, string password);
        Task<int?> GetAccountIdByUuid(Guid uuid);
        Task<RoleType?> GetRoleTypeByUuid(Guid accountUuid);
        Task<bool> ChangeAccountStatusAsync(int idAccount, AccountStatusType status, AccountType accountType);
    }
}