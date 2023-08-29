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
        public MessageHub(IMessageRepository messageRepository, IMapper mapper,IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext!.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User!.GetUserName(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = _messageRepository.GetMessageThread(
              Context.User!.GetUserName(), otherUser   
            );

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
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

              _messageRepository.AddMessage(message);

              if(await _messageRepository.saveAsync())
              {
                var group = GetGroupName(sender.UserName, recipient.UserName);
                await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
              }

        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}