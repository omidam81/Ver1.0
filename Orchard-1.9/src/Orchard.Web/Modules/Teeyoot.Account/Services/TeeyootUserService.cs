using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mandrill;
using Mandrill.Model;
using Orchard.Security;
using Teeyoot.Module.Services;

namespace Teeyoot.Account.Services
{
    public class TeeyootUserService : ITeeyootUserService
    {
        private readonly IMailChimpSettingsService _settingsService;

        public TeeyootUserService(
            IMailChimpSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void SendWelcomeEmail(IUser user)
        {
            var pathToTemplates =
                HttpContext.Current.Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage
            {
                MergeLanguage = MandrillMessageMergeLanguage.Handlebars,
                FromEmail = "noreply@teeyoot.com",
                FromName = "Teeyoot",
                Subject = "Teeyoot welcomes you onboard!"
            };
            var emails = new List<MandrillMailAddress> {new MandrillMailAddress(user.Email, "user")};
            mandrillMessage.To = emails;
            var request = HttpContext.Current.Request;
            mandrillMessage.AddRcptMergeVars(user.Email, "Url", request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/");
            var baseUrl = "";
            baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";
            mandrillMessage.AddRcptMergeVars(user.Email, "VideoPreviewUrl", baseUrl + "/Media/Default/images/video_thumbnail_521x315.jpg/");
            var text = System.IO.File.ReadAllText(pathToTemplates + "welcome-template.html");
            mandrillMessage.Html = text;
            var res = SendTmplMessage(api, mandrillMessage);
        }

        private static string SendTmplMessage(MandrillApi mAPI, MandrillMessage message)
        {
            var result = mAPI.Messages.Send(message);
            return result.ToString();
        }
    }
}