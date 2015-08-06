using RM.QuickLogOn.OAuth.Services;
using RM.QuickLogOn.Providers;

namespace Teeyoot.Account.Services
{
    public interface ITeeyootFacebookOAuthService : IFacebookOAuthService
    {
        QuickLogOnResponse WizardAuth(string tokenToInspect);
    }
}
