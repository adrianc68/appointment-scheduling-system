using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces
{
    public interface IGetNotification
    {
        Task<List<Notification>> GetNotificationsByAccountUuidAsync(Guid uuid);
        Task<List<Notification>> GetUnreadNotificationsByAccountUuidAsync(Guid uuid);
    }
}