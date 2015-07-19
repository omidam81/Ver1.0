using Orchard;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Messaging.Models;

namespace Teeyoot.Messaging.Services
{
    public interface IMailChimpSettingsService : IDependency
    {
        IContentQuery<MailChimpSettingsPart> GetAllSettings();

        MailChimpSettingsPart GetSetting(int id);

        IContentQuery<MailChimpSettingsPart> GetSettingByCulture(string culture);

        MailChimpSettingsPart CreateMailChimpSettingsPart(string apiKey, string mailChimpListId, string welcomeCampaignId, int welcomeTemplateId, string allBuyersCampaignId, int allBuyersTemplateId, string culture);

        void DeleteMailChimpSettingsPart(int id);
    }
}