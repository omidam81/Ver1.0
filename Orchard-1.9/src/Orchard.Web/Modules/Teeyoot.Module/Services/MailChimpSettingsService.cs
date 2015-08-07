using Orchard.ContentManagement;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public class MailChimpSettingsService : IMailChimpSettingsService
    {
        private readonly IRepository<MailChimpSettingsPartRecord> _mailChimpSettingsRepository;
        private readonly IContentManager _contentManager;

        public MailChimpSettingsService(IRepository<MailChimpSettingsPartRecord> mailChimpSettingsRepository, IContentManager contentManager)
        {
            _mailChimpSettingsRepository = mailChimpSettingsRepository;
            _contentManager = contentManager;
        }

        public IContentQuery<MailChimpSettingsPart> GetAllSettings()
        {
            return _contentManager.Query<MailChimpSettingsPart, MailChimpSettingsPartRecord>(VersionOptions.Latest);
        }

        public void DeleteMailChimpSettingsPart(int id)
        {
            _contentManager.Remove(_contentManager.Get<MailChimpSettingsPart>(id).ContentItem);
        }

        public MailChimpSettingsPart CreateMailChimpSettingsPart(string apiKey, string culture)
        {
            var MailChimpSettingsPart = _contentManager.Create<MailChimpSettingsPart>("MailChimpSettings",
                se =>
                {
                    se.ApiKey = apiKey;
                    se.Culture = culture;
                                  
                });

            return MailChimpSettingsPart;
        }

        public MailChimpSettingsPart GetSetting(int id)
        {
            return _contentManager.Get<MailChimpSettingsPart>(id, VersionOptions.Latest);
        }

        public IContentQuery<MailChimpSettingsPart> GetSettingByCulture(string culture)
        {
            return _contentManager.Query<MailChimpSettingsPart, MailChimpSettingsPartRecord>(VersionOptions.Latest).Where(fe => fe.Culture == culture);
        }

    }
}