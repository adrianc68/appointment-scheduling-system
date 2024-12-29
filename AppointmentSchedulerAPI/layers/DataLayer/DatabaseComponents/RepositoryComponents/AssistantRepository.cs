using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class AssistantRepository : IAssistantRepository
    {
        private readonly IDbContextFactory<Model.AppointmentDbContext> context;
        public AssistantRepository(IDbContextFactory<AppointmentDbContext> context)
        {
            this.context = context;
        }

        public async Task<bool> AddServicesToAssistantAsync(Guid assistantUuid, List<Guid?> servicesUuid)
        {
            bool isRegistered = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var assistantId = await dbContext.Assistants
                    .Where(a => a.UserAccount.Uuid == assistantUuid)
                    .Select(a => a.IdUserAccount)
                    .FirstOrDefaultAsync();

                if (assistantId == null)
                {
                    return false;
                }

                var serviceIds = await dbContext.Services
                    .Where(s => servicesUuid.Contains(s.Uuid))
                    .Select(s => s.Id)
                    .ToListAsync();

                if (!serviceIds.Any())
                {
                    return false;
                }

                var assistantServices = serviceIds.Select(serviceId => new AssistantService
                {
                    IdAssistant = assistantId,
                    IdService = serviceId,
                    Uuid = Guid.CreateVersion7()
                }).ToList();

                dbContext.AssistantServices.AddRange(assistantServices);
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                isRegistered = true;
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return isRegistered;
        }

        public async Task<BusinessLogicLayer.Model.Assistant?> GetAssistantByUuidAsync(Guid uuid)
        {
            BusinessLogicLayer.Model.Assistant? assistant = null;
            using var dbContext = context.CreateDbContext();
            var assistantDB = await dbContext.Assistants
                 .Include(a => a.UserAccount)
                     .ThenInclude(ua => ua.UserInformation)
                 .FirstOrDefaultAsync(a => a.UserAccount.Uuid == uuid);

            if (assistantDB != null)
            {
                assistant = new BusinessLogicLayer.Model.Assistant
                {
                    Id = assistantDB.UserAccount.Id,
                    Name = assistantDB.UserAccount.UserInformation.Name,
                    PhoneNumber = assistantDB.UserAccount.UserInformation.PhoneNumber,
                    Email = assistantDB.UserAccount.Email,
                    Username = assistantDB.UserAccount.Username,
                    CreatedAt = assistantDB.UserAccount.CreatedAt,
                    Status = (BusinessLogicLayer.Model.Types.AssistantStatusType?)assistantDB.Status,
                    Uuid = assistantDB.UserAccount.Uuid
                };
            }
            return assistant;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.Assistant>> GetAllAssistantsAsync()
        {
            IEnumerable<BusinessLogicLayer.Model.Assistant> businessAssistants = [];
            using var dbContext = context.CreateDbContext();
            var assistantsDB = await dbContext.Assistants
              .Include(a => a.UserAccount)
                  .ThenInclude(ua => ua.UserInformation)
                  .Where(c => c.UserAccount.Role == RoleType.ASSISTANT)
              .ToListAsync();

            businessAssistants = assistantsDB
                .Where(a => a.UserAccount != null && a.UserAccount.UserInformation != null)
                .Select(a => new BusinessLogicLayer.Model.Assistant
                {
                    Id = a.IdUserAccount,
                    Uuid = a.UserAccount!.Uuid,
                    Email = a.UserAccount.Email!,
                    Name = a.UserAccount.UserInformation!.Name!,
                    PhoneNumber = a.UserAccount.UserInformation.PhoneNumber!,
                    Username = a.UserAccount.Username!,
                    Status = (BusinessLogicLayer.Model.Types.AssistantStatusType)a.Status,
                    CreatedAt = a.UserAccount.CreatedAt
                })
                .ToList();
            return businessAssistants;
        }

        public async Task<bool> AddAssistantAsync(BusinessLogicLayer.Model.Assistant assistant)
        {
            bool isRegistered = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var userAccount = new UserAccount
                {
                    Email = assistant.Email,
                    Password = assistant.Password,
                    Username = assistant.Username,
                    Role = RoleType.ASSISTANT,
                    Uuid = assistant.Uuid
                };
                dbContext.UserAccounts.Add(userAccount);
                await dbContext.SaveChangesAsync();

                var userInformation = new UserInformation
                {
                    Name = assistant.Name,
                    PhoneNumber = assistant.PhoneNumber,
                    Filepath = null,
                    IdUser = userAccount.Id
                };

                dbContext.UserInformations.Add(userInformation);
                await dbContext.SaveChangesAsync();

                var newAssistant = new Assistant
                {
                    IdUserAccount = userAccount.Id,
                    Status = AssistantStatusType.ENABLED
                };

                dbContext.Assistants.Add(newAssistant);
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

        public async Task<List<BusinessLogicLayer.Model.Service>> GetServicesAssignedToAssistantByUuidAsync(Guid uuid)
        {
            IEnumerable<BusinessLogicLayer.Model.Service> businessService = [];
            using var dbContext = context.CreateDbContext();
            var assistantDB = await dbContext.Assistants
                .Include(a => a.AssistantServices)
                    .ThenInclude(ase => ase.Service)
                .FirstOrDefaultAsync(a => a.UserAccount.Uuid == uuid);

            if (assistantDB == null)
            {
                return [];
            }

            businessService = assistantDB.AssistantServices
                .Where(ase => ase.Service != null)
                .Select(ase => new BusinessLogicLayer.Model.Service
                {
                    Id = ase.Service.Id,
                    Name = ase.Service.Name,
                    Description = ase.Service.Description,
                    Minutes = ase.Service.Minutes,
                    Price = ase.Service.Price,
                    Uuid = ase.Service.Uuid,
                    CreatedAt = ase.Service.CreatedAt,
                    Status = (BusinessLogicLayer.Model.Types.ServiceStatusType?)ase.Service.Status
                })
                .ToList();

            return (List<BusinessLogicLayer.Model.Service>)businessService;
        }

        public async Task<int?> GetAssistantIdByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var assistantId = await dbContext.Assistants
                .Where(a => a.UserAccount.Uuid == uuid)
                .Select(a => a.IdUserAccount)
                .FirstOrDefaultAsync();
            return assistantId;
        }

        public async Task<bool> IsUsernameRegisteredAsync(string username)
        {
            using var dbContext = context.CreateDbContext();
            var usernameDB = await dbContext.UserAccounts
                .Where(a => a.Username.ToLower() == username)
                .Select(a => a.Username)
                .FirstOrDefaultAsync();

            return usernameDB != null;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            using var dbContext = context.CreateDbContext();
            var emailDB = await dbContext.UserAccounts
                .Where(a => a.Email.ToLower() == email)
                .Select(a => a.Email)
                .FirstOrDefaultAsync();

            return emailDB != null;
        }

        public async Task<bool> IsPhoneNumberRegisteredAsync(string phoneNumber)
        {
            using var dbContext = context.CreateDbContext();
            var phoneNumberDB = await dbContext.UserInformations
                .Where(a => a.PhoneNumber == phoneNumber)
                .Select(a => a.PhoneNumber)
                .FirstOrDefaultAsync();

            return phoneNumberDB != null;
        }

        public async Task<int?> GetServiceIdByAssistantServiceUuid(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var serviceId = await dbContext.AssistantServices
                .Where(a => a.Service.Uuid == uuid)
                .Select(a => a.Service.Id)
                .FirstOrDefaultAsync();
            return serviceId;
        }
    }
}