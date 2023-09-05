using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class presenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUser = new ();

        public Task<bool> UserConnected(string username, string connectedId)
        {
            bool isOnline = false;
            lock(OnlineUser)
            {
                if(OnlineUser.ContainsKey(username))
                {
                    OnlineUser[username].Add(connectedId);
                }else{
                    OnlineUser.Add(username,new List<string>(){connectedId});
                    isOnline = true;
                }
            }
            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectedId)
        {
            bool IsOffline = false;
            lock(OnlineUser)
            {
                if(!OnlineUser.ContainsKey(username))
                {
                    return Task.FromResult(IsOffline);
                }

                OnlineUser[username].Remove(connectedId);

                if(OnlineUser.Count == 0)
                {
                    OnlineUser.Remove(username);
                    IsOffline = true;
                }
            }
            return Task.FromResult(IsOffline);
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

        public Task<List<string>> GetconnexionFromUser(string username)
        {
             List<string> connexionIds;
             lock(OnlineUser)
             {
                connexionIds = OnlineUser.GetValueOrDefault(username);
             }

             return Task.FromResult(connexionIds);
        }
    }
}