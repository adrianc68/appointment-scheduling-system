using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.SignalRNotifier
{
    public class NotificationHub : Hub, INotifier
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private static readonly Dictionary<string, string> _userConnections = new();


        public NotificationHub(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public override Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                _userConnections[username] = Context.ConnectionId;
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                _userConnections.Remove(username);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task AddToGroupAsync(string connectionId, string groupName)
        {
            await Groups.AddToGroupAsync(connectionId, groupName);
        }

        public async Task RemoveFromGroupAsync(string connectionId, string groupName)
        {
            await Groups.RemoveFromGroupAsync(connectionId, groupName);
        }

        public async Task SendToAllAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task SendToGroupAsync(string groupName, string message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", message);
        }

        public async Task SendToUserAsync(string recipient, string message)
        {
            if (_userConnections.TryGetValue(recipient, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
            else
            {
                // User is not connected!!! $$$>> Resolve this
            }
        }
    }
}