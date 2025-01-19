using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces
{
    public interface INotificationRepository
    {
        Task<bool> CreateNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsByAccountUuidAsync(Guid uuid);
        Task<IEnumerable<Notification>> GetUnreadNotificationsByAccountUuidAsync(Guid uuid);
         Task<bool> IsNotificationRegisteredBysUuidAndAccountUuidAsync(Guid uuid, Guid accountUuid);
        Task<bool> ChangeNotificationStatusByNotificationUuidAsync(Guid uuid, Guid accountUuid, NotificationStatusType status);
    }
}