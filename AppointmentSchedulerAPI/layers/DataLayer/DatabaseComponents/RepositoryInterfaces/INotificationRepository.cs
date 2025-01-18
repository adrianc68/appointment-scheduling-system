using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface INotificationRepository
    {
        Task<bool> CreateNotification(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsByAccountUuid(Guid uuid);
        Task<IEnumerable<Notification>> GetUnreadNotificationsByAccountUuid(Guid uuid);
         Task<bool> IsNotificationRegisteredBysUuidAndAccountUuid(Guid uuid, Guid accountUuid);
        Task<bool> ChangeNotificationStatusByNotificationUuid(Guid uuid, Guid accountUuid, NotificationStatusType status);
    }
}