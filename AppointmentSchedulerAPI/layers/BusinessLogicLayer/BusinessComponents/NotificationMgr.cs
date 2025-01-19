using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class NotificationMgr : INotificationMgt
    {
        private readonly INotificationRepository notificationRepository;
        private readonly IAccountMgt accountMgr;
        private readonly IWebNotifier webNotifier;

        public NotificationMgr(INotificationRepository notificationRepository, IWebNotifier webNotifier, IAccountMgt accountMgr)
        {
            this.notificationRepository = notificationRepository;
            this.webNotifier = webNotifier;
            this.accountMgr = accountMgr;
        }

        public async Task<Guid?> CreateNotification(Notification notification)
        {
            foreach (var recipient in notification.Recipients)
            {
                recipient.Status = NotificationStatusType.UNREAD;
            }

            if (notification.Options.Channels == null || notification.Options.Channels.Count == 0)
            {
                notification.Options.Channels = [NotificationChannelType.WEB_APPLICATION];
            }

            if (notification.Recipients.Count == 0)
            {
                List<AccountData> accountsIds = await accountMgr.GetAllAccountsIdsAndUuids();
                notification.Recipients = accountsIds.Select(x => new NotificationRecipient
                {
                    RecipientData = new NotificationRecipientData
                    {
                        Email = x.Email!,
                        UserAccountId = x.Id!.Value,
                        PhoneNumber = x.PhoneNumber!,
                        UserAccountUuid = x.Uuid!.Value
                    },
                    Status = NotificationStatusType.UNREAD,
                    ChangedAt = DateTime.UtcNow
                }).ToHashSet();
            }

            notification.Uuid = Guid.CreateVersion7();
            bool isRegistered = await notificationRepository.CreateNotification(notification);

            if (!isRegistered)
            {
                notification.Uuid = null;
                return null;
            }

            if (notification is AppointmentNotification appointmentNotification)
            {
                AppointmentNotificationDTO notificationDTO = new AppointmentNotificationDTO
                {
                    CreatedAt = notification.CreatedAt!.Value,
                    Uuid = notification.Uuid.Value,
                    Message = notification.Message,
                    Type = notification.Type,
                    Code = appointmentNotification.Code,
                    Appointment = new AppointmentUuidDTO
                    {
                        Uuid = appointmentNotification.Appointment!.Uuid!
                    }
                };
                SendNotificationToUsers(notificationDTO, notification.Recipients, notification.Options.Channels);
            }
            else if (notification is SystemNotification systemNotification)
            {
                SystemNotificationDTO notificationDTO = new SystemNotificationDTO
                {
                    CreatedAt = notification.CreatedAt!.Value,
                    Uuid = notification.Uuid.Value,
                    Message = notification.Message,
                    Type = notification.Type,
                    Code = systemNotification.Code,
                    Severity = systemNotification.Severity!.Value
                };
                SendNotificationToUsers(notificationDTO, notification.Recipients, notification.Options.Channels);
            }
            else if (notification is GeneralNotification generalNotification)
            {
                GeneralNotificationDTO notificationDTO = new GeneralNotificationDTO
                {
                    CreatedAt = notification.CreatedAt!.Value,
                    Uuid = notification.Uuid.Value,
                    Message = notification.Message,
                    Type = notification.Type,
                    Code = generalNotification.Code,
                };
                SendNotificationToUsers(notificationDTO, notification.Recipients, notification.Options.Channels);
            }
            return notification.Uuid.Value;
        }

        public async Task<List<Notification>> GetNotificationsByAccountUuid(Guid uuid)
        {
            List<Notification> notifications = (List<Notification>)await notificationRepository.GetNotificationsByAccountUuid(uuid);
            return notifications;
        }

        public async Task<List<Notification>> GetUnreadNotificationsByAccountUuid(Guid uuid)
        {
            List<Notification> notifications = (List<Notification>)await notificationRepository.GetUnreadNotificationsByAccountUuid(uuid);
            return notifications;
        }

        public async Task<bool> ChangeNotificationStatusByNotificationUuid(Guid uuid, Guid accountUuid, NotificationStatusType status)
        {
            bool isStatusChanged = await notificationRepository.ChangeNotificationStatusByNotificationUuid(uuid, accountUuid, status);
            return isStatusChanged;
        }

        private string SerializeObjectToJson<T>(T data)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(),
                 },
            };
            return JsonSerializer.Serialize(data, options);
        }

        private async void SendNotificationToUsers<TNotification>(TNotification dto, HashSet<NotificationRecipient> users, List<NotificationChannelType> channels)
            where TNotification : NotificationDTO
        {
            string notificationJson = SerializeObjectToJson(dto);

            foreach (var recipient in users)
            {
                foreach (var channel in channels)
                {
                    switch (channel)
                    {
                        case NotificationChannelType.WEB_APPLICATION:
                            await webNotifier.SendToUserAsync(recipient.RecipientData.UserAccountUuid.ToString(), dto);
                            break;
                        case NotificationChannelType.EMAIL:
                            throw new NotImplementedException();
                        // break;
                        case NotificationChannelType.SMS:
                            throw new NotImplementedException();
                            // break;
                    }
                }
            }
        }


    }
}