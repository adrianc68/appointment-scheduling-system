using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
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
                        Status = Model.Types.NotificationStatusType.UNREAD,
                        Uuid = appointmentNotification.Uuid!.Value,
                        CreatedAt = appointmentNotification.CreatedAt!.Value,
                        Message = appointmentNotification.Message,
                        Code = (Model.Types.NotificationCodeType?)appointmentNotification.Code,
                        Type = (Model.Types.NotificationType?)appointmentNotification.Type,
                        IdUserAccount = appointmentNotification.Recipient!.Id!.Value,
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
                var notificationDB = await dbContext.NotificationBases
                     .FirstOrDefaultAsync(ac => ac.Uuid == uuid);

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

            var notificationsDB = await dbContext.NotificationBases
                    .Include(n => n.UserAccount)
                    .Include(n => n.AppointmentNotification)
                        .ThenInclude(an => an!.Appointment)
                    .Where(n => n.UserAccount!.Uuid == uuid)
                    .ToListAsync();

            notifications = notificationsDB.Select(notification =>
            {
                if (notification.Type == Model.Types.NotificationType.APPOINTMENT_NOTIFICATION)
                {
                    return new BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.AppointmentNotification
                    {
                        Status = (BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types.NotificationStatusType)notification.Status!.Value,
                        Uuid = notification.Uuid,
                        Id = notification.Id,
                        CreatedAt = notification.CreatedAt,
                        Message = notification.Message!,
                        Code = (BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types.NotificationCodeType)notification.Code!.Value,
                        Recipient = new AccountData
                        {
                            Uuid = notification.UserAccount!.Uuid,
                            Id = notification.UserAccount!.Id
                        },
                        Appointment = new BusinessLogicLayer.Model.Appointment
                        {
                            Uuid = notification.AppointmentNotification!.Appointment!.Uuid,
                            Id = notification.AppointmentNotification!.Appointment.Id
                        },
                    };
                }
                throw new NotImplementedException($"Unhandled notification type: {notification.GetType().Name}");
            });
            return notifications;
        }

        public async Task<IEnumerable<BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.NotificationBase>> GetUnreadNotificationsByAccountUuid(Guid uuid)
        {
            IEnumerable<BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.NotificationBase> notifications = [];
            using var dbContext = context.CreateDbContext();

            var notificationsDB = await dbContext.NotificationBases
                    .Include(n => n.UserAccount)
                    .Include(n => n.AppointmentNotification)
                        .ThenInclude(an => an!.Appointment)
                    .Where(n => n.UserAccount!.Uuid == uuid && n.Status == Model.Types.NotificationStatusType.UNREAD)
                    .ToListAsync();

            notifications = notificationsDB.Select(notification =>
            {
                if (notification.Type == Model.Types.NotificationType.APPOINTMENT_NOTIFICATION)
                {
                    return new BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.AppointmentNotification
                    {
                        Status = (BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types.NotificationStatusType)notification.Status!.Value,
                        Uuid = notification.Uuid,
                        Id = notification.Id,
                        CreatedAt = notification.CreatedAt,
                        Message = notification.Message!,
                        Code = (BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types.NotificationCodeType)notification.Code!.Value,
                        Recipient = new AccountData
                        {
                            Uuid = notification.UserAccount!.Uuid,
                            Id = notification.UserAccount!.Id
                        },
                        Appointment = new BusinessLogicLayer.Model.Appointment
                        {
                            Uuid = notification.AppointmentNotification!.Appointment!.Uuid,
                            Id = notification.AppointmentNotification!.Appointment.Id
                        },
                    };
                }
                throw new NotImplementedException($"Unhandled notification type: {notification.GetType().Name}");
            });
            return notifications;
        }
    }
}