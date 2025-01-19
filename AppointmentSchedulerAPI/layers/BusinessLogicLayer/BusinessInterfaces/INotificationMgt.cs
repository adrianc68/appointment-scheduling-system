using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Notification;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces
{
    public interface INotificationMgt
    {
        Task<Guid?> CreateNotificationAsync(Notification notification);
        Task<List<Notification>> GetNotificationsByAccountUuidAsync(Guid uuid);
        Task<List<Notification>> GetUnreadNotificationsByAccountUuidAsync(Guid uuid);
        Task<bool> ChangeNotificationStatusByNotificationUuidAsync(Guid uuid, Guid accountUuid, NotificationStatusType status);
    }
}