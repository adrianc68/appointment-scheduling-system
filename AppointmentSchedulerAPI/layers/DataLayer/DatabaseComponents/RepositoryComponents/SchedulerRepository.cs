using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class SchedulerRepository : ISchedulerRepository
    {
        private readonly Model.AppointmentDbContext context;

        public SchedulerRepository(Model.AppointmentDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.AssistantService>> GetAvailableServicesAsync(DateOnly date)
        {
            var availableServices = await context.AvailabilityTimeSlots
                .Where(slot => slot.Date == date)
                .Include(slot => slot.Assistant)
                    .ThenInclude(assistant => assistant.UserAccount)
                    .ThenInclude(assistant => assistant.UserInformation)
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
                        Id = (int)group.Key,
                        Uuid = group.FirstOrDefault()?.Assistant.UserAccount.Uuid,
                        Name = group.FirstOrDefault()?.Assistant?.UserAccount?.UserInformation?.Name  
                    },
                    Services = group
                        .SelectMany(slot => slot.Assistant?.AssistantServices?.Select(asService => new BusinessLogicLayer.Model.Service
                        {
                            Id = asService.IdService,
                            Name = asService.Service?.Name,
                            Price = asService.Service?.Price,
                            Minutes = asService.Service?.Minutes,
                            Uuid = asService.Service?.Uuid
                        }) ?? new List<BusinessLogicLayer.Model.Service>())
                        .GroupBy(service => new { service.Name, service.Price, service.Minutes })
                        .Select(groupedService => groupedService.First())
                        .ToList() 
                })
                .ToList();
            return businessLogicAssistantServices;
        }

        public async Task<bool> RegisterAvailabilityTimeSlot(BusinessLogicLayer.Model.AvailabilityTimeSlot availabilityTimeSlot, Guid assistantUuid)
        {
            bool isRegistered = false;
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var assistant = await context.UserAccounts.FirstOrDefaultAsync(a => a.Uuid == assistantUuid);
                if (assistant == null)
                {
                    return false;
                }

                var timeSlot = new AvailabilityTimeSlot
                {
                    Uuid = Guid.NewGuid(),
                    Date = availabilityTimeSlot.Date,
                    StartTime = availabilityTimeSlot.StartTime,
                    EndTime = availabilityTimeSlot.EndTime,
                    IdAssistant = assistant.Id
                };

                context.AvailabilityTimeSlots.Add(timeSlot);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return isRegistered;
        }

        // public bool AreServicesAvailable(List<int> services, DateTimeRange range)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool BlockTimeRange(DateTimeRange range)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool ChangeAppointmentStatus(int idAppointment, AppointmentStatusType status)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool DeleteAssistantAppointments(int idAssistant)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool DeleteAssistantAvailabilityTimeSlots(int idAssistant)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool DeleteAvailabilityTimeSlot(int idAvailabilityTimeSlot)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool EditAvailabilityTimeSlot(int idAvailabilityTimeSlot, AvailabilityTimeSlot newAvailabilityTimeSlot)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool FinalizeAppointment(int idAppointment)
        // {
        //     throw new NotImplementedException();
        // }

        // public Appointment GetAppointmentDetails(int idAppointment)
        // {
        //     throw new NotImplementedException();
        // }

        // public List<Appointment> GetAppointments(DateTime startDate, DateTime endDate)
        // {
        //     throw new NotImplementedException();
        // }

        // public List<int> GetAvailableServices(DateTimeRange range)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool IsAppointmentInSpecificState(int idAppointment, AppointmentStatusType expected)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool IsAssistantAvailableInTimeRange(int idAssistant, DateTimeRange range)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool IsAvailabilityTimeSlotAvailable(DateTimeRange range)
        // {
        //     throw new NotImplementedException();
        // }

        // public Task<bool> RegisterAvailabilityTimeSlot(int idAssistant, DateTimeRange range)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool ScheduleAppointment(DateTimeRange range, List<Service> services, Client client)
        // {
        //     throw new NotImplementedException();
        // }
    }
}