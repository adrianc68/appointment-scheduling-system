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
                    IdClient = appointment.Client!.Id
                };
                dbContext.Appointments.Add(appointmentDB);
                await dbContext.SaveChangesAsync();

                foreach (var scheduledService in appointment.ScheduledServices!)
                {
                    var scheduleServices = new ScheduledService
                    {
                        IdAppointment = appointmentDB.Id,
                        IdServiceOffer = scheduledService.ServiceOffer!.Id,
                        ServiceStartTime = scheduledService.ServiceStartTime!.Value,
                        ServiceEndTime = scheduledService.ServiceEndTime!.Value,
                        ServicePrice = scheduledService.ServicePrice!.Value,
                        ServicesMinutes = scheduledService.ServicesMinutes!.Value,
                        ServiceName = scheduledService.ServiceName
                    };
                    dbContext.ScheduledServices.Add(scheduleServices);
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
                    IdAssistant = availabilityTimeSlot.Assistant!.Id,
                    Status = Model.Types.AvailabilityTimeSlotStatusType.AVAILABLE
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
                .Include(appAssSer => appAssSer.ScheduledServices!)
                    .ThenInclude(assisServ => assisServ.ServiceOffer)
                        .ThenInclude(assis => assis!.Assistant)
                            .ThenInclude(asacc => asacc!.UserAccount)
                            .ThenInclude(asinf => asinf!.UserInformation)
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
                ScheduledServices = app.ScheduledServices!.Select(aso => new BusinessLogicLayer.Model.ScheduledService
                {
                    ServiceStartTime = aso.ServiceStartTime,
                    ServiceEndTime = aso.ServiceEndTime,
                    ServicePrice = aso.ServicePrice,
                    ServiceName = aso.ServiceName,
                    ServicesMinutes = aso.ServicesMinutes,
                    ServiceOffer = new BusinessLogicLayer.Model.ServiceOffer
                    {
                        Id = aso.ServiceOffer!.Id,
                        Uuid = aso.ServiceOffer.Uuid,
                        Assistant = new BusinessLogicLayer.Model.Assistant
                        {
                            Id = aso.ServiceOffer.Assistant!.IdUserAccount,
                            Uuid = aso.ServiceOffer.Assistant.UserAccount!.Uuid,
                            Name = aso.ServiceOffer.Assistant.UserAccount.UserInformation!.Name,
                            Email = aso.ServiceOffer.Assistant.UserAccount.Email,
                            Username = aso.ServiceOffer.Assistant.UserAccount.Username,
                            CreatedAt = aso.ServiceOffer.Assistant.UserAccount.CreatedAt,
                            Status = (BusinessLogicLayer.Model.Types.AssistantStatusType?)aso.ServiceOffer.Assistant.Status,
                            PhoneNumber = aso.ServiceOffer.Assistant.UserAccount.UserInformation.PhoneNumber
                        }
                    }
                }).ToList(),
            }).ToList();
            return appointmentsModel;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.AvailabilityTimeSlot>> GetAvailabilityTimeSlotsAsync(DateOnly startDate, DateOnly endDate)
        {
            using var dbContext = context.CreateDbContext();
            var availableServices = await dbContext.AvailabilityTimeSlots
                .Where(slot => slot.Date >= startDate && slot.Date <= endDate && slot.Status == Model.Types.AvailabilityTimeSlotStatusType.AVAILABLE)
                    .Include(a => a.Assistant)
                        .ThenInclude(ass => ass!.UserAccount)
                        .ThenInclude(assc => assc!.UserInformation)
                .ToListAsync();

            var availabilityTimeSlotsModel = availableServices
                .Select(slot => new BusinessLogicLayer.Model.AvailabilityTimeSlot
                {
                    Id = slot.Id,
                    Uuid = slot.Uuid,
                    Date = slot.Date,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    Status = (BusinessLogicLayer.Model.Types.AvailabilityTimeSlotStatusType)slot.Status,
                    Assistant = new BusinessLogicLayer.Model.Assistant
                    {
                        Name = slot.Assistant!.UserAccount!.UserInformation!.Name,
                        Uuid = slot.Assistant.UserAccount.Uuid,
                        Id = slot.Assistant.IdUserAccount
                    }
                })
                .ToList();

            return availabilityTimeSlotsModel;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.ServiceOffer>> GetAvailableServicesAsync(DateOnly date)
        {
            using var dbContext = context.CreateDbContext();
            var availableServices = await dbContext.AvailabilityTimeSlots
                .Where(slot => slot.Date == date)
                .Include(slot => slot.Assistant)
                    .ThenInclude(assistant => assistant!.UserAccount)
                    .ThenInclude(userAccount => userAccount!.UserInformation)
                .Include(slot => slot.Assistant)
                    .ThenInclude(assistant => assistant!.ServiceOffers!)
                        .ThenInclude(asService => asService.Service)
                .ToListAsync();

            var serviceOffers = availableServices
                    .SelectMany(slot => slot.Assistant?.ServiceOffers!.Select(asService => new BusinessLogicLayer.Model.ServiceOffer
                    {
                        Assistant = new BusinessLogicLayer.Model.Assistant
                        {
                            Id = slot.Assistant!.UserAccount!.Id,
                            Uuid = slot.Assistant!.UserAccount!.Uuid,
                            Name = slot.Assistant!.UserAccount!.UserInformation!.Name
                        },
                        Service = new BusinessLogicLayer.Model.Service
                        {
                            Name = asService.Service!.Name,
                            Price = asService.Service!.Price,
                            Minutes = asService.Service!.Minutes,
                            Description = asService.Service?.Description,
                            Uuid = asService.Service!.Uuid,
                            Id = asService.Service!.Id
                        },
                        Id = asService.Id,
                        Uuid = asService.Uuid,
                        Status = (BusinessLogicLayer.Model.Types.ServiceOfferStatusType)asService.Status
                    }) ?? [])
                    .ToList();

            return serviceOffers;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.Types.DateTimeRange>> GetAppointmentDateTimeRangeConflictsByRange(BusinessLogicLayer.Model.Types.DateTimeRange range)
        {
            using var dbContext = context.CreateDbContext();
            var conflicts = await dbContext.Appointments
                .Where(a => a.Date == range.Date && a.StartTime < range.EndTime && a.EndTime > range.StartTime)
                .ToListAsync();

            if (conflicts == null)
            {
                return [];
            }

            List<BusinessLogicLayer.Model.Types.DateTimeRange> conflictingRanges = conflicts.Select(a => new BusinessLogicLayer.Model.Types.DateTimeRange
            {
                StartTime = a.StartTime!.Value,
                EndTime = a.EndTime!.Value,
                Date = a.Date!.Value
            }).ToList();

            return conflictingRanges;
        }

        public async Task<bool> IsAppointmentTimeSlotAvailableAsync(BusinessLogicLayer.Model.Types.DateTimeRange range)
        {
            using var dbContext = context.CreateDbContext();
            bool isAvailable = await dbContext.Appointments
                .Where(a => a.Date == range.Date && a.StartTime < range.EndTime && a.EndTime > range.StartTime)
                .AnyAsync();

            return !isAvailable;
        }

        public async Task<bool> IsAvailabilityTimeSlotRegisteredAsync(BusinessLogicLayer.Model.Types.DateTimeRange range, int idAssistant)
        {
            using var dbContext = context.CreateDbContext();
            bool isAvailable = await dbContext.AvailabilityTimeSlots
                .Where(a => a.Date == range.Date && a.StartTime < range.EndTime && a.EndTime > range.StartTime && a.IdAssistant == idAssistant)
                .AnyAsync();
            return !isAvailable;
        }

        public async Task<bool> IsAssistantAvailableInAvailabilityTimeSlotsAsync(BusinessLogicLayer.Model.Types.DateTimeRange range, int idAssistant)
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

        public async Task<bool> HasAssistantConflictingAppoinmentsAsync(BusinessLogicLayer.Model.Types.DateTimeRange range, int idAssistant)
        {
            using var dbContext = context.CreateDbContext();
            var conflictingOffers = await dbContext.ScheduledServices
                .Where(aso =>
                    aso.ServiceOffer!.IdAssistant == idAssistant &&
                    aso.Appointment!.Date == range.Date &&
                    !(range.EndTime <= aso.ServiceStartTime || range.StartTime >= aso.ServiceEndTime) &&
                    (aso.Appointment.Status == Model.Types.AppointmentStatusType.SCHEDULED ||
                    aso.Appointment.Status == Model.Types.AppointmentStatusType.CONFIRMED))
                .ToListAsync();

            if (conflictingOffers.Any())
            {
                return false;
            }
            return true;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.ScheduledService>> GetConflictingServicesByDateTimeRangeAsync(BusinessLogicLayer.Model.Types.DateTimeRange range)
        {
            using var dbContext = context.CreateDbContext();

            var rawConflicts = await dbContext.ScheduledServices
                .Include(a => a.ServiceOffer)
                    .ThenInclude(ase => ase!.Assistant)
                    .ThenInclude(asac => asac!.UserAccount)
                    .ThenInclude(uac => uac!.UserInformation)
                .Where(aso =>
                    aso.Appointment!.Date == range.Date &&
                    !(range.EndTime <= aso.ServiceStartTime || range.StartTime >= aso.ServiceEndTime) &&
                    (aso.Appointment.Status == Model.Types.AppointmentStatusType.SCHEDULED ||
                     aso.Appointment.Status == Model.Types.AppointmentStatusType.CONFIRMED))
                .ToListAsync();

            var conflictingOffers = rawConflicts.Select(aso => new BusinessLogicLayer.Model.ScheduledService
            {
                Id = aso.ServiceOffer!.Id,
                Uuid = aso.ServiceOffer.Uuid,
                ServiceStartTime = aso.ServiceStartTime,
                ServiceEndTime = aso.ServiceEndTime,
                ServicePrice = aso.ServicePrice,
                ServiceName = aso.ServiceName,
                ServicesMinutes = aso.ServicesMinutes,
                ServiceOffer = new BusinessLogicLayer.Model.ServiceOffer
                {
                    Id = aso.ServiceOffer.Id,
                    Uuid = aso.ServiceOffer.Uuid,
                    Status = (BusinessLogicLayer.Model.Types.ServiceOfferStatusType?) aso.ServiceOffer.Status,
                    Assistant = new BusinessLogicLayer.Model.Assistant
                    {
                        Uuid = aso.ServiceOffer.Assistant!.UserAccount!.Uuid,
                        Name = aso.ServiceOffer.Assistant.UserAccount.UserInformation!.Name
                    }
                },
            }).ToList();
            return conflictingOffers;
        }


        public async Task<BusinessLogicLayer.Model.Appointment?> GetAppointmentByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();

            var appointmentDB = await dbContext.Appointments
            .Include(ap => ap.Client)
                .ThenInclude(ac => ac!.UserAccount)
                .ThenInclude(ua => ua!.UserInformation)
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
                Status = (BusinessLogicLayer.Model.Types.AppointmentStatusType)appointmentDB.Status!.Value,
                Client = new BusinessLogicLayer.Model.Client
                {
                    Id = appointmentDB.Client!.IdUserAccount,
                    Uuid = appointmentDB.Client.UserAccount!.Uuid,
                    Name = appointmentDB.Client.UserAccount.UserInformation!.Name,
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
                .ThenInclude(ac => ac!.UserAccount)
                .ThenInclude(ua => ua!.UserInformation)
            .Include(s => s.ScheduledServices!)
                .ThenInclude(ser => ser.ServiceOffer)
                .ThenInclude(so => so!.Service)
            .Include(s => s.ScheduledServices!)
                .ThenInclude(ser => ser.ServiceOffer)
                .ThenInclude(so => so!.Assistant)
                .ThenInclude(ac => ac!.UserAccount)
                .ThenInclude(ua => ua!.UserInformation)
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
                Status = (BusinessLogicLayer.Model.Types.AppointmentStatusType)appointmentDB.Status!.Value,
                Client = new BusinessLogicLayer.Model.Client
                {
                    Id = appointmentDB.Client!.IdUserAccount,
                    Uuid = appointmentDB.Client.UserAccount!.Uuid,
                    Name = appointmentDB.Client.UserAccount.UserInformation!.Name,
                    Email = appointmentDB.Client.UserAccount.Email,
                    PhoneNumber = appointmentDB.Client.UserAccount.UserInformation.PhoneNumber
                },
                ScheduledServices = appointmentDB.ScheduledServices!.Select(aso => new BusinessLogicLayer.Model.ScheduledService
                {
                    ServiceStartTime = aso.ServiceStartTime,
                    ServiceEndTime = aso.ServiceEndTime,
                    ServicePrice = aso.ServicePrice,
                    ServiceName = aso.ServiceName,
                    ServicesMinutes = aso.ServicesMinutes,
                    ServiceOffer = new BusinessLogicLayer.Model.ServiceOffer
                    {
                        Id = aso.ServiceOffer!.Id,
                        Uuid = aso.ServiceOffer.Uuid,
                        Assistant = new BusinessLogicLayer.Model.Assistant
                        {
                            Id = aso.ServiceOffer.Assistant!.IdUserAccount,
                            Uuid = aso.ServiceOffer.Assistant.UserAccount!.Uuid,
                            Name = aso.ServiceOffer.Assistant.UserAccount.UserInformation!.Name,
                            Email = aso.ServiceOffer.Assistant.UserAccount.Email,
                            PhoneNumber = aso.ServiceOffer.Assistant.UserAccount.UserInformation.PhoneNumber
                        },
                        Service = null
                    },
                }).ToList(),
            };
            return appointment;
        }

        public async Task<bool> ChangeAppointmentStatusTypeAsync(int idAppointment, BusinessLogicLayer.Model.Types.AppointmentStatusType status)
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
                    .ThenInclude(ua => ua!.UserAccount)
                    .ThenInclude(ui => ui!.UserInformation)
                .ToListAsync();

            var appointmentsModel = appointmentDB.Select(app => new BusinessLogicLayer.Model.Appointment
            {
                Date = app.Date,
                EndTime = app.EndTime,
                StartTime = app.StartTime,
                TotalCost = app.TotalCost,
                Uuid = app.Uuid,
                Status = (BusinessLogicLayer.Model.Types.AppointmentStatusType)app.Status!.Value,
                Id = app.Id,
                CreatedAt = app.CreatedAt,
                Client = new BusinessLogicLayer.Model.Client
                {
                    Id = app.Client!.IdUserAccount,
                    Uuid = app.Client.UserAccount!.Uuid,
                    Name = app.Client.UserAccount.UserInformation!.Name,
                    Email = app.Client.UserAccount.Email,
                    PhoneNumber = app.Client.UserAccount.UserInformation.PhoneNumber,
                    Status = (BusinessLogicLayer.Model.Types.ClientStatusType)app.Client.Status!.Value,
                    Username = app.Client.UserAccount.Username,
                    CreatedAt = app.Client.UserAccount.CreatedAt
                },
            }).ToList();

            return appointmentsModel;
        }

        public async Task<bool> ChangeServiceOfferStatusTypeAsync(int idServiceOffer, BusinessLogicLayer.Model.Types.ServiceOfferStatusType status)
        {
            bool isStatusChanged = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var appointment = await dbContext.ServiceOffers
                     .FirstOrDefaultAsync(ap => ap.Id == idServiceOffer);

                if (appointment == null)
                {
                    return false;
                }

                appointment.Status = (DataLayer.DatabaseComponents.Model.Types.ServiceOfferStatusType)status;
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

        public async Task<BusinessLogicLayer.Model.ServiceOffer?> GetServiceOfferByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var serviceOfferDb = await dbContext.ServiceOffers
                    .Include(ser => ser.Service)
                .FirstOrDefaultAsync(a => a.Uuid == uuid);

            if (serviceOfferDb == null)
            {
                return null;
            }

            var serviceOffers = new BusinessLogicLayer.Model.ServiceOffer
            {
                Id = serviceOfferDb.Id,
                Uuid = serviceOfferDb.Uuid,
                Status = (BusinessLogicLayer.Model.Types.ServiceOfferStatusType)serviceOfferDb.Status,
                Service = new BusinessLogicLayer.Model.Service
                {
                    Id = serviceOfferDb.Service!.Id,
                    Uuid = serviceOfferDb.Service.Uuid,
                    Price = serviceOfferDb.Service.Price,
                    Name = serviceOfferDb.Service.Name,
                    Minutes = serviceOfferDb.Service.Minutes,
                    Description = serviceOfferDb.Service.Description
                },
            };
            return serviceOffers;
        }

        public async Task<bool> DeleteAvailabilityTimeSlot(int idAvailabilityTimeSlot)
        {
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var timeSlot = await dbContext.AvailabilityTimeSlots.FirstOrDefaultAsync(a => a.Id == idAvailabilityTimeSlot);

                if (timeSlot == null)
                {
                    return false;
                }
                dbContext.AvailabilityTimeSlots.Remove(timeSlot);
                await dbContext.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return true;
        }

        public async Task<BusinessLogicLayer.Model.AvailabilityTimeSlot?> GetAvailabilityTimeSlotByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var availabilityTimeSlotDB = await dbContext.AvailabilityTimeSlots
                    .Include(a => a.Assistant)
                        .ThenInclude(ass => ass!.UserAccount)
                        .ThenInclude(assc => assc!.UserInformation)
                    .FirstOrDefaultAsync(a => a.Uuid == uuid);

            if(availabilityTimeSlotDB == null)
            {
                return null;
            }

            var availabilityTimeSlotsModel = new BusinessLogicLayer.Model.AvailabilityTimeSlot
            {
                Id = availabilityTimeSlotDB!.Id,
                Uuid = availabilityTimeSlotDB.Uuid,
                Date = availabilityTimeSlotDB.Date,
                StartTime = availabilityTimeSlotDB.StartTime,
                EndTime = availabilityTimeSlotDB.EndTime,
                Status = (BusinessLogicLayer.Model.Types.AvailabilityTimeSlotStatusType)availabilityTimeSlotDB.Status,
                Assistant = new BusinessLogicLayer.Model.Assistant
                {
                    Name = availabilityTimeSlotDB.Assistant!.UserAccount!.UserInformation!.Name,
                    Uuid = availabilityTimeSlotDB.Assistant.UserAccount.Uuid,
                    Id = availabilityTimeSlotDB.Assistant.IdUserAccount
                }
            };

            return availabilityTimeSlotsModel;
        }

        public async Task<bool> ChangeAvailabilityStatusTypeAsync(int idAvailabilityTimeSlot, BusinessLogicLayer.Model.Types.AvailabilityTimeSlotStatusType status)
        {
            bool isStatusChanged = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var timeSlot = await dbContext.AvailabilityTimeSlots.FirstOrDefaultAsync(a => a.Id == idAvailabilityTimeSlot);
                if (timeSlot == null)
                {
                    return false;
                }
                timeSlot.Status = (Model.Types.AvailabilityTimeSlotStatusType)status;
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

        public async Task<bool> UpdateAvailabilityTimeSlot(BusinessLogicLayer.Model.AvailabilityTimeSlot availabilityTimeSlot)
        {
            bool isUpdated = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var existingSlot = await dbContext.AvailabilityTimeSlots.FirstOrDefaultAsync(slot => slot.Id == availabilityTimeSlot.Id);
                if (existingSlot == null)
                {
                    return false;
                }

                existingSlot.Date = availabilityTimeSlot.Date;
                existingSlot.StartTime = availabilityTimeSlot.StartTime;
                existingSlot.EndTime = availabilityTimeSlot.EndTime;

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                isUpdated = true;
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return isUpdated;
        }

        public async Task<bool> HasAvailabilityTimeSlotConflictingSlotsAsync(BusinessLogicLayer.Model.Types.DateTimeRange range, int idAvailabilityTimeSlot, int idAssistant)
        {
            using var dbContext = context.CreateDbContext();
            var availableServices = await dbContext.AvailabilityTimeSlots
                .Where(slot => slot.Date == range.Date && slot.IdAssistant == idAssistant && slot.Status != Model.Types.AvailabilityTimeSlotStatusType.DELETED && slot.Id != idAvailabilityTimeSlot)
                .ToListAsync();

            bool hasConflict = availableServices.Any(slot => range.StartTime < slot.EndTime && range.EndTime > slot.StartTime);

            return hasConflict;
        }

        public async Task<int> GetAppointmentsScheduledCountByClientUuid(int idClient)
        {
            using var dbContext = context.CreateDbContext();
            var count = await dbContext.Appointments
                .Where(a => a.IdClient == idClient && (a.Status == Model.Types.AppointmentStatusType.CONFIRMED || a.Status == Model.Types.AppointmentStatusType.SCHEDULED)).CountAsync();
            return count;
        }
    }
}