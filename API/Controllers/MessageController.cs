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
        //private readonly IUserRepository UserRepository;
        //private readonly IMessageRepository MessageRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorks _UnitOfWorks;
        public MessageController(IUnitOfWorks unitOfWorks, 
           IMapper mapper)
        {
            _UnitOfWorks = unitOfWorks;
            //_UnitOfWorks.MessageRepository! = messageRepository;
            //_userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
        {
              var username = User.GetUserName();

              if(username == createMessageDTO.RecipientUsername!.ToLower())
                return BadRequest("you can not send message to yourself");
              
              var sender = await _UnitOfWorks.UserRepository!.GetUserByUserNameAsync(username);
              var recipient = await _UnitOfWorks.UserRepository!.GetUserByUserNameAsync(createMessageDTO.RecipientUsername!);
  
              if(recipient == null) return NotFound();

              var message = new Message {
                 Sender = sender!,
                 Recipient = recipient,
                 SenderUsername = sender!.UserName,
                 RecipientUsername = recipient.UserName,
                 Content = createMessageDTO.content!
              };

              _UnitOfWorks.MessageRepository!.AddMessage(message);

              if(await _UnitOfWorks.Complete())
                return Ok(_mapper.Map<MessageDTO>(message));

             return BadRequest("failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageForUser([FromQuery] MessageParams messageParams)
        {
             messageParams.username = User.GetUserName();

             var messages = await _UnitOfWorks.MessageRepository!.GetMessageForUser(messageParams);

             Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize,
               messages.TotalCount, messages.TotalPage);
            
            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
        {
              var currentUsername = User.GetUserName();

            return  Ok(await _UnitOfWorks.MessageRepository!.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
          var username = User.GetUserName();

          var message = await _UnitOfWorks.MessageRepository!.GetMessage(id);

          if(message.Sender!.UserName != username && message.Recipient!.UserName != null)
            return Unauthorized();

          if (message.Sender.UserName == username) message.SenderDeleted = true;

          if(message.Recipient!.UserName == username) message.RecipientDeleted = true;

          if(message.SenderDeleted && message.RecipientDeleted )
            _UnitOfWorks.MessageRepository!.DeleteMessage(message);

          if(await _UnitOfWorks.Complete()) return Ok();

          return BadRequest("Failed to deleted message");
        }
    }
}