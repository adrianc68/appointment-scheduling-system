using System.Text.Json;
using System.Text.Json.Serialization;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.AccountMgr.Interfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;

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

        public async Task<Guid?> CreateNotification(NotificationBase notification, NotificationUsersToSendType recipientOptions = NotificationUsersToSendType.SEND_TO_SOME_USERS, List<NotificationChannelType>? channels = null)
        {


            notification.Status = NotificationStatusType.UNREAD;
            notification.Uuid = Guid.CreateVersion7();

            if (recipientOptions == NotificationUsersToSendType.SEND_TO_EVERYONE)
            {
                List<(int, Guid)> accountsIds = await accountMgr.GetAllAccountsIdsAndUuids();
                notification.Recipients = accountsIds.Select(x => new NotificationRecipient
                {
                    Id = x.Item1,
                    Uuid = x.Item2,
                    Status = NotificationStatusType.UNREAD,
                    ChangedAt = DateTime.UtcNow
                }).ToList();
            }

            bool isRegistered = await notificationRepository.CreateNotification(notification);

            if (!isRegistered)
            {
                notification.Uuid = null;
                return null;
            }

            if (channels == null || channels.Count == 0)
            {
                channels = [NotificationChannelType.WEB_APPLICATION];
            }

            if (notification is AppointmentNotification appointmentNotification)
            {
                AppointmentNotificationDTO notificationDTO = new AppointmentNotificationDTO
                {
                    CreatedAt = notification.CreatedAt!.Value,
                    Uuid = notification.Uuid.Value,
                    Status = notification.Status.Value,
                    Message = notification.Message!,
                    Type = notification.Type!.Value,
                    Code = appointmentNotification.Code!.Value,
                    Appointment = new AppointmentUuidDTO
                    {
                        Uuid = appointmentNotification.Appointment!.Uuid!.Value
                    }
                };
                SendNotificationToUsers(notification.Recipients!, notificationDTO, channels);
            }
            else if (notification is SystemNotification systemNotification)
            {
                SystemNotificationDTO notificationDTO = new SystemNotificationDTO
                {
                    CreatedAt = notification.CreatedAt!.Value,
                    Uuid = notification.Uuid.Value,
                    Status = notification.Status.Value,
                    Message = notification.Message!,
                    Type = notification.Type!.Value,
                    Code = systemNotification.Code!.Value,
                    Severity = systemNotification.Severity!.Value
                };
                SendNotificationToUsers(notification.Recipients!, notificationDTO, channels);
            }
            else if (notification is GeneralNotification generalNotification)
            {
                GeneralNotificationDTO notificationDTO = new GeneralNotificationDTO
                {
                    CreatedAt = notification.CreatedAt!.Value,
                    Uuid = notification.Uuid.Value,
                    Status = notification.Status.Value,
                    Message = notification.Message!,
                    Type = notification.Type!.Value,
                    Code = generalNotification.Code!.Value,
                };
                SendNotificationToUsers(notification.Recipients!, notificationDTO, channels);
            }

            return notification.Uuid.Value;
        }

        public async Task<List<NotificationBase>> GetNotificationsByAccountUuid(Guid uuid)
        {
            List<NotificationBase> notifications = (List<NotificationBase>)await notificationRepository.GetNotificationsByAccountUuid(uuid);
            return notifications;
        }

        public async Task<List<NotificationBase>> GetUnreadNotificationsByAccountUuid(Guid uuid)
        {
            List<NotificationBase> notifications = (List<NotificationBase>)await notificationRepository.GetUnreadNotificationsByAccountUuid(uuid);
            return notifications;
        }

        public async Task<bool> ChangeNotificationStatusByNotificationUuid(Guid uuid, NotificationStatusType status)
        {
            bool isStatusChanged = await notificationRepository.ChangeNotificationStatusByNotificationUuid(uuid, status);
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

        private async void SendNotificationToUsers<TNotification>(List<NotificationRecipient> users, TNotification dto, List<NotificationChannelType> channels)
        {
            string notificationJson = SerializeObjectToJson(dto);

            foreach (var recipient in users)
            {
                foreach (var channel in channels)
                {
                    switch (channel)
                    {
                        case NotificationChannelType.WEB_APPLICATION:
                            await webNotifier.SendToUserAsync(recipient.Uuid.ToString(), notificationJson);
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