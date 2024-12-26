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

        public async Task<IEnumerable<BusinessLogicLayer.Model.AvailabilityTimeSlot>> GetAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate)
        {
            var availableServices = await context.AvailabilityTimeSlots
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
                    }
                })
                .ToList();

            return availabilityTimeSlotsModel;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.AssistantService>> GetAvailableServicesAsync(DateOnly date)
        {
            var availableServices = await context.AvailabilityTimeSlots
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
                            Uuid = asService.Service?.Uuid
                        }) ?? new List<BusinessLogicLayer.Model.Service>())
                        .GroupBy(service => new { service.Name, service.Price, service.Minutes })
                        .Select(groupedService => groupedService.First())
                        .ToList()
                })
                .ToList();
            return businessLogicAssistantServices;
        }

        public async Task<bool> AddAppointmentAsync(BusinessLogicLayer.Model.Appointment appointment)
        {
            if (appointment.Client == null)
            {
                return false;
            }


            bool isRegistered = false;
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var newAppointment = new Appointment
                {
                    Uuid = appointment.Uuid,
                    EndTime = appointment.EndTime,
                    StartTime = appointment.StartTime,
                    Date = appointment.Date,
                    Status = (Model.Types.AppointmentStatusType?)appointment.Status,
                    TotalCost = appointment.TotalCost,
                    IdClient = appointment.Client.Id,
                };

                context.Appointments.Add(newAppointment);
                await context.SaveChangesAsync();

                var newAppointmentServices = appointment.AssistantServices?.Select(async asstService =>
                       {
                           var assistant = await context.Assistants.FirstOrDefaultAsync(a => a.UserAccount.Uuid == asstService.Assistant.Uuid);
                           if (assistant == null)
                               throw new Exception("Assistant not found with the provided UUID");
                           var services = new List<AppointmentService>();
                           if (asstService.Services != null)
                           {
                               foreach (var service in asstService.Services)
                               {
                                   var serviceEntity = await context.Services.FirstOrDefaultAsync(s => s.Uuid == service.Uuid);

                                   if (serviceEntity == null)
                                       throw new Exception("Service not found with the provided UUID");
                                   services.Add(new AppointmentService
                                   {
                                       IdAppointment = newAppointment.Id,
                                       IdService = serviceEntity.Id
                                   });
                               }
                           }

                           return services;
                       }).ToList();


                var allServices = (await Task.WhenAll(newAppointmentServices)).SelectMany(s => s).ToList();

                if (allServices.Any())
                {
                    await context.AppointmentServices.AddRangeAsync(allServices);
                    await context.SaveChangesAsync();
                }


                // Handling assistants
                var newAppointmentAssistants = new List<AppointmentAssistant>();
                if (appointment.AssistantServices != null)
                {
                    foreach (var asstService in appointment.AssistantServices)
                    {
                        var assistant = await context.Assistants.FirstOrDefaultAsync(a => a.UserAccount.Uuid == asstService.Assistant.Uuid);
                        if (assistant == null)
                            throw new Exception("Assistant not found with the provided UUID");

                        newAppointmentAssistants.Add(new AppointmentAssistant
                        {
                            IdAppointment = newAppointment.Id,
                            IdAssistant = assistant.UserAccount.Id
                        });
                    }
                }

                if (newAppointmentAssistants.Any())
                {
                    await context.AppointmentAssistants.AddRangeAsync(newAppointmentAssistants);
                    await context.SaveChangesAsync();
                }


                // // Create AppointmentAssistant relationships
                // var newAppointmentAssistants = appointment.AssistantServices?.Select(async asstService =>
                // {
                //     PropToString.PrintData(asstService);
                //     PropToString.PrintData(asstService.Assistant);
                //     var assistant = await context.Assistants.FirstOrDefaultAsync(a => a.UserAccount.Uuid == asstService.Assistant.Uuid);
                //     if (assistant == null)
                //         throw new Exception("Assistant not found with the provided UUID");

                //     System.Console.WriteLine("*****************");
                //     System.Console.WriteLine("*****************");

                //     PropToString.PrintData(assistant);

                //     return new AppointmentAssistant
                //     {
                //         IdAppointment = newAppointment.Id,
                //         IdAssistant = assistant.IdUserAccount
                //     };
                // }).ToList();

                // // Await all tasks and get the list of AppointmentAssistant objects
                // var allAssistants = await Task.WhenAll(newAppointmentAssistants);

                // // Flatten the result: each element in `allAssistants` is an `AppointmentAssistant`
                // var flattenedAssistants = allAssistants.ToList();

                // if (flattenedAssistants.Any())
                // {
                //     await context.AppointmentAssistants.AddRangeAsync(flattenedAssistants);
                //     await context.SaveChangesAsync();
                // }


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
                    Uuid = availabilityTimeSlot.Uuid,
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