using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDbContextFactory<Model.AppointmentDbContext> context;

        public NotificationRepository(IDbContextFactory<AppointmentDbContext> context)
        {
            this.context = context;
        }

        public async Task<bool> CreateNotification(BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.NotificationBase notification)
        {
            bool isRegistered = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                if (notification is BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.AppointmentNotification appointmentNotification)
                {
                    var notificationDB = new NotificationBase
                    {
                        Uuid = appointmentNotification.Uuid!.Value,
                        CreatedAt = appointmentNotification.CreatedAt!.Value,
                        Message = appointmentNotification.Message,
                        Code = (Model.Types.NotificationCodeType?)appointmentNotification.Code,
                        Type = (Model.Types.NotificationType?)appointmentNotification.Type,
                    };

                    await dbContext.NotificationBases.AddAsync(notificationDB);
                    await dbContext.SaveChangesAsync();

                    var appointmentDB = new AppointmentNotification
                    {
                        IdAppointment = appointmentNotification.Appointment!.Id!.Value,
                        IdNotificationBase = notificationDB.Id!.Value
                    };
                    await dbContext.AppointmentNotifications.AddAsync(appointmentDB);
                    await dbContext.SaveChangesAsync();

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
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return isRegistered;
        }

        public async Task<bool> ChangeNotificationStatusByNotificationUuid(Guid uuid, BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types.NotificationStatusType status)
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

        public async Task<IEnumerable<BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.NotificationBase>> GetNotificationsByAccountUuid(Guid uuid)
        {
            IEnumerable<BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.NotificationBase> notifications = [];
            using var dbContext = context.CreateDbContext();

            var notificationDB = await dbContext.UserAccounts
                .Where(ua => ua.Uuid == uuid)
                .Include(a => a.NotificationRecipients)
                    .ThenInclude(nb => nb!.NotificationBase)
                    .ThenInclude(napp => napp!.AppointmentNotification)
                    .ThenInclude(app => app!.Appointment)
                .Where(ua => ua.Uuid == uuid)
                .FirstOrDefaultAsync();

            if (notificationDB != null)
            {
                notifications = notificationDB.NotificationRecipients.Select(notification =>
                {
                    if (notification.NotificationBase!.Type == Model.Types.NotificationType.APPOINTMENT_NOTIFICATION)
                    {
                        var notificationUserRecipients = new List<BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.NotificationRecipient>
                        {
                            new BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.NotificationRecipient
                            {
                                Uuid = notification.UserAccount!.Uuid!.Value,
                                Id = notification.UserAccount!.Id!.Value
                            }
                        };


                        return new BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.AppointmentNotification
                        {
                            Status = (BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types.NotificationStatusType)notification.Status!.Value,
                            Uuid = notification.NotificationBase.Uuid,
                            Id = notification.NotificationBase.Id,
                            CreatedAt = notification.NotificationBase.CreatedAt,
                            Message = notification.NotificationBase.Message!,
                            Code = (BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types.NotificationCodeType)notification.NotificationBase.Code!.Value,
                            Recipients = notificationUserRecipients,
                            Appointment = new BusinessLogicLayer.Model.Appointment
                            {
                                Uuid = notification.NotificationBase.AppointmentNotification!.Appointment!.Uuid,
                                Id = notification.NotificationBase.AppointmentNotification!.Appointment.Id
                            },
                        };
                    }
                    throw new NotImplementedException($"Unhandled notification type: {notification.GetType().Name}, {notification.IdNotificationBase} + : {notification.IdUserAccount}");
                });
            }
            return notifications;
        }

        public async Task<IEnumerable<BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.NotificationBase>> GetUnreadNotificationsByAccountUuid(Guid uuid)
        {
            IEnumerable<BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.NotificationBase> notifications = [];
            using var dbContext = context.CreateDbContext();

            var notificationDB = await dbContext.UserAccounts
               .Where(ua => ua.Uuid == uuid)
               .Include(a => a.NotificationRecipients)
                   .ThenInclude(nb => nb!.NotificationBase)
                   .ThenInclude(napp => napp!.AppointmentNotification)
                   .ThenInclude(app => app!.Appointment)
               .Where(ua => ua.Uuid == uuid)
               .SelectMany(ua => ua.NotificationRecipients)
                .Where(nr => nr.Status == Model.Types.NotificationStatusType.UNREAD)
               .ToListAsync();

            notifications = notificationDB!.Select(notification =>
            {
                if (notification.NotificationBase!.Type == Model.Types.NotificationType.APPOINTMENT_NOTIFICATION)
                {
                    var notificationUserRecipients = new List<BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.NotificationRecipient>
                    {
                        new BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.NotificationRecipient
                        {
                            Uuid = notification.UserAccount!.Uuid!.Value,
                            Id = notification.UserAccount!.Id!.Value
                        }
                    };

                    return new BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.AppointmentNotification
                    {
                        Status = (BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types.NotificationStatusType)notification.Status!.Value,
                        Uuid = notification.NotificationBase.Uuid,
                        Id = notification.NotificationBase.Id,
                        CreatedAt = notification.NotificationBase.CreatedAt,
                        Message = notification.NotificationBase.Message!,
                        Code = (BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types.NotificationCodeType)notification.NotificationBase.Code!.Value,
                        Recipients = notificationUserRecipients,
                        Appointment = new BusinessLogicLayer.Model.Appointment
                        {
                            Uuid = notification.NotificationBase.AppointmentNotification!.Appointment!.Uuid,
                            Id = notification.NotificationBase.AppointmentNotification!.Appointment.Id
                        },
                    };

                }
                throw new NotImplementedException($"Unhandled notification type: {notification.GetType().Name}, {notification.IdNotificationBase} + : {notification.IdUserAccount}");
            });
            return notifications;
        }
    }
}