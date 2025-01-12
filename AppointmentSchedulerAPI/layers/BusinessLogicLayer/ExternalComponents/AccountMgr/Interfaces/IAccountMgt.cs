using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.AccountMgr.Types;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.AccountMgr.Interfaces
{
    public interface IAccountMgt
    {
        Task<bool> IsUsernameRegisteredAsync(string username);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber);
        Task<AccountData?> GetAccountDataByEmailOrUsernameOrPhoneNumber(string account, string password);
        Task<bool> ChangeAccountStatusAsync(int idAccount, AccountStatusType status, AccountType accountType);
    }
}