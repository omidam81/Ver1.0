using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public class MessageService : IMessageService
    {
        private readonly IRepository<MessageRecord> _messageRepository;

        public MessageService(IRepository<MessageRecord> messageRepository)
        {
            _messageRepository = messageRepository;
        }
        
        public IQueryable<MessageRecord> GetAllMessages()
        {
            return _messageRepository.Table;
        }

        public IQueryable<MessageRecord> GetAllMessagesForUser(int userId)
        {
            return _messageRepository.Table.Where(m => m.UserId == userId);
        }

        public void DeleteMessage(int id)
        {
            _messageRepository.Delete(_messageRepository.Get(id));
        }

        public MessageRecord GetMessage(int id)
        {
           return _messageRepository.Table.Where(m => m.Id == id).FirstOrDefault();
        }


        public void AddMessage(int userId, string text, string from, DateTime sendDate)
        {
            var message = new MessageRecord()
            {
                UserId = userId,
                Text = text,
                Sender = from,
                SendDate = sendDate
            };
            _messageRepository.Create(message);
        }
    }
}