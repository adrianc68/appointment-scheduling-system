using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using Microsoft.AspNetCore.SignalR;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.SignalRNotifier
{
    public class NotificationHub : Hub, INotifier
    {
        private readonly IHubContext<NotificationHub> hubContext;
        private static readonly Dictionary<string, string> userConnections = new();


        public NotificationHub(IHubContext<NotificationHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public void AddUser(string username)
        {
            var connectionId = Context.ConnectionId;
            if (!userConnections.ContainsKey(username))
            {
                userConnections.Add(username, connectionId);
            }
            else
            {
                userConnections[username] = connectionId; 
            }
            PropToString.PrintListData(userConnections);
            System.Console.WriteLine($"User {username} connected with connectionId {connectionId}");
        }

        public override Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                userConnections[username] = Context.ConnectionId;
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                userConnections.Remove(username);
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
            await hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task SendToGroupAsync(string groupName, string message)
        {
            await hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", message);
        }

        public async Task SendToUserAsync(string recipient, string message)
        {
            if (userConnections.TryGetValue(recipient, out var connectionId))
            {
                await hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
            else
            {
                System.Console.WriteLine("User is not connected");
                // User is not connected!!! $$$>> Resolve this
            }
            await Task.CompletedTask;
        }
    }
}