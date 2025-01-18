using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface IAccountRepository
    {
        Task<bool> IsUsernameRegisteredAsync(string username);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
        Task<bool> ChangeAccountStatusType(int idAccount, AccountStatusType status);
        Task<RoleType?> GetRoleTypeByUuid(Guid accountUuid);
        Task<AccountData?> GetAccountDataByEmailOrUsernameOrPhoneNumber(string account, string password);
        Task<int?> GetAccountIdByUuid(Guid uuid);
        Task<List<AccountData>> GetAllAccountData();
    }
}