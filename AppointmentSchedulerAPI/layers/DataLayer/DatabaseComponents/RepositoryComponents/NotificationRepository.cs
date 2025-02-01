using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
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

        public async Task<bool> CreateNotificationAsync(BusinessLogicLayer.Model.Notification notification)
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
                        IdAppointment = appointmentNotification.Appointment!.Id!,
                        IdNotificationBase = notificationDB.Id!.Value,
                        Code = (Model.Types.AppointmentNotificationCodeType)appointmentNotification.Code,
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
                        Code = (Model.Types.SystemNotificationCodeType?)systemNotification.Code,
                    };
                    await dbContext.SystemNotifications.AddAsync(systemDB);
                    await dbContext.SaveChangesAsync();
                }
                else if (notification is BusinessLogicLayer.Model.GeneralNotification generalNotification)
                {
                    var generalDB = new GeneralNotification
                    {
                        IdNotificationBase = notificationDB.Id!.Value,
                        Code = (Model.Types.GeneralNotificationCodeType)generalNotification.Code!,
                    };
                    await dbContext.GeneralNotifications.AddAsync(generalDB);
                    await dbContext.SaveChangesAsync();
                }

                List<NotificationRecipient> recipients = notification.Recipients!.Select(recipient => new NotificationRecipient
                {
                    IdNotificationBase = notificationDB.Id,
                    IdUserAccount = recipient.RecipientData.UserAccountId,
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

        public async Task<bool> ChangeNotificationStatusByNotificationUuidAsync(Guid uuid, Guid accountUuid, BusinessLogicLayer.Model.Types.Notification.NotificationStatusType status)
        {
            bool isStatusChanged = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var notificationDB = await dbContext.NotificationRecipients
                    .Include(nb => nb.NotificationBase)
                    .Include(ac => ac.UserAccount)
                     .FirstOrDefaultAsync(ac => ac.NotificationBase!.Uuid == uuid && ac.UserAccount!.Uuid == accountUuid);

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

        public async Task<bool> IsNotificationRegisteredBysUuidAndAccountUuidAsync(Guid uuid, Guid accountUuid)
        {
            using var dbContext = context.CreateDbContext();

            var notificationExists = await dbContext.NotificationRecipients
                .Include(a => a.NotificationBase)
                .Include(a => a.UserAccount)
                .Where(a => a.UserAccount!.Uuid == accountUuid && a.NotificationBase!.Uuid == uuid)
                .FirstOrDefaultAsync();

            return notificationExists != null;
        }


        public async Task<IEnumerable<BusinessLogicLayer.Model.Notification>> GetNotificationsByAccountUuidAsync(Guid uuid)
        {
            IEnumerable<BusinessLogicLayer.Model.Notification> notifications = [];
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

                notifications = notificationDB.NotificationRecipients.Select<NotificationRecipient, BusinessLogicLayer.Model.Notification>(notification =>
                {
                    PropToString.PrintData(notification);
                    // var recipientdata = new BusinessLogicLayer.Model.NotificationRecipient
                    // {
                    //     RecipientData = new BusinessLogicLayer.Model.NotificationRecipientData
                    //     {
                    //         Email = notification.UserAccount!.Email!,
                    //         UserAccountId = notification.UserAccount.Id!.Value,
                    //         UserAccountUuid = notification.UserAccount.Uuid!.Value,
                    //         PhoneNumber = notification.UserAccount.UserInformation!.PhoneNumber!
                    //     },
                    //     Status = (BusinessLogicLayer.Model.Types.Notification.NotificationStatusType?)notification.Status!.Value,
                    //     ChangedAt = notification.ChangedAt!.Value
                    // };


                    if (notification.NotificationBase!.Type == Model.Types.NotificationType.APPOINTMENT_NOTIFICATION)
                    {
                        return new BusinessLogicLayer.Model.AppointmentNotification
                        {
                            Uuid = notification.NotificationBase.Uuid,
                            Id = notification.NotificationBase.Id,
                            CreatedAt = notification.NotificationBase.CreatedAt,
                            Message = notification.NotificationBase.Message!,
                            Type = (BusinessLogicLayer.Model.Types.Notification.NotificationType)notification.NotificationBase.Type.Value,
                            Code = (BusinessLogicLayer.Model.Types.Notification.AppointmentNotificationCodeType)notification.NotificationBase.AppointmentNotification!.Code!.Value,
                            // Recipients = [recipientdata],
                            Recipients = [],
                            Appointment = new BusinessLogicLayer.Model.AppointmentIdentifiers
                            {
                                Uuid = notification.NotificationBase.AppointmentNotification!.Appointment!.Uuid!.Value,
                                Id = notification.NotificationBase.AppointmentNotification!.Appointment.Id!.Value
                            },
                            Options = new NotificationOptions
                            {
                                Channels = []
                            }
                        };
                    }
                    else if (notification.NotificationBase!.Type == Model.Types.NotificationType.SYSTEM_NOTIFICATION)
                    {
                        return new BusinessLogicLayer.Model.SystemNotification
                        {
                            Uuid = notification.NotificationBase.Uuid,
                            Id = notification.NotificationBase.Id,
                            CreatedAt = notification.NotificationBase.CreatedAt,
                            Message = notification.NotificationBase.Message!,
                            Type = (BusinessLogicLayer.Model.Types.Notification.NotificationType)notification.NotificationBase.Type.Value,
                            Code = (BusinessLogicLayer.Model.Types.Notification.SystemNotificationCodeType)notification.NotificationBase.SystemNotification!.Code!.Value,
                            Recipients = [],
                            Severity = (BusinessLogicLayer.Model.Types.Notification.SystemNotificationSeverityCodeType?)notification.NotificationBase.SystemNotification!.Severity!.Value,
                            Options = new NotificationOptions
                            {
                                Channels = []
                            }
                        };
                    }
                    else if (notification.NotificationBase!.Type == Model.Types.NotificationType.GENERAL_NOTIFICATION)
                    {
                        return new BusinessLogicLayer.Model.GeneralNotification
                        {
                            Uuid = notification.NotificationBase.Uuid,
                            Id = notification.NotificationBase.Id,
                            CreatedAt = notification.NotificationBase.CreatedAt,
                            Message = notification.NotificationBase.Message!,
                            Type = (BusinessLogicLayer.Model.Types.Notification.NotificationType)notification.NotificationBase.Type.Value,
                            Code = (BusinessLogicLayer.Model.Types.Notification.GeneralNotificationCodeType)notification.NotificationBase.GeneralNotification!.Code!.Value,
                            Recipients = [],
                            Options = new NotificationOptions
                            {
                                Channels = []
                            }
                        };
                    }
                    throw new NotImplementedException($"Unhandled notification type: {notification.GetType().Name}, {notification.IdNotificationBase} + : {notification.IdUserAccount}");
                }).ToList();
            }
            return notifications;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.Notification>> GetUnreadNotificationsByAccountUuidAsync(Guid uuid)
        {
            IEnumerable<BusinessLogicLayer.Model.Notification> notifications = [];
            using var dbContext = context.CreateDbContext();

            var dataClient = await dbContext.UserAccounts
                .Where(ua => ua.Uuid == uuid)
                .FirstOrDefaultAsync();

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

            if (notificationDB != null)
            {

                notifications = notificationDB.Select<NotificationRecipient, BusinessLogicLayer.Model.Notification>(notification =>
                {

                    // var recipientdata = new BusinessLogicLayer.Model.NotificationRecipient
                    // {
                    //     RecipientData = new BusinessLogicLayer.Model.NotificationRecipientData
                    //     {
                    //         Email = notification.UserAccount!.Email!,
                    //         UserAccountId = notification.UserAccount.Id!.Value,
                    //         UserAccountUuid = notification.UserAccount.Uuid!.Value,
                    //         PhoneNumber = notification.UserAccount.UserInformation!.PhoneNumber!
                    //     },
                    //     Status = (BusinessLogicLayer.Model.Types.Notification.NotificationStatusType?)notification.Status!.Value,
                    //     ChangedAt = notification.ChangedAt!.Value
                    // };


                    return notification.NotificationBase!.Type switch
                    {
                        Model.Types.NotificationType.APPOINTMENT_NOTIFICATION => new BusinessLogicLayer.Model.AppointmentNotification
                        {
                            Uuid = notification.NotificationBase.Uuid,
                            Id = notification.NotificationBase.Id,
                            CreatedAt = notification.NotificationBase.CreatedAt,
                            Message = notification.NotificationBase.Message!,
                            Type = (BusinessLogicLayer.Model.Types.Notification.NotificationType)notification.NotificationBase.Type.Value,
                            Code = (BusinessLogicLayer.Model.Types.Notification.AppointmentNotificationCodeType)notification.NotificationBase.AppointmentNotification!.Code!.Value,
                            Appointment = new BusinessLogicLayer.Model.AppointmentIdentifiers
                            {
                                Uuid = notification.NotificationBase.AppointmentNotification!.Appointment!.Uuid!.Value,
                                Id = notification.NotificationBase.AppointmentNotification!.Appointment.Id!.Value
                            },
                            // Recipients = [recipientdata],
                            Recipients = [],
                            Options = new NotificationOptions
                            {
                                Channels = []
                            }
                        },
                        Model.Types.NotificationType.SYSTEM_NOTIFICATION => new BusinessLogicLayer.Model.SystemNotification
                        {
                            Uuid = notification.NotificationBase.Uuid,
                            Id = notification.NotificationBase.Id,
                            CreatedAt = notification.NotificationBase.CreatedAt,
                            Message = notification.NotificationBase.Message!,
                            Type = (BusinessLogicLayer.Model.Types.Notification.NotificationType)notification.NotificationBase.Type.Value,
                            Code = (BusinessLogicLayer.Model.Types.Notification.SystemNotificationCodeType)notification.NotificationBase.SystemNotification!.Code!.Value,
                            Severity = (BusinessLogicLayer.Model.Types.Notification.SystemNotificationSeverityCodeType?)notification.NotificationBase.SystemNotification!.Severity!.Value,
                            // Recipients = [recipientdata],
                            Recipients = [],
                            Options = new NotificationOptions
                            {
                                Channels = []
                            }
                        },
                        Model.Types.NotificationType.GENERAL_NOTIFICATION => new BusinessLogicLayer.Model.GeneralNotification
                        {
                            Uuid = notification.NotificationBase.Uuid,
                            Id = notification.NotificationBase.Id,
                            CreatedAt = notification.NotificationBase.CreatedAt,
                            Message = notification.NotificationBase.Message!,
                            Type = (BusinessLogicLayer.Model.Types.Notification.NotificationType)notification.NotificationBase.Type.Value,
                            Code = (BusinessLogicLayer.Model.Types.Notification.GeneralNotificationCodeType)notification.NotificationBase.GeneralNotification!.Code!.Value,
                            // Recipients = [recipientdata],
                            Recipients = [],
                            Options = new NotificationOptions
                            {
                                Channels = []
                            }
                        },
                        _ => throw new NotImplementedException($"Unhandled notification type: {notification.NotificationBase.Type}")
                    };
                }).ToList();
            }
            return notifications;
        }


    }
}