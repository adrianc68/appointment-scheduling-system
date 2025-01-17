using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces
{
    public interface IGetNotification
    {
        Task<List<NotificationBase>> GetNotificationsByAccountUuid(Guid uuid);
        Task<List<NotificationBase>> GetUnreadNotificationsByAccountUuid(Guid uuid);
    }
}