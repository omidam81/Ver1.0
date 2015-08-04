using System.Linq;
using Orchard;
using Teeyoot.Module.Models;
using System;

namespace Teeyoot.Module.Services
{
    public interface IMessageService : IDependency
    {
        IQueryable<MessageRecord> GetAllMessages();

        IQueryable<MessageRecord> GetAllMessagesForUser(int userId);

        void DeleteMessage(int id);

        MessageRecord GetMessage(int id);

        void AddMessage(int userId, string text, string from, DateTime sendDate, int campaignId, string subject, bool isApprowed);

        DateTime GetLatestMessageDateForCampaign (int campaignId);

        IQueryable<MessageRecord> GetAllMessagesForCampaign(int campaignId);

    }

    
}
