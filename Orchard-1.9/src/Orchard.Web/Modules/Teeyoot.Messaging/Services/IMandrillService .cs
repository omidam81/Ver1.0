using Mandrill;
using Mandrill.Model;
using Orchard;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Messaging.Services
{
    public interface IMandrillService : IDependency
    {
        void SendSellerMessage(int messageId, string pathToTemplates, string pathToMedia);

        void SendWelcomeMessage(string userEmail, string pathToTemplates);

        void SendOrderMessage(int campaignId, string pathToTemplates, string pathToMedia);

        void FillMessageMergeVars(MandrillMessage message, LinkOrderCampaignProductRecord record);

        string SendTmplMessage(MandrillApi mAPI, MandrillMessage message);
    }
}