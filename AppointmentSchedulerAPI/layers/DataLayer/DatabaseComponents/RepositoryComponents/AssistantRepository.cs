using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class AssistantRepository : IAssistantRepository
    {
        private readonly Model.AppointmentDbContext context;
        public AssistantRepository(AppointmentDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> AssignServicesToAssistant(Guid assistantUuid, List<Guid?> servicesUuid)
        {
            bool isRegistered = false;
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var assistantId = await context.Assistants
                    .Where(a => a.UserAccount.Uuid == assistantUuid)
                    .Select(a => a.IdUserAccount)
                    .FirstOrDefaultAsync();

                if (assistantId == 0)
                {
                    return false;
                }

                var serviceIds = await context.Services
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
                    IdService = serviceId
                }).ToList();

                context.AssistantServices.AddRange(assistantServices);
                await context.SaveChangesAsync();
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

        public bool ChangeAssistantStatus(int idAssistant, BusinessLogicLayer.Model.Types.AssistantStatusType status)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAssistantAsync(Guid uuid)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.Assistant>> GetAllAssistantsAsync()
        {
            IEnumerable<BusinessLogicLayer.Model.Assistant> businessAssistants = [];
            var assistantsDB = await context.Assistants
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

        public Task<BusinessLogicLayer.Model.Assistant?> GetAssistantByUuidAsync(Guid uuid)
        {
            throw new NotImplementedException();
        }

        public BusinessLogicLayer.Model.Types.AssistantStatusType GetAssistantStatus(int idAssistant)
        {
            throw new NotImplementedException();
        }

        public bool GetServicesAssignedToAssistant(int idAssistant)
        {
            throw new NotImplementedException();
        }

        public bool IsAssistantRegistered(BusinessLogicLayer.Model.Assistant assistant)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RegisterAssistantAsync(BusinessLogicLayer.Model.Assistant assistant)
        {
            bool isRegistered = false;
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var userAccount = new UserAccount
                {
                    Email = assistant.Email,
                    Password = assistant.Password,
                    Username = assistant.Username,
                    Role = RoleType.ASSISTANT,
                    Uuid = Guid.CreateVersion7()
                };
                context.UserAccounts.Add(userAccount);
                await context.SaveChangesAsync();

                var userInformation = new UserInformation
                {
                    Name = assistant.Name,
                    PhoneNumber = assistant.PhoneNumber,
                    Filepath = null,
                    IdUser = userAccount.Id
                };

                context.UserInformations.Add(userInformation);
                await context.SaveChangesAsync();

                var newAssistant = new Assistant
                {
                    IdUserAccount = userAccount.Id,
                    Status = AssistantStatusType.ENABLED
                };

                context.Assistants.Add(newAssistant);
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

        public bool UpdateAssistant(int idAssistant, BusinessLogicLayer.Model.Assistant assistant)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAssistantAsync(BusinessLogicLayer.Model.Assistant assistant)
        {
            throw new NotImplementedException();
        }
    }
}