using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDbContextFactory<Model.AppointmentDbContext> context;
        private readonly IPasswordHasherService passwordHasherService;
        public ClientRepository(IDbContextFactory<Model.AppointmentDbContext> context, IPasswordHasherService passwordHasherService)
        {
            this.context = context;
            this.passwordHasherService = passwordHasherService;
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
                    Password = passwordHasherService.HashPassword(client.Password!),
                    Username = client.Username,
                    Role = RoleType.CLIENT,
                    Uuid = client.Uuid,
                    Status = AccountStatusType.ENABLED
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

        public async Task<int?> GetClientIdByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var clientID = await dbContext.Clients
                .Where(a => a.UserAccount!.Uuid == uuid)
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
                  .ThenInclude(ua => ua!.UserInformation)
                  .Where(c => c.UserAccount!.Role == RoleType.CLIENT && c.UserAccount!.Status != AccountStatusType.DELETED)
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
                    Status = (BusinessLogicLayer.Model.Types.AccountStatusType)a.UserAccount.Status!.Value,
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
                     .ThenInclude(ua => ua!.UserInformation)
                 .FirstOrDefaultAsync(a => a.UserAccount!.Uuid == uuid);

            if (clientDB != null)
            {
                client = new BusinessLogicLayer.Model.Client
                {
                    Id = clientDB.UserAccount!.Id,
                    Name = clientDB.UserAccount.UserInformation!.Name,
                    PhoneNumber = clientDB.UserAccount.UserInformation.PhoneNumber,
                    Email = clientDB.UserAccount.Email,
                    Username = clientDB.UserAccount.Username,
                    CreatedAt = clientDB.UserAccount.CreatedAt,
                    Status = (BusinessLogicLayer.Model.Types.AccountStatusType?)clientDB.UserAccount!.Status,
                    Uuid = clientDB.UserAccount.Uuid
                };
            }
            return client;
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
                userAccount.UserInformation!.Name = client.Name;
                userAccount.UserInformation.PhoneNumber = client.PhoneNumber;
                // userAccount.Password = passwordHasherService.HashPassword(client.Password!);

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