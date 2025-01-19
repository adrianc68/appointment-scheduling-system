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
                        ServiceName = scheduledService.ServiceName,
                        Uuid = scheduledService.Uuid!.Value
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
                    Uuid = availabilityTimeSlot.Uuid!.Value,
                    Date = availabilityTimeSlot.Date,
                    StartTime = availabilityTimeSlot.StartTime,
                    EndTime = availabilityTimeSlot.EndTime,
                    IdAssistant = availabilityTimeSlot.Assistant!.Id,
                    Status = Model.Types.AvailabilityTimeSlotStatusType.ENABLED
                };
                dbContext.AvailabilityTimeSlots.Add(timeSlot);
                await dbContext.SaveChangesAsync();

                if (availabilityTimeSlot.UnavailableTimeSlots != null)
                {
                    List<UnavailableTimeSlot> unavailableTimeSlotsDb = availabilityTimeSlot.UnavailableTimeSlots.Select(una => new UnavailableTimeSlot
                    {
                        StartTime = una.StartTime,
                        EndTime = una.EndTime,
                        IdAvailabilityTimeSlot = timeSlot.Id
                    }).ToList();
                    dbContext.UnavailableTimeSlots.AddRange(unavailableTimeSlotsDb);
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
                .Where(app => app.Date >= startDate && app.Date <= endDate && (app.Status == Model.Types.AppointmentStatusType.CONFIRMED || app.Status == Model.Types.AppointmentStatusType.SCHEDULED || app.Status == Model.Types.AppointmentStatusType.RESCHEDULED))
                .Include(appAssSer => appAssSer.ScheduledServices!)
                    .ThenInclude(assisServ => assisServ.ServiceOffer)
                    .ThenInclude(assis => assis!.Assistant)
                    .ThenInclude(asacc => asacc!.UserAccount)
                    .ThenInclude(asinf => asinf!.UserInformation)
                .Include(cli => cli.Client)
                    .ThenInclude(asacc => asacc!.UserAccount)
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
                    Uuid = app.Client!.UserAccount!.Uuid,
                    Id = app.Client.IdUserAccount
                },
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
                            Status = (BusinessLogicLayer.Model.Types.AccountStatusType?)aso.ServiceOffer.Assistant.UserAccount.Status,
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
                .Where(slot => slot.Date >= startDate && slot.Date <= endDate && slot.Status == Model.Types.AvailabilityTimeSlotStatusType.ENABLED)
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
                        Id = slot.Assistant.IdUserAccount,
                        PhoneNumber = slot.Assistant!.UserAccount!.UserInformation!.Name,
                        Email = slot.Assistant!.UserAccount!.UserInformation!.Name,
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
                            Name = slot.Assistant!.UserAccount!.UserInformation!.Name,
                            PhoneNumber = slot.Assistant!.UserAccount!.UserInformation!.Name,
                            Email = slot.Assistant!.UserAccount!.UserInformation!.Name,
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

        public async Task<IEnumerable<BusinessLogicLayer.Model.Types.DateTimeRange>> GetAppointmentDateTimeRangeConflictsByRangeAsync(BusinessLogicLayer.Model.Types.DateTimeRange range)
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
                    (aso.Appointment.Status == Model.Types.AppointmentStatusType.SCHEDULED || aso.Appointment.Status == Model.Types.AppointmentStatusType.CONFIRMED || aso.Appointment.Status == Model.Types.AppointmentStatusType.RESCHEDULED))
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
                    (aso.Appointment.Status == Model.Types.AppointmentStatusType.SCHEDULED || aso.Appointment.Status == Model.Types.AppointmentStatusType.CONFIRMED || aso.Appointment.Status == Model.Types.AppointmentStatusType.RESCHEDULED))
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
                    Status = (BusinessLogicLayer.Model.Types.ServiceOfferStatusType?)aso.ServiceOffer.Status,
                    Assistant = new BusinessLogicLayer.Model.Assistant
                    {
                        Uuid = aso.ServiceOffer.Assistant!.UserAccount!.Uuid,
                        Name = aso.ServiceOffer.Assistant.UserAccount.UserInformation!.Name,
                        Id = aso.ServiceOffer.Assistant.UserAccount.Id,
                        PhoneNumber = aso.ServiceOffer.Assistant.UserAccount.UserInformation!.PhoneNumber,
                        Email = aso.ServiceOffer.Assistant.UserAccount.Email,
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

        public async Task<IEnumerable<BusinessLogicLayer.Model.Appointment>> GetAllAppoinmentsAsync(DateOnly startDate, DateOnly endDate)
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
                    Status = (BusinessLogicLayer.Model.Types.AccountStatusType)app.Client.UserAccount.Status!.Value,
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

        public async Task<bool> DeleteAvailabilityTimeSlotAsync(int idAvailabilityTimeSlot)
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

        public async Task<BusinessLogicLayer.Model.AvailabilityTimeSlot?> GetAvailabilityTimeSlotByIdAsync(int idAvailabilityTimeSlot)
        {
            using var dbContext = context.CreateDbContext();
            var availabilityTimeSlotDB = await dbContext.AvailabilityTimeSlots
                    .Include(una => una.UnavailableTimeSlots)
                    .Include(a => a.Assistant)
                        .ThenInclude(ass => ass!.UserAccount)
                        .ThenInclude(assc => assc!.UserInformation)
                    .FirstOrDefaultAsync(a => a.Id == idAvailabilityTimeSlot);

            if (availabilityTimeSlotDB == null)
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
                    Id = availabilityTimeSlotDB.Assistant.IdUserAccount,
                    PhoneNumber = availabilityTimeSlotDB.Assistant.UserAccount.UserInformation.PhoneNumber,
                    Email = availabilityTimeSlotDB.Assistant.UserAccount.Email,
                },
                UnavailableTimeSlots = availabilityTimeSlotDB.UnavailableTimeSlots?.Select(a => new BusinessLogicLayer.Model.UnavailableTimeSlot
                {
                    StartTime = a.StartTime!.Value,
                    EndTime = a.EndTime!.Value
                }).ToList()
            };

            return availabilityTimeSlotsModel;
        }

        public async Task<BusinessLogicLayer.Model.AvailabilityTimeSlot?> GetAvailabilityTimeSlotByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var availabilityTimeSlotDB = await dbContext.AvailabilityTimeSlots
                    .Include(una => una.UnavailableTimeSlots)
                    .Include(a => a.Assistant)
                        .ThenInclude(ass => ass!.UserAccount)
                        .ThenInclude(assc => assc!.UserInformation)
                    .FirstOrDefaultAsync(a => a.Uuid == uuid);

            if (availabilityTimeSlotDB == null)
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
                    Id = availabilityTimeSlotDB.Assistant.IdUserAccount,
                    PhoneNumber = availabilityTimeSlotDB.Assistant.UserAccount.UserInformation.PhoneNumber,
                    Email = availabilityTimeSlotDB.Assistant.UserAccount.Email,
                },
                UnavailableTimeSlots = availabilityTimeSlotDB.UnavailableTimeSlots?.Select(a => new BusinessLogicLayer.Model.UnavailableTimeSlot
                {
                    StartTime = a.StartTime!.Value,
                    EndTime = a.EndTime!.Value
                }).ToList()
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

        public async Task<bool> UpdateAvailabilityTimeSlotAsync(BusinessLogicLayer.Model.AvailabilityTimeSlot availabilityTimeSlot)
        {
            bool isUpdated = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var existingSlot = await dbContext.AvailabilityTimeSlots
                    .Include(slot => slot.UnavailableTimeSlots)
                    .FirstOrDefaultAsync(slot => slot.Id == availabilityTimeSlot.Id);

                if (existingSlot == null)
                {
                    return false;
                }

                existingSlot.Date = availabilityTimeSlot.Date;
                existingSlot.StartTime = availabilityTimeSlot.StartTime;
                existingSlot.EndTime = availabilityTimeSlot.EndTime;

                dbContext.UnavailableTimeSlots.RemoveRange(existingSlot.UnavailableTimeSlots!);
                if (availabilityTimeSlot.UnavailableTimeSlots?.Count > 0)
                {
                    List<UnavailableTimeSlot> unavailableTimeSlotsDb = availabilityTimeSlot.UnavailableTimeSlots.Select(una => new UnavailableTimeSlot
                    {
                        StartTime = una.StartTime,
                        EndTime = una.EndTime,
                        IdAvailabilityTimeSlot = existingSlot.Id,
                    }).ToList();
                    dbContext.UnavailableTimeSlots.AddRange(unavailableTimeSlotsDb);
                }

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

        public async Task<int> GetAppointmentsScheduledCountByClientUuidAsync(int idClient)
        {
            using var dbContext = context.CreateDbContext();
            var count = await dbContext.Appointments
                .Where(a => a.IdClient == idClient && (a.Status == Model.Types.AppointmentStatusType.CONFIRMED || a.Status == Model.Types.AppointmentStatusType.SCHEDULED || a.Status == Model.Types.AppointmentStatusType.RESCHEDULED)).CountAsync();
            return count;
        }



        public async Task<List<int>> GetServiceOfferIdsByServiceIdAsync(int idService)
        {
            using var dbContext = context.CreateDbContext();
            var servicesOfferIds = await dbContext.ServiceOffers
                .Where(sero => sero.IdService == idService)
                .Select(ser => ser.Id)
                .ToListAsync();
            return servicesOfferIds;
        }

        public async Task<List<int>> GetServiceOfferIdsByAssistantIdAsync(int idAssistant)
        {
            using var dbContext = context.CreateDbContext();
            var servicesOfferIds = await dbContext.ServiceOffers
                .Where(sero => sero.IdAssistant == idAssistant)
                .Select(ser => ser.Id)
                .ToListAsync();
            return servicesOfferIds;
        }




        public async Task<List<BusinessLogicLayer.Model.Appointment>> GetScheduledOrConfirmedAppointmentsOfAsssistantByIdAsync(int idAssistant)
        {
            using var dbContext = context.CreateDbContext();

            var dbAppointments = await dbContext.Appointments
                .Include(a => a.ScheduledServices!)
                    .ThenInclude(ss => ss.ServiceOffer)
                    .ThenInclude(se => se!.Assistant)
                    .ThenInclude(ac => ac!.UserAccount)
                    .ThenInclude(ui => ui!.UserInformation)
                .Include(cl => cl.Client)
                    .ThenInclude(ac => ac!.UserAccount)
                    .ThenInclude(ui => ui!.UserInformation)
            .Where(app => (app.Status == Model.Types.AppointmentStatusType.SCHEDULED || app.Status == Model.Types.AppointmentStatusType.CONFIRMED || app.Status == Model.Types.AppointmentStatusType.RESCHEDULED) &&
                            app.ScheduledServices!.Any(ax => ax.ServiceOffer!.Assistant!.IdUserAccount == idAssistant)
            ).ToListAsync();

            var businessAppointments = dbAppointments.Select(dbApp => new BusinessLogicLayer.Model.Appointment
            {
                Id = dbApp.Id,
                Uuid = dbApp.Uuid,
                Date = dbApp.Date,
                Status = (BusinessLogicLayer.Model.Types.AppointmentStatusType)dbApp.Status!.Value,
                StartTime = dbApp.StartTime,
                EndTime = dbApp.EndTime,
                TotalCost = dbApp.TotalCost,
                Client = new BusinessLogicLayer.Model.Client
                {
                    Id = dbApp.Client!.IdUserAccount,
                    Uuid = dbApp.Client.UserAccount!.Uuid,
                    PhoneNumber = dbApp.Client.UserAccount.UserInformation!.PhoneNumber,
                    Email = dbApp.Client.UserAccount.Email
                },
                ScheduledServices = dbApp.ScheduledServices!.Select(ss => new BusinessLogicLayer.Model.ScheduledService
                {
                    Id = ss.Id,
                    Uuid = ss.Uuid,
                    ServiceStartTime = ss.ServiceStartTime,
                    ServiceEndTime = ss.ServiceEndTime,
                    ServicePrice = ss.ServicePrice,
                    ServicesMinutes = ss.ServicesMinutes,
                    ServiceName = ss.ServiceName,
                    ServiceOffer = new BusinessLogicLayer.Model.ServiceOffer
                    {
                        Id = ss.ServiceOffer!.Id,
                        Uuid = ss.ServiceOffer.Uuid,
                        Status = (BusinessLogicLayer.Model.Types.ServiceOfferStatusType?)ss.ServiceOffer.Status,
                        Assistant = new BusinessLogicLayer.Model.Assistant
                        {
                            Id = ss.ServiceOffer.Assistant!.IdUserAccount,
                            Uuid = ss.ServiceOffer.Assistant!.UserAccount!.Uuid,
                            Name = ss.ServiceOffer.Assistant!.UserAccount!.UserInformation!.Name,
                            PhoneNumber = ss.ServiceOffer.Assistant!.UserAccount!.UserInformation.PhoneNumber,
                            Email = ss.ServiceOffer.Assistant!.UserAccount!.Email,
                        }
                    }
                }).ToList()
            }).ToList();
            return businessAppointments;
        }


        public async Task<List<BusinessLogicLayer.Model.Appointment>> GetScheduledOrConfirmedAppointmentsOfClientByIdAsync(int idClient)
        {
            using var dbContext = context.CreateDbContext();

            var dbAppointments = await dbContext.Appointments
                .Include(a => a.ScheduledServices!)
                    .ThenInclude(ss => ss.ServiceOffer)
                    .ThenInclude(se => se!.Assistant)
                    .ThenInclude(ac => ac!.UserAccount)
                    .ThenInclude(ui => ui!.UserInformation)
                .Include(cl => cl.Client)
                    .ThenInclude(ac => ac!.UserAccount)
            .Where(app => (app.Status == Model.Types.AppointmentStatusType.SCHEDULED || app.Status == Model.Types.AppointmentStatusType.CONFIRMED || app.Status == Model.Types.AppointmentStatusType.RESCHEDULED) &&
                            app.IdClient == idClient
            ).ToListAsync();

            var businessAppointments = dbAppointments.Select(dbApp => new BusinessLogicLayer.Model.Appointment
            {
                Id = dbApp.Id,
                Date = dbApp.Date,
                Uuid = dbApp.Uuid,
                Status = (BusinessLogicLayer.Model.Types.AppointmentStatusType)dbApp.Status!.Value,
                StartTime = dbApp.StartTime,
                EndTime = dbApp.EndTime,
                TotalCost = dbApp.TotalCost,
                Client = new BusinessLogicLayer.Model.Client
                {
                    Id = dbApp.Client!.IdUserAccount,
                    Uuid = dbApp.Client.UserAccount!.Uuid
                },
                ScheduledServices = dbApp.ScheduledServices!.Select(ss => new BusinessLogicLayer.Model.ScheduledService
                {
                    Id = ss.Id,
                    Uuid = ss.Uuid,
                    ServiceStartTime = ss.ServiceStartTime,
                    ServiceEndTime = ss.ServiceEndTime,
                    ServicePrice = ss.ServicePrice,
                    ServicesMinutes = ss.ServicesMinutes,
                    ServiceName = ss.ServiceName,
                    ServiceOffer = new BusinessLogicLayer.Model.ServiceOffer
                    {
                        Id = ss.ServiceOffer!.Id,
                        Uuid = ss.ServiceOffer.Uuid,
                        Status = (BusinessLogicLayer.Model.Types.ServiceOfferStatusType?)ss.ServiceOffer.Status,
                        Assistant = new BusinessLogicLayer.Model.Assistant
                        {
                            Id = ss.ServiceOffer.Assistant!.IdUserAccount,
                            Uuid = ss.ServiceOffer.Assistant!.UserAccount!.Uuid,
                            Name = ss.ServiceOffer.Assistant!.UserAccount!.UserInformation!.Name,
                            PhoneNumber = ss.ServiceOffer.Assistant!.UserAccount!.UserInformation.PhoneNumber,
                            Email = ss.ServiceOffer.Assistant!.UserAccount!.Email,

                        }
                    }
                }).ToList()
            }).ToList();
            return businessAppointments;
        }



        public async Task<bool> UpdateAppointmentAsync(BusinessLogicLayer.Model.Appointment appointment)
        {
            bool isUpdated = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var dbAppointment = await dbContext.Appointments
                    .Include(a => a.ScheduledServices)
                    .FirstOrDefaultAsync(app => app.Id!.Value == appointment.Id!.Value);

                if (dbAppointment != null)
                {
                    dbAppointment.Date = appointment.Date;
                    dbAppointment.StartTime = appointment.StartTime;
                    dbAppointment.EndTime = appointment.EndTime;
                    dbAppointment.TotalCost = appointment.TotalCost;
                    dbAppointment.Status = Model.Types.AppointmentStatusType.RESCHEDULED;
                    dbAppointment.IdClient = appointment.Client!.Id;

                    dbContext.ScheduledServices.RemoveRange(dbAppointment.ScheduledServices!);

                    foreach (var scheduledService in appointment.ScheduledServices!)
                    {
                        var scheduleServices = new ScheduledService
                        {
                            IdAppointment = dbAppointment.Id,
                            IdServiceOffer = scheduledService.ServiceOffer!.Id,
                            ServiceStartTime = scheduledService.ServiceStartTime!.Value,
                            ServiceEndTime = scheduledService.ServiceEndTime!.Value,
                            ServicePrice = scheduledService.ServicePrice!.Value,
                            ServicesMinutes = scheduledService.ServicesMinutes!.Value,
                            ServiceName = scheduledService.ServiceName,
                            Uuid = scheduledService.Uuid!.Value
                        };
                        dbContext.ScheduledServices.Add(scheduleServices);
                    }
                }
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

        public async Task<List<BusinessLogicLayer.Model.ServiceOffer>> GetServiceOffersByAssistantIdAsync(int idAssistant)
        {
            using var dbContext = context.CreateDbContext();
            var dbServiceOffers = await dbContext.ServiceOffers
                    .Include(ser => ser.Service)
                    .Where(a => a.IdAssistant == idAssistant)
                    .ToListAsync();

            if (dbServiceOffers == null)
            {
                return [];
            }

            var serviceOffers = dbServiceOffers.Select(so => new BusinessLogicLayer.Model.ServiceOffer
            {
                Id = so.Id,
                Uuid = so.Uuid,
                Status = (BusinessLogicLayer.Model.Types.ServiceOfferStatusType)so.Status,
                Service = new BusinessLogicLayer.Model.Service
                {
                    Id = so.Service!.Id,
                    Uuid = so.Service.Uuid,
                    Price = so.Service.Price,
                    Name = so.Service.Name,
                    Minutes = so.Service.Minutes,
                    Description = so.Service.Description
                },
            }).ToList();
            return serviceOffers;
        }

        public async Task<List<BusinessLogicLayer.Model.ServiceOffer>> GetServiceOffersByServiceIdAsync(int idService)
        {
            using var dbContext = context.CreateDbContext();
            var dbServiceOffers = await dbContext.ServiceOffers
                    .Include(ser => ser.Service)
                    .Where(a => a.IdService == idService)
                    .ToListAsync();

            if (dbServiceOffers == null)
            {
                return [];
            }

            var serviceOffers = dbServiceOffers.Select(so => new BusinessLogicLayer.Model.ServiceOffer
            {
                Id = so.Id,
                Uuid = so.Uuid,
                Status = (BusinessLogicLayer.Model.Types.ServiceOfferStatusType)so.Status,
                Service = new BusinessLogicLayer.Model.Service
                {
                    Id = so.Service!.Id,
                    Uuid = so.Service.Uuid,
                    Price = so.Service.Price,
                    Name = so.Service.Name,
                    Minutes = so.Service.Minutes,
                    Description = so.Service.Description
                },
            }).ToList();
            return serviceOffers;
        }

        public async Task<List<int>> GetScheduledOrConfirmedAppoinmentsIdsOfClientByIdAsync(int idClient)
        {
            using var dbContext = context.CreateDbContext();

            var dbAppointments = await dbContext.Appointments
             .Where(a => (a.Status == Model.Types.AppointmentStatusType.SCHEDULED || a.Status == Model.Types.AppointmentStatusType.CONFIRMED || a.Status == Model.Types.AppointmentStatusType.RESCHEDULED) && a.IdClient == idClient)
             .Select(a => a.Id!.Value)
             .ToListAsync();

            if (dbAppointments == null)
            {
                return [];
            }

            return dbAppointments;
        }

        public async Task<List<BusinessLogicLayer.Model.Appointment>> GetScheduledOrConfirmedAppointmentsOfAsssistantByIdAndRangeAsync(int idAssistant, BusinessLogicLayer.Model.Types.DateTimeRange range)
        {
            using var dbContext = context.CreateDbContext();

            var dbAppointments = await dbContext.Appointments
                .Include(a => a.ScheduledServices!)
                    .ThenInclude(ss => ss.ServiceOffer)
                    .ThenInclude(se => se!.Assistant)
                    .ThenInclude(ac => ac!.UserAccount)
                    .ThenInclude(ui => ui!.UserInformation)
                .Include(cl => cl.Client)
                    .ThenInclude(ac => ac!.UserAccount)
            .Where(app => (app.Status == Model.Types.AppointmentStatusType.SCHEDULED || app.Status == Model.Types.AppointmentStatusType.CONFIRMED || app.Status == Model.Types.AppointmentStatusType.RESCHEDULED) &&
                            app.Date == range.Date &&
                            app.ScheduledServices!.Any(ax => ax.ServiceOffer!.Assistant!.IdUserAccount == idAssistant &&
                                                                               (ax.ServiceStartTime < range.EndTime && ax.ServiceEndTime > range.StartTime)
                            )
            ).ToListAsync();

            var businessAppointments = dbAppointments.Select(dbApp => new BusinessLogicLayer.Model.Appointment
            {
                Id = dbApp.Id,
                Date = dbApp.Date,
                Uuid = dbApp.Uuid,
                Status = (BusinessLogicLayer.Model.Types.AppointmentStatusType)dbApp.Status!.Value,
                StartTime = dbApp.StartTime,
                EndTime = dbApp.EndTime,
                TotalCost = dbApp.TotalCost,
                Client = new BusinessLogicLayer.Model.Client
                {
                    Id = dbApp.Client!.IdUserAccount,
                    Uuid = dbApp.Client!.UserAccount!.Uuid
                },
                ScheduledServices = dbApp.ScheduledServices!.Select(ss => new BusinessLogicLayer.Model.ScheduledService
                {
                    Id = ss.Id,
                    Uuid = ss.Uuid,
                    ServiceStartTime = ss.ServiceStartTime,
                    ServiceEndTime = ss.ServiceEndTime,
                    ServicePrice = ss.ServicePrice,
                    ServicesMinutes = ss.ServicesMinutes,
                    ServiceName = ss.ServiceName,
                    ServiceOffer = new BusinessLogicLayer.Model.ServiceOffer
                    {
                        Id = ss.ServiceOffer!.Id,
                        Uuid = ss.ServiceOffer.Uuid,
                        Status = (BusinessLogicLayer.Model.Types.ServiceOfferStatusType?)ss.ServiceOffer.Status,
                        Assistant = new BusinessLogicLayer.Model.Assistant
                        {
                            Id = ss.ServiceOffer.Assistant!.IdUserAccount,
                            Uuid = ss.ServiceOffer.Assistant!.UserAccount!.Uuid,
                            Name = ss.ServiceOffer.Assistant!.UserAccount!.UserInformation!.Name,
                            PhoneNumber = ss.ServiceOffer.Assistant!.UserAccount!.UserInformation.PhoneNumber,
                            Email = ss.ServiceOffer.Assistant!.UserAccount!.Email,
                        }
                    }
                }).ToList()
            }).ToList();
            return businessAppointments;
        }
    }
}