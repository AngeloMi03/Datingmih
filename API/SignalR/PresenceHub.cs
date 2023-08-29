using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly presenceTracker _Tracker;
        public PresenceHub(presenceTracker tracker)
        {
            _Tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            await _Tracker.UserConnected(Context.User!.GetUserName(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", Context.User!.GetUserName());

            var currentUser = _Tracker.GetOnlineUser();
            await Clients.All.SendAsync("GetOnlineUsers", currentUser);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _Tracker.UserDisconnected(Context.User!.GetUserName(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", Context.User!.GetUserName());

            var currentUser = _Tracker.GetOnlineUser();
            await Clients.All.SendAsync("GetOnlineUsers", currentUser);

            await base.OnDisconnectedAsync(exception);
        }
        
    }
}