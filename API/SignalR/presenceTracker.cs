using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class presenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUser = new ();

        public Task UserConnected(string username, string connectedId)
        {
            lock(OnlineUser)
            {
                if(OnlineUser.ContainsKey(username))
                {
                    OnlineUser[username].Add(connectedId);
                }else{
                    OnlineUser.Add(username,new List<string>(){connectedId});
                }
            }
            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectedId)
        {
            lock(OnlineUser)
            {
                if(!OnlineUser.ContainsKey(username))
                {
                    return Task.CompletedTask;
                }

                OnlineUser[username].Remove(connectedId);

                if(OnlineUser.Count == 0)
                {
                    OnlineUser.Remove(username);
                }
            }
            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUser()
        {
            string[] onlineUser;
             lock(OnlineUser)
             {
                onlineUser = OnlineUser.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
             }

             return Task.FromResult(onlineUser);
        }
    }
}