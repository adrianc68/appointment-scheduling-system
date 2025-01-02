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
                var appointmentDB = new Appointment
                {
                    Date = appointment.Date,
                    EndTime = appointment.EndTime,
                    StartTime = appointment.StartTime,
                    TotalCost = appointment.TotalCost,
                    Uuid = appointment.Uuid,
                    Status = (Model.Types.AppointmentStatusType?)appointment.Status,
                    IdClient = appointment.Client.Id
                };
                dbContext.Appointments.Add(appointmentDB);
                await dbContext.SaveChangesAsync();

                foreach (var serviceOffer in appointment.ServiceOffers)
                {
                    var appointmentAssistantService = new AppointmentServiceOffer
                    {
                        IdAppointment = appointmentDB.Id,
                        IdServiceOffer = serviceOffer.Id,
                        StartTime = serviceOffer.StartTime!.Value,
                        EndTime = serviceOffer.EndTime!.Value
                    };
                    dbContext.AppointmentServiceOffers.Add(appointmentAssistantService);
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

        public async Task<bool> AddAvailabilityTimeSlotAsync(BusinessLogicLayer.Model.AvailabilityTimeSlot availabilityTimeSlot)
        {
            bool isRegistered = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var timeSlot = new AvailabilityTimeSlot
                {
                    Uuid = availabilityTimeSlot.Uuid,
                    Date = availabilityTimeSlot.Date,
                    StartTime = availabilityTimeSlot.StartTime,
                    EndTime = availabilityTimeSlot.EndTime,
                    IdAssistant = availabilityTimeSlot.Assistant!.Id
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

        public async Task<IEnumerable<BusinessLogicLayer.Model.Appointment>> GetScheduledOrConfirmedAppoinmentsAsync(DateOnly startDate, DateOnly endDate)
        {
            using var dbContext = context.CreateDbContext();
            var appointmentDB = await dbContext.Appointments
                .Where(app => app.Date >= startDate && app.Date <= endDate && (app.Status == Model.Types.AppointmentStatusType.CONFIRMED || app.Status == Model.Types.AppointmentStatusType.SCHEDULED))
                .Include(appAssSer => appAssSer.AppointmentServiceOffers)
                    .ThenInclude(assisServ => assisServ.ServiceOffer)
                        .ThenInclude(assis => assis.Assistant)
                            .ThenInclude(asacc => asacc.UserAccount)
                            .ThenInclude(asinf => asinf.UserInformation)
                .ToListAsync();

            var appointmentsModel = appointmentDB.Select(app => new BusinessLogicLayer.Model.Appointment
            {
                Date = app.Date,
                EndTime = app.EndTime,
                StartTime = app.StartTime,
                TotalCost = app.TotalCost,
                Uuid = app.Uuid,
                Id = app.Id,
                CreatedAt = app.CreatedAt,
                Client = null,
                ServiceOffers = app.AppointmentServiceOffers.Select(aso => new BusinessLogicLayer.Model.ServiceOffer
                {
                    Id = aso.ServiceOffer.Id,
                    Uuid = aso.ServiceOffer.Uuid,
                    StartTime = aso.StartTime,
                    EndTime = aso.EndTime,
                    Service = null,
                    Assistant = new BusinessLogicLayer.Model.Assistant
                    {
                        Id = aso.ServiceOffer.Assistant.IdUserAccount,
                        Uuid = aso.ServiceOffer.Assistant.UserAccount.Uuid,
                        Name = aso.ServiceOffer.Assistant.UserAccount.UserInformation.Name,
                        Email = aso.ServiceOffer.Assistant.UserAccount.Email,
                        Username = aso.ServiceOffer.Assistant.UserAccount.Username,
                        CreatedAt = aso.ServiceOffer.Assistant.UserAccount.CreatedAt,
                        Status = (AssistantStatusType?)aso.ServiceOffer.Assistant.Status,
                        PhoneNumber = aso.ServiceOffer.Assistant.UserAccount.UserInformation.PhoneNumber
                    },
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
                    .ThenInclude(assistant => assistant.ServiceOffers)
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
                            .SelectMany(slot => slot.Assistant?.ServiceOffers?.Select(asService => new BusinessLogicLayer.Model.Service
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

        public async Task<bool> IsAppointmentTimeSlotAvailableAsync(BusinessLogicLayer.Model.Types.DateTimeRange range)
        {
            using var dbContext = context.CreateDbContext();
            bool isAvailable = await dbContext.Appointments
                .Where(a => a.Date == range.Date && a.StartTime < range.EndTime && a.EndTime > range.StartTime)
                .AnyAsync();

            return !isAvailable;
        }

        public async Task<bool> IsAvailabilityTimeSlotRegisteredAsync(DateTimeRange range, int idAssistant)
        {
            using var dbContext = context.CreateDbContext();
            bool isAvailable = await dbContext.AvailabilityTimeSlots
                .Where(a => a.Date == range.Date && a.StartTime < range.EndTime && a.EndTime > range.StartTime && a.IdAssistant == idAssistant)
                .AnyAsync();
            return !isAvailable;
        }

        public async Task<bool> IsAssistantAvailableInAvailabilityTimeSlotsAsync(DateTimeRange range, int idAssistant)
        {
            using var dbContext = context.CreateDbContext();
            var availableSlots = await dbContext.AvailabilityTimeSlots
                .Where(ats => ats.IdAssistant == idAssistant && ats.Date == range.Date)
                .ToListAsync();
            bool isCoveredByAnySlot = availableSlots.Any(slot => range.StartTime >= slot.StartTime && range.EndTime <= slot.EndTime);

            if (!isCoveredByAnySlot)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> HasAssistantConflictingAppoinmentsAsync(DateTimeRange range, int idAssistant)
        {
            using var dbContext = context.CreateDbContext();
            var conflictingOffers = await dbContext.AppointmentServiceOffers
                .Where(aso =>
                    aso.ServiceOffer.IdAssistant == idAssistant &&
                    aso.Appointment.Date == range.Date &&
                    !(range.EndTime <= aso.StartTime || range.StartTime >= aso.EndTime) &&
                    (aso.Appointment.Status == Model.Types.AppointmentStatusType.SCHEDULED ||
                    aso.Appointment.Status == Model.Types.AppointmentStatusType.CONFIRMED))
                .ToListAsync();

            if (conflictingOffers.Any())
            {
                return false;
            }
            return true;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.ServiceOffer>> GetConflictingServicesByDateTimeRangeAsync(DateTimeRange range)
        {
            using var dbContext = context.CreateDbContext();
            var conflictingOffers = await dbContext.AppointmentServiceOffers
                .Where(aso =>
                    aso.Appointment.Date == range.Date &&
                    !(range.EndTime <= aso.StartTime || range.StartTime >= aso.EndTime) &&
                    (aso.Appointment.Status == Model.Types.AppointmentStatusType.SCHEDULED ||
                    aso.Appointment.Status == Model.Types.AppointmentStatusType.CONFIRMED))
                .Select(aso => new BusinessLogicLayer.Model.ServiceOffer
                {
                    Id = aso.ServiceOffer.Id,
                    Uuid = aso.ServiceOffer.Uuid,
                    StartTime = aso.StartTime,
                    EndTime = aso.EndTime,
                    Assistant = new BusinessLogicLayer.Model.Assistant
                    {
                        Uuid = aso.ServiceOffer.Assistant.UserAccount.Uuid,
                        Name = aso.ServiceOffer.Assistant.UserAccount.UserInformation.Name
                    }
                })
                .ToListAsync();
            return conflictingOffers;
        }


        public async Task<BusinessLogicLayer.Model.Appointment?> GetAppointmentByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();

            var appointmentDB = await dbContext.Appointments
            .Include(ap => ap.Client)
                .ThenInclude(ac => ac.UserAccount)
                .ThenInclude(ua => ua.UserInformation)
            .FirstOrDefaultAsync(ap => ap.Uuid == uuid);

            if (appointmentDB == null)
            {
                return null;
            }

            var appointment = new BusinessLogicLayer.Model.Appointment
            {
                Id = appointmentDB.Id,
                Uuid = appointmentDB.Uuid,
                Date = appointmentDB.Date,
                StartTime = appointmentDB.StartTime,
                EndTime = appointmentDB.EndTime,
                TotalCost = appointmentDB.TotalCost,
                CreatedAt = appointmentDB.CreatedAt,
                Status = (AppointmentStatusType)appointmentDB.Status!.Value,
                Client = new BusinessLogicLayer.Model.Client
                {
                    Id = appointmentDB.Client.IdUserAccount,
                    Uuid = appointmentDB.Client.UserAccount.Uuid,
                    Name = appointmentDB.Client.UserAccount.UserInformation.Name,
                    Email = appointmentDB.Client.UserAccount.Email,
                    PhoneNumber = appointmentDB.Client.UserAccount.UserInformation.PhoneNumber
                },
            };
            return appointment;
        }

        public async Task<BusinessLogicLayer.Model.Appointment?> GetAppointmentFullDetailsByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();

            var appointmentDB = await dbContext.Appointments
            .Include(ap => ap.Client)
                .ThenInclude(ac => ac.UserAccount)
                .ThenInclude(ua => ua.UserInformation)
            .Include(s => s.AppointmentServiceOffers)
                .ThenInclude(ser => ser.ServiceOffer)
                .ThenInclude(so => so.Service)
            .Include(s => s.AppointmentServiceOffers)
                .ThenInclude(ser => ser.ServiceOffer)
                .ThenInclude(so => so.Assistant)
                .ThenInclude(ac => ac.UserAccount)
                .ThenInclude(ua => ua.UserInformation)
            .FirstOrDefaultAsync(ap => ap.Uuid == uuid);


            if (appointmentDB == null)
            {
                return null;
            }

            var appointment = new BusinessLogicLayer.Model.Appointment
            {
                Id = appointmentDB.Id,
                Uuid = appointmentDB.Uuid,
                Date = appointmentDB.Date,
                StartTime = appointmentDB.StartTime,
                EndTime = appointmentDB.EndTime,
                TotalCost = appointmentDB.TotalCost,
                CreatedAt = appointmentDB.CreatedAt,
                Status = (AppointmentStatusType)appointmentDB.Status!.Value,
                Client = new BusinessLogicLayer.Model.Client
                {
                    Id = appointmentDB.Client.IdUserAccount,
                    Uuid = appointmentDB.Client.UserAccount.Uuid,
                    Name = appointmentDB.Client.UserAccount.UserInformation.Name,
                    Email = appointmentDB.Client.UserAccount.Email,
                    PhoneNumber = appointmentDB.Client.UserAccount.UserInformation.PhoneNumber
                },
                ServiceOffers = appointmentDB.AppointmentServiceOffers
                .Select(aso => new BusinessLogicLayer.Model.ServiceOffer
                {
                    Id = aso.ServiceOffer.Id,
                    Uuid = aso.ServiceOffer.Uuid,
                    StartTime = aso.StartTime,
                    EndTime = aso.EndTime,
                    Assistant = aso.ServiceOffer.Assistant != null
                        ? new BusinessLogicLayer.Model.Assistant
                        {
                            Id = aso.ServiceOffer.Assistant.IdUserAccount,
                            Uuid = aso.ServiceOffer.Assistant.UserAccount.Uuid,
                            Name = aso.ServiceOffer.Assistant.UserAccount.UserInformation.Name,
                            Email = aso.ServiceOffer.Assistant.UserAccount.Email,
                            PhoneNumber = aso.ServiceOffer.Assistant.UserAccount.UserInformation.PhoneNumber
                        }
                        : null,
                    Service = aso.ServiceOffer.Service != null
                        ? new BusinessLogicLayer.Model.Service
                        {
                            Id = aso.ServiceOffer.Service.Id,
                            Uuid = aso.ServiceOffer.Service.Uuid,
                            Name = aso.ServiceOffer.Service.Name,
                            Description = aso.ServiceOffer.Service.Description,
                            Minutes = aso.ServiceOffer.Service.Minutes,
                            Price = aso.ServiceOffer.Service.Price,
                            Status = (BusinessLogicLayer.Model.Types.ServiceStatusType?)aso.ServiceOffer.Service.Status,
                            CreatedAt = aso.ServiceOffer.Service.CreatedAt
                        }
                        : null
                }).ToList()
            };
            return appointment;
        }

        public async Task<bool> ChangeAppointmentStatusTypeAsync(int idAppointment, AppointmentStatusType status)
        {
            bool isStatusChanged = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var appointment = await dbContext.Appointments
                     .FirstOrDefaultAsync(ap => ap.Id == idAppointment);

                if (appointment == null)
                {
                    return false;
                }

                appointment.Status = (DataLayer.DatabaseComponents.Model.Types.AppointmentStatusType)status;
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

        public async Task<IEnumerable<BusinessLogicLayer.Model.Appointment>> GetAllAppoinments(DateOnly startDate, DateOnly endDate)
        {
            using var dbContext = context.CreateDbContext();
            var appointmentDB = await dbContext.Appointments
                .Where(app => app.Date >= startDate && app.Date <= endDate)
                .Include(c => c.Client)
                    .ThenInclude(ua => ua.UserAccount)
                    .ThenInclude(ui => ui.UserInformation)
                .ToListAsync();

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
                    Id = app.Client.IdUserAccount,
                    Uuid = app.Client.UserAccount.Uuid,
                    Name = app.Client.UserAccount.UserInformation.Name,
                    Email = app.Client.UserAccount.Email,
                    PhoneNumber = app.Client.UserAccount.UserInformation.PhoneNumber
                },
            }).ToList();

            return appointmentsModel;
        }
    }
}