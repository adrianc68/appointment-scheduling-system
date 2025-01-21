using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface INotifier
    {
        Task SendToUserAsync(string recipient, NotificationDTO notification);
        Task SendToGroupAsync(string groupName, NotificationDTO notification);
        Task SendToAllAsync(NotificationDTO notification);

        Task AddToGroupAsync(string connectionId, string groupName);
        Task RemoveFromGroupAsync(string connectionId, string groupName);

    }
}