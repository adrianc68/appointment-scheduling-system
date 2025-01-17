using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface INotificationMgt
    {
        Task<Guid?> CreateNotification(NotificationBase notification, NotificationUsersToSendType recipientOptions = NotificationUsersToSendType.SEND_TO_SOME_USERS, List<NotificationChannelType>? channels = null);
        Task<List<NotificationBase>> GetNotificationsByAccountUuid(Guid uuid);
        Task<List<NotificationBase>> GetUnreadNotificationsByAccountUuid(Guid uuid);
        Task<bool> ChangeNotificationStatusByNotificationUuid(Guid uuid, Guid accountUuid, NotificationStatusType status);
    }
}