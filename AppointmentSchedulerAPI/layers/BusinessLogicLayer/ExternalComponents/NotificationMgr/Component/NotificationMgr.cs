using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Component
{
    public class NotificationMgr : INotificationMgt
    {
        private readonly List<INotifier> notifiers;

        public NotificationMgr(IEnumerable<INotifier> notifiers)
        {
            this.notifiers = notifiers.ToList();
        }

        public void Suscribe(INotifier notifier)
        {
            notifiers.Add(notifier);
        }

        public async Task NotifyToUserAsync(string recipient, string message)
        {
            var tasks = notifiers.Select(notifier => notifier.SendToUserAsync(recipient, message));
            await Task.WhenAll(tasks);
        }

        public async Task NotifyAllAsync(string message)
        {
            var tasks = notifiers.Select(notifier => notifier.SendToAllAsync(message));
            await Task.WhenAll(tasks);
        }

        public async Task NotifyToGroupAsync(string groupName, string message)
        {
            var tasks = notifiers.Select(notifier => notifier.SendToGroupAsync(groupName, message));
            await Task.WhenAll(tasks);
        }
    }
}