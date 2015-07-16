using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public interface IMailChimpSettingsService : IDependency
    {
        IEnumerable<MailChimpSettingsPartRecord> GetAllSettings();

        MailChimpSettingsPart CreateMailChimpSettingsPart(string apiKey, string mailChimpCampaignId, int templateId, string templateName, string mailChimpListId, string culture);

        void DeleteMailChimpSettingsPart(int id);
    }
}