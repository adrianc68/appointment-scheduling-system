using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface INotificationMgt
    {
        Task<Guid?> CreateNotification(Notification notification);
        Task<List<Notification>> GetNotificationsByAccountUuid(Guid uuid);
        Task<List<Notification>> GetUnreadNotificationsByAccountUuid(Guid uuid);
        Task<bool> ChangeNotificationStatusByNotificationUuid(Guid uuid, Guid accountUuid, NotificationStatusType status);
    }
}