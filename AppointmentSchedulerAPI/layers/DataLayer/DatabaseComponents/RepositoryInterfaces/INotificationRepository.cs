using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface INotificationRepository
    {
        Task<bool> CreateNotification(NotificationBase notification);
        Task<IEnumerable<NotificationBase>> GetNotificationsByAccountUuid(Guid uuid);
        Task<IEnumerable<NotificationBase>> GetUnreadNotificationsByAccountUuid(Guid uuid);
        Task<bool> ChangeNotificationStatusByNotificationUuid(Guid uuid, NotificationStatusType status);
    }
}