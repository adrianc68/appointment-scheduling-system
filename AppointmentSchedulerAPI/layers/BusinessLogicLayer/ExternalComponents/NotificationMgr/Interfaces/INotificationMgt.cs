using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces
{
    public interface INotificationMgt
    {
        Task<Guid?> CreateNotification(NotificationBase notification, NotificationUsersToSendType recipientOptions = NotificationUsersToSendType.SEND_TO_SOME_USERS, List<NotificationChannelType>? channels = null);
        Task<List<NotificationBase>> GetNotificationsByAccountUuid(Guid uuid);
        Task<List<NotificationBase>> GetUnreadNotificationsByAccountUuid(Guid uuid);
        Task<bool> ChangeNotificationStatusByNotificationUuid(Guid uuid, NotificationStatusType status);
    }
}