using api.Hubs.Clients;
using api.Models.ViewModels.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public override Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;

            _connections.Add(name, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public static ConnectionMapping<string> ConnectionIds => _connections;
    }
}
