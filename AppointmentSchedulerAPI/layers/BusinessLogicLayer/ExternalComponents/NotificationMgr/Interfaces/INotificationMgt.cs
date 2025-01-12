namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces
{
    public interface INotificationMgt
    {
        Task NotifyAllAsync(string message);
        Task NotifyToGroupAsync(string groupName, string message);
        Task NotifyToUserAsync(string recipient, string message);
        void Suscribe(INotifier notifier);
    }
}