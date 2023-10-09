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

        public void AddGroup(Group group)
        {
            _datacontext.Groups!.Add(group);
        }

        public void AddMessage(Message message)
        {
            _datacontext.Messages!.Add(message);
        }

        public void DeleteMessage(Message message)
        {
           _datacontext.Messages!.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            var connexion = await _datacontext.Connections!.FindAsync(connectionId);
            return connexion!;
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
           var group =  await _datacontext.Groups!
                          .Include(x => x.Connections)
                          .Where(x => x.Connections.Any(x => x.ConnectionId == connectionId))
                          .FirstOrDefaultAsync();
           return group!;
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
               .ProjectTo<MessageDTO>(_Mapper.ConfigurationProvider)
               .AsQueryable();

            query = messageParams.Container switch 
            {
              "inbox" => query.Where(m => m.RecipientUsername == messageParams.username 
                 && m.RecipientDeleted == false),
              "outbox" => query.Where(m => m.SenderUsername == messageParams.username
                 && m.SenderDeleted == false),
               _ => query.Where(m => m.RecipientUsername == messageParams.username 
                 && m.RecipientDeleted == false && m.DateRead == null)
            };

            //var messages = query.ProjectTo<MessageDTO>(_Mapper.ConfigurationProvider);

            return await PageList<MessageDTO>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            var messageGroup = await _datacontext.Groups!
              .Include(x => x.Connections)
              .FirstOrDefaultAsync(x => x.Name == groupName);
            return messageGroup!;
        }

        public  async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string RecipientUsername)
        {
            var messages = await _datacontext.Messages!
             //.Include(u => u.Sender).ThenInclude(u => u!.Photos)
             //.Include(u => u.Recipient).ThenInclude(u => u!.Photos)
             .Where(u => u.Recipient!.UserName == currentUsername   && u.RecipientDeleted == false
                     && u.Sender!.UserName == RecipientUsername 
                || u.Recipient.UserName == RecipientUsername && u.Sender!.UserName == currentUsername
                    && u.SenderDeleted == false)
             .OrderBy(m => m.MessageSent)
             .ProjectTo<MessageDTO>(_Mapper.ConfigurationProvider)//ajout
             .ToListAsync();

             var unreadMessages = messages.Where(m => m.DateRead is null && m.RecipientUsername! == currentUsername);
        
           if(unreadMessages.Any()){
              foreach(var message in unreadMessages)
              {
                message.DateRead = DateTime.UtcNow;
              }

              await _datacontext.SaveChangesAsync();
           }

           //return _Mapper.Map<IEnumerable<MessageDTO>>(messages);
           return messages;
        }

        public void RemoveConnexion(Connection connection)
        {
            _datacontext.Connections!.Remove(connection);
        }

        /*public async Task<bool> saveAsync()
        {
            return await _datacontext.SaveChangesAsync() > 0;
        }*/
    }
}