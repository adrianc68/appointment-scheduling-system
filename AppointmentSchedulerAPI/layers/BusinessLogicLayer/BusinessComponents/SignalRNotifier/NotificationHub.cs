using System.Text.Json;
using System.Text.Json.Serialization;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;
using Microsoft.AspNetCore.SignalR;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents.SignalRNotifier
{
    public class NotificationHub : Hub, IWebNotifier
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
            Console.WriteLine($"User {claims.Uuid.ToString()} connected with connectionId {connectionId}");
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

        public async Task SendToAllAsync(NotificationDTO message)
        {
            string messageData = SerializaNotification(message);
            await hubContext.Clients.All.SendAsync("ReceiveNotification", messageData);
        }

        public async Task SendToGroupAsync(string groupName, NotificationDTO message)
        {
            string messageData = SerializaNotification(message);
            await hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", messageData);
        }

        public async Task SendToUserAsync(string recipient, NotificationDTO message)
        {
            if (userConnections.TryGetValue(recipient, out var connectionId))
            {
                string messageData = SerializaNotification(message);
                await hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", messageData);
            }
            else
            {
                Console.WriteLine("User is not connected");
                // User is not connected!!! $$$>> Resolve this
            }
            await Task.CompletedTask;
        }


        public string SerializaNotification(object notificationDTO)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(),
                 },
            };

            string serializedNotification = JsonSerializer.Serialize(notificationDTO, options);
            return serializedNotification;
        }


    }
}