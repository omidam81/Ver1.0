using RM.QuickLogOn.OAuth.Services;
using RM.QuickLogOn.Providers;

namespace Teeyoot.Account.Services
{
    public interface ITeeyootGoogleOAuthService : IGoogleOAuthService
    {
        QuickLogOnResponse WizardAuth(string tokenToVerify);
    }
}
