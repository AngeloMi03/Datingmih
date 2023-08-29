using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessageController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
         private readonly IMapper _mapper;
        public MessageController(IUserRepository userRepository, IMessageRepository messageRepository, 
           IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
        {
              var username = User.GetUserName();

              if(username == createMessageDTO.RecipientUsername!.ToLower())
                return BadRequest("you can not send message to yourself");
              
              var sender = await _userRepository.GetUserByUserNameAsync(username);
              var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDTO.RecipientUsername!);
  
              if(recipient == null) return NotFound();

              var message = new Message {
                 Sender = sender!,
                 Recipient = recipient,
                 SenderUsername = sender!.UserName,
                 RecipientUsername = recipient.UserName,
                 Content = createMessageDTO.content!
              };

              _messageRepository.AddMessage(message);

              if(await _messageRepository.saveAsync())
                return Ok(_mapper.Map<MessageDTO>(message));

             return BadRequest("failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageForUser([FromQuery] MessageParams messageParams)
        {
             messageParams.username = User.GetUserName();

             var messages = await _messageRepository.GetMessageForUser(messageParams);

             Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize,
               messages.TotalCount, messages.TotalPage);
            
            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
        {
              var currentUsername = User.GetUserName();

            return  Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
          var username = User.GetUserName();

          var message = await _messageRepository.GetMessage(id);

          if(message.Sender!.UserName != username && message.Recipient!.UserName != null)
            return Unauthorized();

          if (message.Sender.UserName == username) message.SenderDeleted = true;

          if(message.Recipient!.UserName == username) message.RecipientDeleted = true;

          if(message.SenderDeleted && message.RecipientDeleted )
            _messageRepository.DeleteMessage(message);

          if(await _messageRepository.saveAsync()) return Ok();

          return BadRequest("Failed to deleted message");
        }
    }
}