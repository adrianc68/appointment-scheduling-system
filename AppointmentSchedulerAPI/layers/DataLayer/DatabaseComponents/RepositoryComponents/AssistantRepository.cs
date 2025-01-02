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

        public async Task<bool> AddServicesToAssistantAsync(int idAssistant, List<int> idServices)
        {
            bool isRegistered = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var assistantServices = idServices.Select(serviceId => new ServiceOffer
                {
                    IdAssistant = idAssistant,
                    IdService = serviceId,
                    Uuid = Guid.CreateVersion7()
                }).ToList();

                dbContext.ServiceOffers.AddRange(assistantServices);
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

        public async Task<bool> IsUsernameRegisteredAsync(string username)
        {
            using var dbContext = context.CreateDbContext();
            var usernameDB = await dbContext.UserAccounts
                .Where(a => a.Username!.ToLower() == username)
                .Select(a => a.Username)
                .FirstOrDefaultAsync();

            return usernameDB != null;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            using var dbContext = context.CreateDbContext();
            var emailDB = await dbContext.UserAccounts
                .Where(a => a.Email!.ToLower() == email)
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

        public async Task<int?> GetAssistantIdByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var assistantId = await dbContext.Assistants
                .Where(a => a.UserAccount.Uuid == uuid)
                .Select(a => a.IdUserAccount)
                .FirstOrDefaultAsync();
            return assistantId;
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

        public async Task<int?> GetServiceIdByAssistantServiceUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var serviceId = await dbContext.ServiceOffers
                .Where(a => a.Uuid == uuid)
                .Select(a => a.Service.Id)
                .FirstOrDefaultAsync();
            return serviceId;
        }

        public async Task<List<BusinessLogicLayer.Model.Service>> GetServicesAssignedToAssistantByUuidAsync(Guid uuid)
        {
            IEnumerable<BusinessLogicLayer.Model.Service> businessService = [];
            using var dbContext = context.CreateDbContext();
            var assistantDB = await dbContext.Assistants
                .Include(a => a.ServiceOffers)
                    .ThenInclude(ase => ase.Service)
                .FirstOrDefaultAsync(a => a.UserAccount.Uuid == uuid);

            if (assistantDB == null)
            {
                return [];
            }

            businessService = assistantDB.ServiceOffers
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

        public async Task<BusinessLogicLayer.Model.ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();

            var dbServiceOffer = await dbContext.ServiceOffers
                .Include(a => a.Service)
                .Include(a => a.Assistant)
                    .ThenInclude(asi => asi.UserAccount)
                    .ThenInclude(asc => asc.UserInformation)
                .FirstOrDefaultAsync(a => a.Uuid == uuid);

            if (dbServiceOffer == null)
                return null;

            return new BusinessLogicLayer.Model.ServiceOffer
            {
                Service = new BusinessLogicLayer.Model.Service
                {
                    Id = dbServiceOffer.Service.Id,
                    Name = dbServiceOffer.Service.Name,
                    Description = dbServiceOffer.Service.Description,
                    Minutes = dbServiceOffer.Service.Minutes,
                    Price = dbServiceOffer.Service.Price,
                    Uuid = dbServiceOffer.Service.Uuid,
                    CreatedAt = dbServiceOffer.Service.CreatedAt,
                    Status = (BusinessLogicLayer.Model.Types.ServiceStatusType?)dbServiceOffer.Service.Status
                },
                Id = dbServiceOffer.Id,
                Uuid = dbServiceOffer.Uuid,
                Assistant = new BusinessLogicLayer.Model.Assistant
                {
                    Id = dbServiceOffer.Assistant.IdUserAccount,
                    Uuid = dbServiceOffer.Assistant.UserAccount!.Uuid,
                    Email = dbServiceOffer.Assistant.UserAccount.Email!,
                    Name = dbServiceOffer.Assistant.UserAccount.UserInformation!.Name!,
                    PhoneNumber = dbServiceOffer.Assistant.UserAccount.UserInformation.PhoneNumber!,
                    Username = dbServiceOffer.Assistant.UserAccount.Username!,
                    Status = (BusinessLogicLayer.Model.Types.AssistantStatusType)dbServiceOffer.Assistant.Status,
                    CreatedAt = dbServiceOffer.Assistant.UserAccount.CreatedAt
                }
            };
        }

        public async Task<bool> IsAssistantOfferingServiceByUuidAsync(int idService, int idAssistant)
        {
            using var dbContext = context.CreateDbContext();
            var serviceOfferDB = await dbContext.ServiceOffers
                .Where(a => a.IdService == idService && a.IdAssistant == idAssistant)
                .FirstOrDefaultAsync();

            return serviceOfferDB != null;
        }

        public async Task<bool> ChangeAssistantStatus(int idAssistant, BusinessLogicLayer.Model.Types.AssistantStatusType status)
        {
            bool isStatusChanged = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var assistantDb = await dbContext.Assistants
                     .FirstOrDefaultAsync(ac => ac.UserAccount.Id == idAssistant);

                if (assistantDb == null)
                {
                    return false;
                }

                assistantDb.Status = (AssistantStatusType)status;
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
    }
}