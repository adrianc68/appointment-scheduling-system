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
                    StartDate = appointment.StartDate,
                    EndDate = appointment.EndDate,
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
                        ServiceStartDate = scheduledService.ServiceStartDate!.Value,
                        ServiceEndDate = scheduledService.ServiceEndDate!.Value,
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
                    StartDate = availabilityTimeSlot.StartDate,
                    EndDate = availabilityTimeSlot.EndDate,
                    IdAssistant = availabilityTimeSlot.Assistant!.Id,
                    Status = Model.Types.AvailabilityTimeSlotStatusType.ENABLED
                };
                dbContext.AvailabilityTimeSlots.Add(timeSlot);
                await dbContext.SaveChangesAsync();

                if (availabilityTimeSlot.UnavailableTimeSlots != null)
                {
                    List<UnavailableTimeSlot> unavailableTimeSlotsDb = availabilityTimeSlot.UnavailableTimeSlots.Select(una => new UnavailableTimeSlot
                    {
                        StartDate = una.StartDate,
                        EndDate = una.EndDate,
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
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            using var dbContext = context.CreateDbContext();
            var appointmentDB = await dbContext.Appointments
                .Where(app => app.StartDate >= startDateTime && app.StartDate <= endDateTime && (app.Status == Model.Types.AppointmentStatusType.CONFIRMED || app.Status == Model.Types.AppointmentStatusType.SCHEDULED || app.Status == Model.Types.AppointmentStatusType.RESCHEDULED))
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
                StartDate = app.StartDate,
                EndDate = app.EndDate,
                TotalCost = app.TotalCost,
                Uuid = app.Uuid,
                Status = (BusinessLogicLayer.Model.Types.AppointmentStatusType)app.Status!.Value,
                Id = app.Id,
                CreatedAt = app.CreatedAt,
                Client = new BusinessLogicLayer.Model.Client
                {
                    Uuid = app.Client!.UserAccount!.Uuid,
                    Id = app.Client.IdUserAccount
                },
                ScheduledServices = app.ScheduledServices!.Select(aso => new BusinessLogicLayer.Model.ScheduledService
                {
                    ServiceStartDate = aso.ServiceStartDate,
                    ServiceEndDate = aso.ServiceEndDate,
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

            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);


            var availableServices = await dbContext.AvailabilityTimeSlots
                .Where(slot => slot.StartDate >= startDateTime && slot.EndDate <= endDateTime && slot.Status == Model.Types.AvailabilityTimeSlotStatusType.ENABLED)
                    .Include(a => a.Assistant)
                        .ThenInclude(ass => ass!.UserAccount)
                        .ThenInclude(assc => assc!.UserInformation)
                    .Include(una => una.UnavailableTimeSlots)
                .ToListAsync();

            var availabilityTimeSlotsModel = availableServices
                .Select(slot => new BusinessLogicLayer.Model.AvailabilityTimeSlot
                {
                    Id = slot.Id,
                    Uuid = slot.Uuid,
                    StartDate = slot.StartDate,
                    EndDate = slot.EndDate,
                    Status = (BusinessLogicLayer.Model.Types.AvailabilityTimeSlotStatusType)slot.Status,
                    Assistant = new BusinessLogicLayer.Model.Assistant
                    {
                        Name = slot.Assistant!.UserAccount!.UserInformation!.Name,
                        Uuid = slot.Assistant.UserAccount.Uuid,
                        Id = slot.Assistant.IdUserAccount,
                        PhoneNumber = slot.Assistant!.UserAccount!.UserInformation!.Name,
                        Email = slot.Assistant!.UserAccount!.UserInformation!.Name,
                    },
                    UnavailableTimeSlots = slot.UnavailableTimeSlots?.Select(unav => new BusinessLogicLayer.Model.UnavailableTimeSlot
                    {
                        StartDate = unav.StartDate,
                        EndDate = unav.EndDate
                    }).ToList() ?? [],
                })
                .ToList();

            return availabilityTimeSlotsModel;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.ServiceOffer>> GetAvailableServicesAsync(DateOnly date)
        {
            var startDateTime = date.ToDateTime(TimeOnly.MinValue);
            var endDateTime = date.ToDateTime(TimeOnly.MaxValue);

            using var dbContext = context.CreateDbContext();

            var availableServices = await dbContext.AvailabilityTimeSlots
                .Where(slot => slot.StartDate >= startDateTime && slot.StartDate <= endDateTime)
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
                // 🔑 aquí eliminamos duplicados
                .DistinctBy(so => so.Uuid)
                .ToList();

            return serviceOffers;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.Types.DateTimeRange>> GetAppointmentDateTimeRangeConflictsByRangeAsync(BusinessLogicLayer.Model.Types.DateTimeRange range)
        {
            using var dbContext = context.CreateDbContext();
            var conflicts = await dbContext.Appointments
     .Where(a => a.StartDate == range.StartDate
                 && a.StartDate < range.EndDate
                 && a.EndDate > range.EndDate)
     .ToListAsync();

            if (conflicts == null)
            {
                return [];
            }

            List<BusinessLogicLayer.Model.Types.DateTimeRange> conflictingRanges = conflicts.Select(a => new BusinessLogicLayer.Model.Types.DateTimeRange
            {
                StartDate = a.StartDate,
                EndDate = a.EndDate,
            }).ToList();

            return conflictingRanges;
        }

        public async Task<bool> IsAppointmentTimeSlotAvailableAsync(BusinessLogicLayer.Model.Types.DateTimeRange range)
        {
            PropToString.PrintData(range);

            using var dbContext = context.CreateDbContext();
            bool isAvailable = await dbContext.Appointments
         .AnyAsync(a =>
             a.StartDate < range.EndDate &&
             a.EndDate > range.StartDate
         );


            return !isAvailable;
        }

        public async Task<bool> IsAvailabilityTimeSlotRegisteredAsync(BusinessLogicLayer.Model.Types.DateTimeRange range, int idAssistant)
        {
            using var dbContext = context.CreateDbContext();
            bool isAvailable = await dbContext.AvailabilityTimeSlots
         .AnyAsync(a =>
             a.IdAssistant == idAssistant &&
             a.StartDate < range.EndDate &&
             a.EndDate > range.StartDate && a.Status == Model.Types.AvailabilityTimeSlotStatusType.ENABLED || a.Status == Model.Types.AvailabilityTimeSlotStatusType.DISABLED
         );

            return !isAvailable;
        }

        public async Task<bool> IsAssistantAvailableInAvailabilityTimeSlotsAsync(
            BusinessLogicLayer.Model.Types.DateTimeRange range, int idAssistant)
        {
            using var dbContext = context.CreateDbContext();

            var coveringSlots = await dbContext.AvailabilityTimeSlots
                .Include(slot => slot.UnavailableTimeSlots)
                .Where(slot => slot.IdAssistant == idAssistant
                               && slot.Status == Model.Types.AvailabilityTimeSlotStatusType.ENABLED
                               && slot.StartDate <= range.StartDate
                               && slot.EndDate >= range.EndDate)
                .ToListAsync();

            foreach (var slot in coveringSlots)
            {
                if (slot.UnavailableTimeSlots!.Any(uts =>
                    uts.StartDate < range.EndDate && uts.EndDate > range.StartDate))
                {
                    return false;
                }
            }

            return coveringSlots.Any();
        }

        public async Task<bool> HasAssistantConflictingAppoinmentsAsync(BusinessLogicLayer.Model.Types.DateTimeRange range, int idAssistant)
        {
            using var dbContext = context.CreateDbContext();

            var scheduledServices = await dbContext.ScheduledServices
    .Include(ss => ss.ServiceOffer)
    .Where(ss => ss.ServiceOffer!.IdAssistant == idAssistant)
    .ToListAsync();



            var conflictingServices = scheduledServices
                .Where(ss => range.StartDate < ss.ServiceEndDate
                             && range.EndDate > ss.ServiceStartDate)
                .ToList();



            return conflictingServices.Any();
        }


        public async Task<IEnumerable<BusinessLogicLayer.Model.ScheduledService>> GetConflictingServicesByDateTimeRangeAsync(BusinessLogicLayer.Model.Types.DateTimeRange range)
        {
            using var dbContext = context.CreateDbContext();

            var rawConflicts = await dbContext.ScheduledServices
                .Where(aso =>
                    (aso.Appointment!.Status == Model.Types.AppointmentStatusType.SCHEDULED ||
                     aso.Appointment.Status == Model.Types.AppointmentStatusType.CONFIRMED ||
                     aso.Appointment.Status == Model.Types.AppointmentStatusType.RESCHEDULED) &&
                    range.StartDate < aso.ServiceEndDate &&
                    range.EndDate > aso.ServiceStartDate)
                .Include(aso => aso.ServiceOffer)
                    .ThenInclude(so => so!.Assistant)
                        .ThenInclude(a => a!.UserAccount)
                            .ThenInclude(u => u!.UserInformation)
                .ToListAsync();

            var conflicts = rawConflicts.Select(aso => new BusinessLogicLayer.Model.ScheduledService
            {
                Id = aso.ServiceOffer!.Id,
                Uuid = aso.ServiceOffer.Uuid,
                ServiceStartDate = aso.ServiceStartDate,
                ServiceEndDate = aso.ServiceEndDate,
                ServicePrice = aso.ServicePrice,
                ServiceName = aso.ServiceName,
                ServicesMinutes = aso.ServicesMinutes,
                ServiceOffer = new BusinessLogicLayer.Model.ServiceOffer
                {
                    Id = aso.ServiceOffer.Id,
                    Uuid = aso.ServiceOffer.Uuid,
                    Status = (BusinessLogicLayer.Model.Types.ServiceOfferStatusType)aso.ServiceOffer.Status,
                    Assistant = new BusinessLogicLayer.Model.Assistant
                    {
                        Uuid = aso.ServiceOffer.Assistant!.UserAccount!.Uuid,
                        Name = aso.ServiceOffer.Assistant.UserAccount.UserInformation!.Name,
                        Id = aso.ServiceOffer.Assistant.UserAccount.Id,
                        PhoneNumber = aso.ServiceOffer.Assistant.UserAccount.UserInformation!.PhoneNumber,
                        Email = aso.ServiceOffer.Assistant.UserAccount.Email,
                    }
                }
            }).ToList();

            return conflicts;
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
                StartDate = appointmentDB.StartDate,
                EndDate = appointmentDB.EndDate,
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
                StartDate = appointmentDB.StartDate,
                EndDate = appointmentDB.EndDate,
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
                    ServiceStartDate = aso.ServiceStartDate,
                    ServiceEndDate = aso.ServiceEndDate,
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
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            var appointmentDB = await dbContext.Appointments
                .Where(app => app.StartDate >= startDateTime && app.EndDate <= endDateTime)
                .Include(c => c.Client)
                    .ThenInclude(ua => ua!.UserAccount)
                    .ThenInclude(ui => ui!.UserInformation)
                .ToListAsync();

            var appointmentsModel = appointmentDB.Select(app => new BusinessLogicLayer.Model.Appointment
            {
                StartDate = app.StartDate,
                EndDate = app.EndDate,
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
                StartDate = availabilityTimeSlotDB.StartDate,
                EndDate = availabilityTimeSlotDB.EndDate,
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
                    StartDate = a.StartDate,
                    EndDate = a.EndDate
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
                StartDate = availabilityTimeSlotDB.StartDate,
                EndDate = availabilityTimeSlotDB.EndDate,
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
                    StartDate = a.StartDate,
                    EndDate = a.EndDate
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


                dbContext.UnavailableTimeSlots.RemoveRange(existingSlot.UnavailableTimeSlots!);
                if (availabilityTimeSlot.UnavailableTimeSlots?.Count > 0)
                {
                    List<UnavailableTimeSlot> unavailableTimeSlotsDb = availabilityTimeSlot.UnavailableTimeSlots.Select(una => new UnavailableTimeSlot
                    {
                        StartDate = una.StartDate,
                        EndDate = una.EndDate,
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

        public async Task<bool> HasAvailabilityTimeSlotConflictingSlotsAsync(
           BusinessLogicLayer.Model.Types.DateTimeRange range,
           int idAvailabilityTimeSlot,
           int idAssistant)
        {
            using var dbContext = context.CreateDbContext();

            var otherSlots = await dbContext.AvailabilityTimeSlots
                .Where(slot => slot.IdAssistant == idAssistant
                            && slot.Id != idAvailabilityTimeSlot
                            && slot.Status != Model.Types.AvailabilityTimeSlotStatusType.DELETED)
                .ToListAsync();

            bool hasConflict = otherSlots.Any(slot =>
                range.StartDate < slot.EndDate &&
                range.EndDate > slot.StartDate
            );

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
                StartDate = dbApp.StartDate,
                EndDate = dbApp.EndDate,
                Status = (BusinessLogicLayer.Model.Types.AppointmentStatusType)dbApp.Status!.Value,
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
                    ServiceStartDate = ss.ServiceStartDate,
                    ServiceEndDate = ss.ServiceEndDate,
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
                StartDate = dbApp.StartDate,
                EndDate = dbApp.EndDate,
                Uuid = dbApp.Uuid,
                Status = (BusinessLogicLayer.Model.Types.AppointmentStatusType)dbApp.Status!.Value,
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
                    ServiceStartDate = ss.ServiceStartDate,
                    ServiceEndDate = ss.ServiceEndDate,
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
                    dbAppointment.StartDate = appointment.StartDate;
                    dbAppointment.EndDate = appointment.EndDate;
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
                            ServiceStartDate = scheduledService.ServiceStartDate!.Value,
                            ServiceEndDate = scheduledService.ServiceEndDate!.Value,
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
                            app.StartDate == range.StartDate &&
                            app.ScheduledServices!.Any(ax => ax.ServiceOffer!.Assistant!.IdUserAccount == idAssistant &&
                                                                               (ax.ServiceStartDate < range.EndDate && ax.ServiceEndDate > range.EndDate)
                            )
            ).ToListAsync();

            var businessAppointments = dbAppointments.Select(dbApp => new BusinessLogicLayer.Model.Appointment
            {
                Id = dbApp.Id,
                StartDate = dbApp.StartDate,
                EndDate = dbApp.EndDate,
                Uuid = dbApp.Uuid,
                Status = (BusinessLogicLayer.Model.Types.AppointmentStatusType)dbApp.Status!.Value,
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
                    ServiceStartDate = ss.ServiceStartDate,
                    ServiceEndDate = ss.ServiceEndDate,
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