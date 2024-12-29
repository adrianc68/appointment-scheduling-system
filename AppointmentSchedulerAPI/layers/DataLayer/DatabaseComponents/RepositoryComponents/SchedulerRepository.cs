using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class SchedulerRepository : ISchedulerRepository
    {
        private readonly IDbContextFactory<Model.AppointmentDbContext> context;

        public SchedulerRepository(IDbContextFactory<Model.AppointmentDbContext> context)
        {
            this.context = context;
        }

        public async Task<bool> AddAppointmentAsync(BusinessLogicLayer.Model.Appointment appointment)
        {
            bool isRegistered = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {

                var client = await dbContext.Clients
                .FirstOrDefaultAsync(c => c.UserAccount.Uuid == appointment.Client.Uuid);

                if (client == null)
                {
                    throw new Exception("Client not found");
                }

                var appointmentDB = new Appointment
                {
                    Date = appointment.Date,
                    EndTime = appointment.EndTime,
                    StartTime = appointment.StartTime,
                    TotalCost = appointment.TotalCost,
                    Uuid = appointment.Uuid,
                    Status = (Model.Types.AppointmentStatusType?)appointment.Status,
                    IdClient = client.IdUserAccount
                };
                dbContext.Appointments.Add(appointmentDB);
                await dbContext.SaveChangesAsync();

                foreach (var service in appointment.AssistantService)
                {
                    var serviceEntity = await dbContext.AssistantServices
                        .FirstOrDefaultAsync(s => s.Uuid == service.Uuid);

                    if (serviceEntity == null)
                    {
                        throw new Exception($"Service with UUID {service.Uuid} not found");
                    }

                    var appointmentAssistantService = new AppointmentAssistantService
                    {
                        IdAppointment = appointmentDB.Id,
                        IdAssistantService = serviceEntity.Id
                    };

                    dbContext.AppointmentAssistantServices.Add(appointmentAssistantService);
                }

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


        public async Task<bool> AddAvailabilityTimeSlotAsync(BusinessLogicLayer.Model.AvailabilityTimeSlot availabilityTimeSlot, Guid assistantUuid)
        {
            bool isRegistered = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var assistant = await dbContext.UserAccounts.FirstOrDefaultAsync(a => a.Uuid == assistantUuid);
                if (assistant == null)
                {
                    return false;
                }

                var timeSlot = new AvailabilityTimeSlot
                {
                    Uuid = availabilityTimeSlot.Uuid,
                    Date = availabilityTimeSlot.Date,
                    StartTime = availabilityTimeSlot.StartTime,
                    EndTime = availabilityTimeSlot.EndTime,
                    IdAssistant = assistant.Id
                };

                dbContext.AvailabilityTimeSlots.Add(timeSlot);
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

        public async Task<int?> GetAvailabilityTimeSlotIdByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var slotId = await dbContext.AvailabilityTimeSlots
                .Where(a => a.Uuid == uuid)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            return slotId;
        }

        public async Task<int?> GetAppointmentIdByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var appointmentId = await dbContext.Appointments
             .Where(a => a.Uuid == uuid)
             .Select(x => x.Id)
             .FirstOrDefaultAsync();
            return appointmentId;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.Appointment>> GetAppointmentsAsync(DateOnly startDate, DateOnly endDate)
        {
            using var dbContext = context.CreateDbContext();
            var appointmentDB = await dbContext.Appointments
                .Where(app => app.Date >= startDate && app.Date <= endDate)
                .Include(appAssSer => appAssSer.AppointmentAssistantServices)
                    .ThenInclude(assisServ => assisServ.AssistantService)
                        .ThenInclude(assis => assis.Assistant)
                            .ThenInclude(asacc => asacc.UserAccount)
                .Include(client => client.Client)
                    .ThenInclude(clientAcc => clientAcc.UserAccount)
                    .ThenInclude(clientaccinf => clientaccinf.UserInformation)
                .Include(appAssSer => appAssSer.AppointmentAssistantServices)
                    .ThenInclude(serv => serv.AssistantService)
                    .ThenInclude(assService => assService.Service)
                .ToListAsync();

            PropToString.PrintData(appointmentDB[0].AppointmentAssistantServices.First().AssistantService);


            var appointmentsModel = appointmentDB.Select(app => new BusinessLogicLayer.Model.Appointment
            {
                Date = app.Date,
                EndTime = app.EndTime,
                StartTime = app.StartTime,
                TotalCost = app.TotalCost,
                Uuid = app.Uuid,
                Id = app.Id,
                CreatedAt = app.CreatedAt,

                Client = new BusinessLogicLayer.Model.Client
                {
                    Name = app.Client.UserAccount.UserInformation.Name,
                    Uuid = app.Client.UserAccount.Uuid,
                    Id = app.Client.IdUserAccount,
                },
                AssistantService = app.AppointmentAssistantServices.Select(aas => new BusinessLogicLayer.Model.Service
                {
                    Id = aas.AssistantService.Service.Id,
                    Description = aas.AssistantService.Service.Description,
                    Minutes = aas.AssistantService.Service.Minutes,
                    // Warning! we are returning the assistantservice uuid instead of the service uuid
                    // This is intended to simplify communication and validation of selected services between the client and backend.
                    Uuid = aas.AssistantService.Uuid,
                    CreatedAt = aas.AssistantService.Service.CreatedAt,
                    Status = (BusinessLogicLayer.Model.Types.ServiceStatusType?)aas.AssistantService.Service.Status,
                    Name = aas.AssistantService.Service.Name,
                    Price = aas.AssistantService.Service.Price,
                }).ToList()
            }).ToList();
            return appointmentsModel;
        }


        public async Task<IEnumerable<BusinessLogicLayer.Model.AvailabilityTimeSlot>> GetAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate)
        {
            using var dbContext = context.CreateDbContext();
            var availableServices = await dbContext.AvailabilityTimeSlots
                .Where(slot => slot.Date >= startDate && slot.Date <= endDate)
                    .Include(a => a.Assistant)
                        .ThenInclude(ass => ass.UserAccount)
                        .ThenInclude(assc => assc.UserInformation)
                .ToListAsync();

            var availabilityTimeSlotsModel = availableServices
                .Select(slot => new BusinessLogicLayer.Model.AvailabilityTimeSlot
                {
                    Id = slot.Id,
                    Uuid = slot.Uuid,
                    Date = slot.Date,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    Assistant = new BusinessLogicLayer.Model.Assistant
                    {
                        Name = slot.Assistant.UserAccount.UserInformation.Name,
                        Uuid = slot.Assistant.UserAccount.Uuid,
                        Id = slot.Assistant.IdUserAccount
                    }
                })
                .ToList();

            return availabilityTimeSlotsModel;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.AssistantService>> GetAvailableServicesAsync(DateOnly date)
        {
            using var dbContext = context.CreateDbContext();
            var availableServices = await dbContext.AvailabilityTimeSlots
                .Where(slot => slot.Date == date)
                .Include(slot => slot.Assistant)
                    .ThenInclude(assistant => assistant.UserAccount)
                    .ThenInclude(userAccount => userAccount.UserInformation)
                .Include(slot => slot.Assistant)
                    .ThenInclude(assistant => assistant.AssistantServices)
                        .ThenInclude(asService => asService.Service)
                .ToListAsync();

            var businessLogicAssistantServices = availableServices
                    .GroupBy(slot => slot.Assistant?.IdUserAccount)
                    .Select(group => new BusinessLogicLayer.Model.AssistantService
                    {
                        Assistant = new BusinessLogicLayer.Model.Assistant
                        {
                            Id = group.Key,
                            Uuid = group.FirstOrDefault()?.Assistant?.UserAccount?.Uuid,
                            Name = group.FirstOrDefault()?.Assistant?.UserAccount?.UserInformation?.Name
                        },
                        Services = group
                            .SelectMany(slot => slot.Assistant?.AssistantServices?.Select(asService => new BusinessLogicLayer.Model.Service
                            {
                                Id = asService.IdService,
                                Name = asService.Service?.Name,
                                Price = asService.Service?.Price,
                                Minutes = asService.Service?.Minutes,
                                // warning! we are returning the assistantservice uuid instead of the service uuid
                                // This is intended to simplify communication and validation of selected services between the client and backend.
                                Uuid = asService.Uuid
                            }) ?? new List<BusinessLogicLayer.Model.Service>())
                            .GroupBy(service => new { service.Name, service.Price, service.Minutes })
                            .Select(groupedService => groupedService.First())
                            .ToList()
                    })
                    .Where(assistantService => assistantService.Services.Any())
                    .ToList();

            return businessLogicAssistantServices;
        }

        public async Task<bool> IsTimeSlotAvailable(BusinessLogicLayer.Model.Types.DateTimeRange range)
        {
            using var dbContext = context.CreateDbContext();
            bool isAvailable = await dbContext.Appointments
                .Where(a => a.Date == range.Date && a.StartTime < range.EndTime && a.EndTime > range.StartTime) 
                .AnyAsync();

            return !isAvailable;
        }
    }
}