namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces
{
    public interface INotifier
    {
        Task SendToUserAsync(string recipient, string message);
        Task SendToGroupAsync(string groupName, string message);
        Task SendToAllAsync(string message);

        Task AddToGroupAsync(string connectionId, string groupName);
        Task RemoveFromGroupAsync(string connectionId, string groupName);

    }
}