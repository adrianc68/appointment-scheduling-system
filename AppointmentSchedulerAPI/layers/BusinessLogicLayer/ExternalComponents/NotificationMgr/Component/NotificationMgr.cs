using System.Text.Json;
using System.Text.Json.Serialization;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;
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

        public async Task<Guid?> CreateNotification(NotificationBase notification)
        {
            notification.Status = NotificationStatusType.UNREAD;
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
                    Status = notification.Status.Value,
                    Message = notification.Message!,
                    Type = notification.Type!.Value,
                    Code = appointmentNotification.Code!.Value,
                    Appointment = new AppointmentUuidDTO
                    {
                        Uuid = appointmentNotification.Appointment!.Uuid!.Value
                    }
                };
                SendNotificationToUsers(notification.Recipients!, notificationDTO);
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
                SendNotificationToUsers(notification.Recipients!, notificationDTO);
                throw new NotImplementedException();
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
                SendNotificationToUsers(notification.Recipients!, notificationDTO);
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

        private async void SendNotificationToUsers(List<NotificationRecipient> users, NotificationDTO dto)
        {
            foreach (var recipient in users)
            {
                if (dto is AppointmentNotificationDTO appointmentNotificationDTO)
                {
                    await this.webNotifier.SendToUserAsync(recipient!.Uuid.ToString()!, SerializeObjectToJson(appointmentNotificationDTO));
                }
                else if (dto is SystemNotificationDTO systemNotificationDTO)
                {
                    await this.webNotifier.SendToAllAsync(SerializeObjectToJson(systemNotificationDTO));
                }
                else if (dto is GeneralNotificationDTO generalNotificationDTO)
                {
                    await this.webNotifier.SendToAllAsync(SerializeObjectToJson(generalNotificationDTO));
                }
            }
        }

    }
}