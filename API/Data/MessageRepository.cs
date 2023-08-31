using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly Datacontext _datacontext;
        private readonly  IMapper _Mapper ;
        public MessageRepository(Datacontext datacontext, IMapper mapper)
        {
            _Mapper = mapper;
            _datacontext = datacontext;
        }

        public void AddMessage(Message message)
        {
            _datacontext.Messages!.Add(message);
        }

        public void DeleteMessage(Message message)
        {
           _datacontext.Messages!.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            var messages = await _datacontext.Messages!
                 .Include(x => x.Sender)
                 .Include(x => x.Recipient)
                 .SingleOrDefaultAsync( x => x.id == id);
            return messages!;
        }

        public async Task<PageList<MessageDTO>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _datacontext.Messages!
               .OrderByDescending(m => m.MessageSent)
               .AsQueryable();

            query = messageParams.Container switch 
            {
              "inbox" => query.Where(m => m.Recipient!.UserName == messageParams.username 
                 && m.RecipientDeleted == false),
              "outbox" => query.Where(m => m.Sender!.UserName == messageParams.username
                 && m.SenderDeleted == false),
               _ => query.Where(m => m.Recipient!.UserName == messageParams.username 
                 && m.RecipientDeleted == false && m.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDTO>(_Mapper.ConfigurationProvider);

            return await PageList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public  async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string RecipientUsername)
        {
            var messages = await _datacontext.Messages!
             .Include(u => u.Sender).ThenInclude(u => u!.Photos)
             .Include(u => u.Recipient).ThenInclude(u => u!.Photos)
             .Where(u => u.Recipient!.UserName == currentUsername   && u.RecipientDeleted == false
                     && u.Sender!.UserName == RecipientUsername 
                || u.Recipient.UserName == RecipientUsername && u.Sender!.UserName == currentUsername
                    && u.SenderDeleted == false)
             .OrderBy(m => m.MessageSent)
             .ToListAsync();

             var unreadMessages = messages.Where(m => m.DateRead is null && m.Recipient!.UserName == currentUsername);
        
           if(unreadMessages.Any()){
              foreach(var message in unreadMessages)
              {
                message.DateRead = DateTime.Now;
              }

              await _datacontext.SaveChangesAsync();
           }

           return _Mapper.Map<IEnumerable<MessageDTO>>(messages);
        }
        public async Task<bool> saveAsync()
        {
            return await _datacontext.SaveChangesAsync() > 0;
        }
    }
}