using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;

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