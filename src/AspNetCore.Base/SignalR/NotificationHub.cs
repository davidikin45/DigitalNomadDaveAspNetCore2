using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Base.SignalR
{
    public class NotificationHub : Hub
    {
        //Client JS methods

        //public async Task SendMessageToClients(string message, params string[] connectionIds)
        //{
        //    await Clients.Clients(connectionIds.ToList()).SendAsync("ReceiveMessage", message);
        //}

        //public async Task SendMessageToUsers(string message, params string[] userIds)
        //{
        //    await Clients.Users(userIds.ToList()).SendAsync("ReceiveMessage", message);
        //}

        //public async Task SendMessageToAllUsers(string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", message);
        //}

        //public Task SendMessageToGroups(string message, params string[] groups)
        //{
        //    return Clients.Groups(groups.ToList()).SendAsync("ReceiveMessage", message);
        //}

        public override async Task OnConnectedAsync()
        {
            var roles = Context.User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                       .Select(c => c.Value)
                       .ToList();

            foreach (var role in roles)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, role);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var roles = Context.User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                     .Select(c => c.Value)
                     .ToList();

            foreach (var role in roles)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, role);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

    public static class NotificationHubServerExtensions
    {
        public static async Task SendMessageToClients(this IHubContext<NotificationHub> hubContext, string message, params string[] connectionIds)
        {
            await hubContext.Clients.Clients(connectionIds.ToList()).SendAsync("ReceiveMessage", message);
        }

        public static async Task SendMessageToUsers(this IHubContext<NotificationHub> hubContext, string message, params string[] userIds)
        {
            await hubContext.Clients.Users(userIds.ToList()).SendAsync("ReceiveMessage", message);
        }

        public static async Task SendMessageToAllUsers(this IHubContext<NotificationHub> hubContext, string message)
        {
            await hubContext.Clients.All.SendAsync("ReceiveMessage", message);
        }

        public static Task SendMessageToGroups(this IHubContext<NotificationHub> hubContext, string message, params string[] groups)
        {
            return hubContext.Clients.Groups(groups.ToList()).SendAsync("ReceiveMessage", message);
        }
    }
}
