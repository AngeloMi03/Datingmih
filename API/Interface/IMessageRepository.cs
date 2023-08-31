using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interface
{
    public interface IMessageRepository
    {

      void AddGroup(Group group);
      void RemoveConnexion(Connection connection);
      Task<Connection> GetConnection(string connexionId);
      Task<Group> GetMesssageGroup(string groupName);
      void AddMessage(Message message);
      void DeleteMessage(Message message);
      Task<Message> GetMessage(int id);
      Task<PageList<MessageDTO>> GetMessageForUser(MessageParams messageParams);
      Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string RecipientUsername);
      Task<bool> saveAsync();
    }
}