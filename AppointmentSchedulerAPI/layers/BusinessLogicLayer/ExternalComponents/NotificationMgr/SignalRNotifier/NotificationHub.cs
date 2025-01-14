using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces;
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

        public void AddUser()
        {

            var connectionId = Context.ConnectionId;
            var claims = ClaimsPOCO.GetUserClaims(Context.User!);

            if (!userConnections.ContainsKey(claims.Uuid.ToString()))
            {
                userConnections.Add(claims.Uuid.ToString(), connectionId);
            }
            else
            {
                userConnections[claims.Uuid.ToString()] = connectionId;
            }
            System.Console.WriteLine($"User {claims.Uuid.ToString()} connected with connectionId {connectionId}");
        }

        public override Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name;
            var claims = ClaimsPOCO.GetUserClaims(Context.User!);
            userConnections[claims.Uuid.ToString()] = Context.ConnectionId;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var claims = ClaimsPOCO.GetUserClaims(Context.User!);
            userConnections.Remove(claims.Uuid.ToString());
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