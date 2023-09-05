using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interface;
using API.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using API.DTOs;
using API.Entities;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<PresenceHub> _PresenceHub;
        private readonly presenceTracker _presenceTracker;
        public MessageHub(IMessageRepository messageRepository, IMapper mapper,IUserRepository userRepository,    
           IHubContext<PresenceHub> presenceHub, presenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
            _PresenceHub = presenceHub;
            _userRepository = userRepository;
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext!.Request.Query["user"].ToString();
            //Console.WriteLine("other usrer" + otherUser);
            var groupName = GetGroupName(Context.User!.GetUserName(), otherUser);
            //Console.WriteLine("other usrer" + groupName);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group =  await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = _messageRepository.GetMessageThread(
              Context.User!.GetUserName(), otherUser   
            );

            //await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
             await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name!).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDTO createMessageDTO)
        {
            var username = Context.User!.GetUserName();

              if(username == createMessageDTO.RecipientUsername!.ToLower())
                throw new HubException("you can not send message to yourself");
              
              var sender = await _userRepository.GetUserByUserNameAsync(username);
              var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDTO.RecipientUsername!);
  
              if(recipient == null) throw new HubException("user not found");

              var message = new Message {
                 Sender = sender!,
                 Recipient = recipient,
                 SenderUsername = sender!.UserName,
                 RecipientUsername = recipient.UserName,
                 Content = createMessageDTO.content!
              };

              var groupName = GetGroupName(sender.UserName, recipient.UserName);

              var groupe = await _messageRepository.GetMessageGroup(groupName);

              if(groupe.Connections.Any(x => x.Username ==  recipient.UserName))
              {
                 message.DateRead = DateTime.UtcNow;
              }else{
                var connections = await _presenceTracker.GetconnexionFromUser(recipient.UserName);
                if(connections != null){
                    await _PresenceHub.Clients.Clients(connections).SendAsync("newMessageReceived",
                    new {username = sender.UserName, KnowAs =  sender.KnowAs});
                }
              }
             

              _messageRepository.AddMessage(message);

              if(await _messageRepository.saveAsync())
              {
               
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
              }

        }

        private async Task<Group> AddToGroup(string groupName)
        {
          var group = await _messageRepository.GetMessageGroup(groupName);
          var connection = new Connection(Context.ConnectionId, Context.User!.GetUserName());

          if(group == null)
          {
            group = new Group(groupName);
            _messageRepository.AddGroup(group);
          }

          group.Connections.Add(connection);

          if (await _messageRepository.saveAsync())
          {
            return group;
          }

          throw new HubException("Failed to join Group");
        }


        private async Task<Group> RemoveFromMessageGroup()
        {
          var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
          var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
          _messageRepository.RemoveConnexion(connection!);
          if(await _messageRepository.saveAsync())
          {
            return group;
          }

          throw new HubException("failed to remove from Group");
        }


        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}