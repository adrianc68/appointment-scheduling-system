using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<Model.AppointmentDbContext> context;

        public AccountRepository(IDbContextFactory<AppointmentDbContext> context)
        {
            this.context = context;
        }

        public async Task<bool> IsUsernameRegisteredAsync(string username)
        {
            using var dbContext = context.CreateDbContext();
            var usernameDB = await dbContext.UserAccounts
                .Where(a => a.Username!.ToLower() == username && a.Status != Model.Types.AccountStatusType.DELETED)
                .Select(a => a.Username)
                .FirstOrDefaultAsync();

            return usernameDB != null;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            using var dbContext = context.CreateDbContext();
            var emailDB = await dbContext.UserAccounts
                .Where(a => a.Email!.ToLower() == email && a.Status != Model.Types.AccountStatusType.DELETED)
                .Select(a => a.Email)
                .FirstOrDefaultAsync();

            return emailDB != null;
        }

        public async Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber)
        {
            using var dbContext = context.CreateDbContext();
            var phoneNumberDB = await dbContext.UserInformations
                .Where(a => a.PhoneNumber == phoneNumber && a.UserAccount!.Status != Model.Types.AccountStatusType.DELETED)
                .Select(a => a.PhoneNumber)
                .FirstOrDefaultAsync();

            return phoneNumberDB != null;
        }

        public async Task<BusinessLogicLayer.Model.AccountData?> GetAccountDataByEmailOrUsernameOrPhoneNumber(string account, string password)
        {
            using var dbContext = context.CreateDbContext();
            var userDB = await dbContext.UserAccounts
                .Include(a => a.UserInformation)
               .Where(a => (a.Email == account || a.Username == account || a.UserInformation!.PhoneNumber == account) && a.Password == password)
               .FirstOrDefaultAsync();

            if (userDB == null)
            {
                return null;
            }

            return new BusinessLogicLayer.Model.AccountData
            {
                Id = userDB.Id,
                Email = userDB.Email,
                Uuid = userDB.Uuid,
                PhoneNumber = userDB.UserInformation!.PhoneNumber,
                Username = userDB.Username,
                CreatedAt = userDB.CreatedAt,
                Role = (BusinessLogicLayer.Model.Types.RoleType)userDB.Role!.Value
            };
        }

        public async Task<int?> GetAccountIdByUuid(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var userDB = await dbContext.UserAccounts
               .Where(a => a.Uuid == uuid)
               .FirstOrDefaultAsync();

            if (userDB == null)
            {
                return null;
            }

            return userDB.Id!.Value;
        }

        public async Task<bool> ChangeAccountStatusType(int idAccount, BusinessLogicLayer.Model.Types.AccountStatusType status)
        {
            bool isStatusChanged = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var accountDb = await dbContext.UserAccounts
                     .FirstOrDefaultAsync(ac => ac.Id == idAccount);

                if (accountDb == null)
                {
                    return false;
                }

                accountDb!.Status = (Model.Types.AccountStatusType?)status;
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                isStatusChanged = true;
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return isStatusChanged;
        }

        public async Task<BusinessLogicLayer.Model.Types.RoleType?> GetRoleTypeByUuid(Guid accountUuid)
        {
            using var dbContext = context.CreateDbContext();
            var userDB = await dbContext.UserAccounts
               .Where(a => a.Uuid == accountUuid)
               .FirstOrDefaultAsync();

            if (userDB == null)
            {
                return null;
            }

            return (BusinessLogicLayer.Model.Types.RoleType?)userDB.Role;
        }

        public async Task<List<AccountData>> GetAllAccountData()
        {
            using var dbContext = context.CreateDbContext();

            var accountsData = await dbContext.UserAccounts
                .Include(e => e.UserInformation)
                .ToListAsync();

            if (accountsData == null)
            {
                return [];
            }

            List<AccountData> accounts =  accountsData.Select(e => new AccountData {
                Id = e.Id,
                Uuid = e.Uuid,
                Email = e.Email,
                PhoneNumber = e.UserInformation!.PhoneNumber,
                Username = e.Username,
                Role = (BusinessLogicLayer.Model.Types.RoleType?)e.Role,
                CreatedAt = e.CreatedAt
            }).ToList();

            return accounts;
        }
    }
}