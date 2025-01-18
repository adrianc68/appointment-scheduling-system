using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces
{
    public interface IGetNotification
    {
        Task<List<Notification>> GetNotificationsByAccountUuid(Guid uuid);
        Task<List<Notification>> GetUnreadNotificationsByAccountUuid(Guid uuid);
    }
}