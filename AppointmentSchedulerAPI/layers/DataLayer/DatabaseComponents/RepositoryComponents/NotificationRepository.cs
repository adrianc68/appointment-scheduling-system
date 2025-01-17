using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using NotificationBase = AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.NotificationBase;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDbContextFactory<Model.AppointmentDbContext> context;

        public NotificationRepository(IDbContextFactory<AppointmentDbContext> context)
        {
            this.context = context;
        }

        public async Task<bool> CreateNotification(BusinessLogicLayer.Model.NotificationBase notification)
        {
            bool isRegistered = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var notificationDB = new NotificationBase
                {
                    Uuid = notification.Uuid!.Value,
                    CreatedAt = notification.CreatedAt!.Value,
                    Message = notification.Message,
                    Type = (Model.Types.NotificationType?)notification.Type,
                };

                await dbContext.NotificationBases.AddAsync(notificationDB);
                await dbContext.SaveChangesAsync();

                if (notification is BusinessLogicLayer.Model.AppointmentNotification appointmentNotification)
                {
                    var appointmentDB = new AppointmentNotification
                    {
                        IdAppointment = appointmentNotification.Appointment!.Id!.Value,
                        IdNotificationBase = notificationDB.Id!.Value,
                        Code = (Model.Types.AppointmentNotificationCodeType)appointmentNotification.Code!.Value,
                    };
                    await dbContext.AppointmentNotifications.AddAsync(appointmentDB);
                    await dbContext.SaveChangesAsync();

                }
                else if (notification is BusinessLogicLayer.Model.SystemNotification systemNotification)
                {
                    var systemDB = new SystemNotification
                    {
                        IdNotificationBase = notificationDB.Id!.Value,
                        Severity = (Model.Types.SystemNotificationSeverityCodeType?)systemNotification.Severity!.Value,
                        Code = (Model.Types.SystemNotificationCodeType?)systemNotification.Code!.Value,
                    };
                    await dbContext.SystemNotifications.AddAsync(systemDB);
                    await dbContext.SaveChangesAsync();
                }
                else if (notification is BusinessLogicLayer.Model.GeneralNotification generalNotification)
                {
                    var generalDB = new GeneralNotification
                    {
                        IdNotificationBase = notificationDB.Id!.Value,
                        Code = (Model.Types.GeneralNotificationCodeType?)generalNotification.Code!.Value,
                    };
                    await dbContext.GeneralNotifications.AddAsync(generalDB);
                    await dbContext.SaveChangesAsync();
                }

                List<NotificationRecipient> recipients = notification.Recipients!.Select(recipient => new NotificationRecipient
                {
                    IdNotificationBase = notificationDB.Id,
                    IdUserAccount = recipient.Id,
                    Status = (Model.Types.NotificationStatusType?)recipient.Status,
                    ChangedAt = DateTime.UtcNow
                }).ToList();
                await dbContext.NotificationRecipients.AddRangeAsync(recipients);
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

        public async Task<bool> ChangeNotificationStatusByNotificationUuid(Guid uuid, BusinessLogicLayer.Model.Types.Notification.NotificationStatusType status)
        {
            bool isStatusChanged = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var notificationDB = await dbContext.NotificationRecipients
                    .Include(nb => nb.NotificationBase)
                     .FirstOrDefaultAsync(ac => ac.NotificationBase!.Uuid == uuid);

                if (notificationDB == null)
                {
                    return false;
                }

                notificationDB!.Status = (Model.Types.NotificationStatusType)status;
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

        public async Task<IEnumerable<BusinessLogicLayer.Model.NotificationBase>> GetNotificationsByAccountUuid(Guid uuid)
        {
            IEnumerable<BusinessLogicLayer.Model.NotificationBase> notifications = [];
            using var dbContext = context.CreateDbContext();

            var notificationDB = await dbContext.UserAccounts
                .Where(ua => ua.Uuid == uuid)
                .Include(a => a.NotificationRecipients)
                    .ThenInclude(nb => nb!.NotificationBase)
                    .ThenInclude(napp => napp!.AppointmentNotification)
                    .ThenInclude(app => app!.Appointment)
                .Include(ua => ua.NotificationRecipients)
                    .ThenInclude(nr => nr.NotificationBase)
                    .ThenInclude(nb => nb!.SystemNotification)
                .Include(ua => ua.NotificationRecipients)
                    .ThenInclude(nr => nr.NotificationBase)
                    .ThenInclude(nb => nb!.GeneralNotification)
                .Where(ua => ua.Uuid == uuid)
                .FirstOrDefaultAsync();

            if (notificationDB != null)
            {
                notifications = notificationDB.NotificationRecipients.Select<NotificationRecipient, BusinessLogicLayer.Model.NotificationBase>(notification =>
                {
                    var notificationUserRecipients = new List<BusinessLogicLayer.Model.NotificationRecipient>
                        {
                            new BusinessLogicLayer.Model.NotificationRecipient
                            {
                                Uuid = notification.UserAccount!.Uuid!.Value,
                                Id = notification.UserAccount!.Id!.Value
                            }
                        };
                    if (notification.NotificationBase!.Type == Model.Types.NotificationType.APPOINTMENT_NOTIFICATION)
                    {
                        return new BusinessLogicLayer.Model.AppointmentNotification
                        {
                            Status = (BusinessLogicLayer.Model.Types.Notification.NotificationStatusType)notification.Status!.Value,
                            Uuid = notification.NotificationBase.Uuid,
                            Id = notification.NotificationBase.Id,
                            CreatedAt = notification.NotificationBase.CreatedAt,
                            Message = notification.NotificationBase.Message!,
                            Code = (BusinessLogicLayer.Model.Types.Notification.AppointmentNotificationCodeType?)notification.NotificationBase.AppointmentNotification!.Code!.Value,
                            Recipients = notificationUserRecipients,
                            Appointment = new BusinessLogicLayer.Model.Appointment
                            {
                                Uuid = notification.NotificationBase.AppointmentNotification!.Appointment!.Uuid,
                                Id = notification.NotificationBase.AppointmentNotification!.Appointment.Id
                            },
                        };
                    }
                    else if (notification.NotificationBase!.Type == Model.Types.NotificationType.SYSTEM_NOTIFICATION)
                    {
                        return new BusinessLogicLayer.Model.SystemNotification
                        {
                            Status = (BusinessLogicLayer.Model.Types.Notification.NotificationStatusType?)notification.Status!.Value,
                            Uuid = notification.NotificationBase.Uuid,
                            Id = notification.NotificationBase.Id,
                            CreatedAt = notification.NotificationBase.CreatedAt,
                            Message = notification.NotificationBase.Message!,
                            Code = (BusinessLogicLayer.Model.Types.Notification.SystemNotificationCodeType?)notification.NotificationBase.SystemNotification!.Code!.Value,
                            Recipients = notificationUserRecipients,
                            Severity = (BusinessLogicLayer.Model.Types.Notification.SystemNotificationSeverityCodeType?)notification.NotificationBase.SystemNotification!.Severity!.Value
                        };
                    }
                    else if (notification.NotificationBase!.Type == Model.Types.NotificationType.GENERAL_NOTIFICATION)
                    {
                        return new BusinessLogicLayer.Model.GeneralNotification
                        {
                            Status = (BusinessLogicLayer.Model.Types.Notification.NotificationStatusType?)notification.Status!.Value,
                            Uuid = notification.NotificationBase.Uuid,
                            Id = notification.NotificationBase.Id,
                            CreatedAt = notification.NotificationBase.CreatedAt,
                            Message = notification.NotificationBase.Message!,
                            Code = (BusinessLogicLayer.Model.Types.Notification.GeneralNotificationCodeType?)notification.NotificationBase.GeneralNotification!.Code!.Value,
                            Recipients = notificationUserRecipients,
                        };
                    }
                    throw new NotImplementedException($"Unhandled notification type: {notification.GetType().Name}, {notification.IdNotificationBase} + : {notification.IdUserAccount}");
                });
            }
            return notifications;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.NotificationBase>> GetUnreadNotificationsByAccountUuid(Guid uuid)
        {
            IEnumerable<BusinessLogicLayer.Model.NotificationBase> notifications = [];
            using var dbContext = context.CreateDbContext();

            var notificationDB = await dbContext.UserAccounts
                .Where(ua => ua.Uuid == uuid)
                .Include(ua => ua.NotificationRecipients)
                    .ThenInclude(nr => nr.NotificationBase)
                    .ThenInclude(nb => nb!.AppointmentNotification)
                    .ThenInclude(app => app!.Appointment)
                .Include(ua => ua.NotificationRecipients)
                    .ThenInclude(nr => nr.NotificationBase)
                    .ThenInclude(nb => nb!.SystemNotification)
                .Include(ua => ua.NotificationRecipients)
                    .ThenInclude(nr => nr.NotificationBase)
                    .ThenInclude(nb => nb!.GeneralNotification)
                .SelectMany(ua => ua.NotificationRecipients)
                .Where(nr => nr.Status == Model.Types.NotificationStatusType.UNREAD)
                .ToListAsync();


            notifications = notificationDB.Select<NotificationRecipient, BusinessLogicLayer.Model.NotificationBase>(notification =>
            {
                var notificationUserRecipients = new List<BusinessLogicLayer.Model.NotificationRecipient>
                {
                    new BusinessLogicLayer.Model.NotificationRecipient
                    {
                        Uuid = notification.UserAccount!.Uuid!.Value,
                        Id = notification.UserAccount!.Id!.Value
                    }
                };

                return notification.NotificationBase!.Type switch
                {
                    Model.Types.NotificationType.APPOINTMENT_NOTIFICATION => new BusinessLogicLayer.Model.AppointmentNotification
                    {
                        Status = (BusinessLogicLayer.Model.Types.Notification.NotificationStatusType?)notification.Status!.Value,
                        Uuid = notification.NotificationBase.Uuid,
                        Id = notification.NotificationBase.Id,
                        CreatedAt = notification.NotificationBase.CreatedAt,
                        Message = notification.NotificationBase.Message!,
                        Code = (BusinessLogicLayer.Model.Types.Notification.AppointmentNotificationCodeType?)notification.NotificationBase.AppointmentNotification!.Code!.Value,
                        Recipients = notificationUserRecipients,
                        Appointment = new BusinessLogicLayer.Model.Appointment
                        {
                            Uuid = notification.NotificationBase.AppointmentNotification!.Appointment!.Uuid,
                            Id = notification.NotificationBase.AppointmentNotification!.Appointment.Id
                        },
                    },
                    Model.Types.NotificationType.SYSTEM_NOTIFICATION => new BusinessLogicLayer.Model.SystemNotification
                    {
                        Status = (BusinessLogicLayer.Model.Types.Notification.NotificationStatusType?)notification.Status!.Value,
                        Uuid = notification.NotificationBase.Uuid,
                        Id = notification.NotificationBase.Id,
                        CreatedAt = notification.NotificationBase.CreatedAt,
                        Message = notification.NotificationBase.Message!,
                        Code = (BusinessLogicLayer.Model.Types.Notification.SystemNotificationCodeType?)notification.NotificationBase.SystemNotification!.Code!.Value,
                        Recipients = notificationUserRecipients,
                        Severity = (BusinessLogicLayer.Model.Types.Notification.SystemNotificationSeverityCodeType?)notification.NotificationBase.SystemNotification!.Severity!.Value
                    },
                    Model.Types.NotificationType.GENERAL_NOTIFICATION => new BusinessLogicLayer.Model.GeneralNotification
                    {
                        Status = (BusinessLogicLayer.Model.Types.Notification.NotificationStatusType?)notification.Status!.Value,
                        Uuid = notification.NotificationBase.Uuid,
                        Id = notification.NotificationBase.Id,
                        CreatedAt = notification.NotificationBase.CreatedAt,
                        Message = notification.NotificationBase.Message!,
                        Code = (BusinessLogicLayer.Model.Types.Notification.GeneralNotificationCodeType?)notification.NotificationBase.GeneralNotification!.Code!.Value,
                        Recipients = notificationUserRecipients,
                    },
                    _ => throw new NotImplementedException($"Unhandled notification type: {notification.NotificationBase.Type}")
                };
            });
            return notifications;
        }
    }
}