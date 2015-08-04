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


        public void AddMessage(int userId, string text, string from, DateTime sendDate, int campaignId, string subject, bool isApprowed = false)
        {
            var message = new MessageRecord()
            {
                UserId = userId,
                Text = text,
                Sender = from,
                SendDate = sendDate,
                CampaignId = campaignId,
                Subject = subject,
                IsApprowed = isApprowed
            };
            _messageRepository.Create(message);
        }


        public DateTime GetLatestMessageDateForCampaign(int campaignId)
        {
            List<MessageRecord> messages = _messageRepository.Table.Where(m => m.CampaignId == campaignId).ToList();
            if (messages.Count > 0)
            {
                return messages.OrderByDescending(c => c.SendDate).FirstOrDefault().SendDate;
            }
            return DateTime.MaxValue;
        }


        public IQueryable<MessageRecord> GetAllMessagesForCampaign(int campaignId)
        {
            return _messageRepository.Table.Where(m => m.CampaignId == campaignId);
        }

    }
}