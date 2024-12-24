using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class ClientRepository : IClientRepository
    {
        private readonly Model.AppointmentDbContext context;
        public ClientRepository(Model.AppointmentDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.Client>> GetAllClientsAsync()
        {
            IEnumerable<BusinessLogicLayer.Model.Client> businessClient = [];
            var clientsDB = await context.Clients
                .Include(a => a.UserAccount)
                  .ThenInclude(ua => ua.UserInformation)
                  .Where(c => c.UserAccount.Role == RoleType.CLIENT)
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


        public async Task<bool> RegisterClientAsync(BusinessLogicLayer.Model.Client client)
        {
            bool isRegistered = false;
            using var transaction = await context.Database.BeginTransactionAsync();
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
                context.UserAccounts.Add(userAccount);
                await context.SaveChangesAsync();

                var userInformation = new UserInformation
                {
                    Name = client.Name,
                    PhoneNumber = client.PhoneNumber,
                    Filepath = null,
                    IdUser = userAccount.Id
                };

                context.UserInformations.Add(userInformation);
                await context.SaveChangesAsync();

                var newClient = new Client
                {
                    IdUserAccount = userAccount.Id,
                    Status = ClientStatusType.ENABLED
                };

                context.Clients.Add(newClient);
                await context.SaveChangesAsync();
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
    }
}