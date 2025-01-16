using System.Text.Json;
using System.Text.Json.Serialization;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Component
{
    public class NotificationMgr : INotificationMgt
    {
        private readonly INotificationRepository notificationRepository;
        private readonly IWebNotifier webNotifier;

        public NotificationMgr(INotificationRepository notificationRepository, IWebNotifier webNotifier)
        {
            this.notificationRepository = notificationRepository;
            this.webNotifier = webNotifier;
        }

        // public async Task NotifyToUserAsync(string recipient, string message)
        // {
        //     var tasks = notifiers.Select(notifier => notifier.SendToUserAsync(recipient, message));
        //     await Task.WhenAll(tasks);
        // }

        // public async Task NotifyAllAsync(string message)
        // {
        //     var tasks = notifiers.Select(notifier => notifier.SendToAllAsync(message));
        //     await Task.WhenAll(tasks);
        // }

        // public async Task NotifyToGroupAsync(string groupName, string message)
        // {
        //     var tasks = notifiers.Select(notifier => notifier.SendToGroupAsync(groupName, message));
        //     await Task.WhenAll(tasks);
        // }

        private string SerializeObjectToJson<T>(T data)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() } 
            };
            return JsonSerializer.Serialize(data, options);
        }

        public async Task<Guid?> CreateNotification(NotificationBase notification)
        {
            notification.Status = NotificationStatusType.UNREAD;
            notification.Uuid = Guid.CreateVersion7();
            bool isRegistered = await notificationRepository.CreateNotification(notification);

            PropToString.PrintData(notification);
            System.Console.WriteLine(isRegistered);

            if (!isRegistered)
            {
                notification.Uuid = null;
                return null;
            }

            if (notification.Type == NotificationType.APPOINTMENT_NOTIFICATION)
            {
                NotificationDto notificationDTO = new NotificationDto
                {
                    CreatedAt = notification.CreatedAt!.Value,
                    Uuid = notification.Uuid.Value,
                    Status = notification.Status.Value,
                    Message = notification.Message,
                    Code = notification.Code,
                    Type = notification.Type
                };
                await this.webNotifier.SendToUserAsync(notification.Recipient!.Uuid.ToString()!, SerializeObjectToJson<NotificationDto>(notificationDTO));
            }
            else if (notification.Type == NotificationType.SYSTEM_NOTIFICATION)
            {
                throw new NotImplementedException();
            }
            else if (notification.Type == NotificationType.GENERAL_NOTIFICATION)
            {
                throw new NotImplementedException();
            }
            else if (notification.Type == NotificationType.PAYMENT_NOTIFICATION)
            {
                throw new NotImplementedException();
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
    }
}