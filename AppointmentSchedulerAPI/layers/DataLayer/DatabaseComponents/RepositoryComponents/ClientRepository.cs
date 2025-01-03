using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDbContextFactory<Model.AppointmentDbContext> context;
        public ClientRepository(IDbContextFactory<Model.AppointmentDbContext> context)
        {
            this.context = context;
        }

        public async Task<bool> AddClientAsync(BusinessLogicLayer.Model.Client client)
        {
            bool isRegistered = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var userAccount = new UserAccount
                {
                    Email = client.Email,
                    Password = client.Password,
                    Username = client.Username,
                    Role = RoleType.CLIENT,
                    Uuid = Guid.CreateVersion7()
                };
                dbContext.UserAccounts.Add(userAccount);
                await dbContext.SaveChangesAsync();

                var userInformation = new UserInformation
                {
                    Name = client.Name,
                    PhoneNumber = client.PhoneNumber,
                    Filepath = null,
                    IdUser = userAccount.Id
                };

                dbContext.UserInformations.Add(userInformation);
                await dbContext.SaveChangesAsync();

                var newClient = new Client
                {
                    IdUserAccount = userAccount.Id,
                    Status = ClientStatusType.ENABLED
                };

                dbContext.Clients.Add(newClient);
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                isRegistered = true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return isRegistered;
        }

        public async Task<bool> IsUsernameRegisteredAsync(string username)
        {
            using var dbContext = context.CreateDbContext();
            var usernameDB = await dbContext.UserAccounts
                .Where(a => a.Username!.ToLower() == username && a.Client!.Status != ClientStatusType.DELETED)
                .Select(a => a.Username)
                .FirstOrDefaultAsync();

            return usernameDB != null;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            using var dbContext = context.CreateDbContext();
            var emailDB = await dbContext.UserAccounts
                .Where(a => a.Email!.ToLower() == email && a.Client!.Status != ClientStatusType.DELETED)
                .Select(a => a.Email)
                .FirstOrDefaultAsync();

            return emailDB != null;
        }

        public async Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber)
        {
            using var dbContext = context.CreateDbContext();
            var phoneNumberDB = await dbContext.UserInformations
                .Where(a => a.PhoneNumber == phoneNumber && a.UserAccount!.Client!.Status != ClientStatusType.DELETED)
                .Select(a => a.PhoneNumber)
                .FirstOrDefaultAsync();

            return phoneNumberDB != null;
        }

        public async Task<int?> GetClientIdByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var clientID = await dbContext.Clients
                .Where(a => a.UserAccount.Uuid == uuid)
                .Select(a => a.IdUserAccount)
                .FirstOrDefaultAsync();
            return clientID;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.Client>> GetAllClientsAsync()
        {
            IEnumerable<BusinessLogicLayer.Model.Client> businessClient = [];
            using var dbContext = context.CreateDbContext();
            var clientsDB = await dbContext.Clients
                .Include(a => a.UserAccount)
                  .ThenInclude(ua => ua.UserInformation)
                  .Where(c => c.UserAccount.Role == RoleType.CLIENT && c.Status != ClientStatusType.DELETED)
              .ToListAsync();

            businessClient = clientsDB
                .Where(a => a.UserAccount != null && a.UserAccount.UserInformation != null)
                .Select(a => new BusinessLogicLayer.Model.Client
                {
                    Id = a.IdUserAccount,
                    Uuid = a.UserAccount!.Uuid,
                    Email = a.UserAccount.Email!,
                    Name = a.UserAccount.UserInformation!.Name!,
                    PhoneNumber = a.UserAccount.UserInformation.PhoneNumber!,
                    Username = a.UserAccount.Username!,
                    Status = (BusinessLogicLayer.Model.Types.ClientStatusType)a.Status,
                    CreatedAt = a.UserAccount.CreatedAt
                })
                .ToList();
            return businessClient;
        }

        public async Task<BusinessLogicLayer.Model.Client?> GetClientByUuidAsync(Guid uuid)
        {
            BusinessLogicLayer.Model.Client? client = null;
            using var dbContext = context.CreateDbContext();
            var clientDB = await dbContext.Clients
                 .Include(a => a.UserAccount)
                     .ThenInclude(ua => ua.UserInformation)
                 .FirstOrDefaultAsync(a => a.UserAccount.Uuid == uuid);

            if (clientDB != null)
            {
                client = new BusinessLogicLayer.Model.Client
                {
                    Id = clientDB.UserAccount.Id,
                    Name = clientDB.UserAccount.UserInformation.Name,
                    PhoneNumber = clientDB.UserAccount.UserInformation.PhoneNumber,
                    Email = clientDB.UserAccount.Email,
                    Username = clientDB.UserAccount.Username,
                    CreatedAt = clientDB.UserAccount.CreatedAt,
                    Status = (BusinessLogicLayer.Model.Types.ClientStatusType?)clientDB.Status,
                    Uuid = clientDB.UserAccount.Uuid
                };
            }
            return client;
        }

        public async Task<bool> ChangeClientStatusTypeAsync(int idClient, BusinessLogicLayer.Model.Types.ClientStatusType status)
        {
            bool isStatusChanged = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var clientDb = await dbContext.Clients
                     .FirstOrDefaultAsync(ac => ac.UserAccount.Id == idClient);

                if (clientDb == null)
                {
                    return false;
                }

                clientDb.Status = (ClientStatusType)status;
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

        public async Task<bool> UpdateClientAsync(BusinessLogicLayer.Model.Client client)
        {
             bool isUpdated = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var userAccount = await dbContext.UserAccounts
                    .Include(ua => ua.UserInformation)
                    .FirstOrDefaultAsync(ua => ua.Uuid == client.Uuid);

                if (userAccount == null)
                {
                    return false;
                }

                userAccount.Email = client.Email;
                userAccount.Username = client.Username;
                userAccount.Role = RoleType.CLIENT;
                userAccount.UserInformation.Name = client.Name;
                userAccount.UserInformation.PhoneNumber = client.PhoneNumber;

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                isUpdated = true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            return isUpdated;
        }
    }
}