using Orchard;
using RM.QuickLogOn.Providers;

namespace Teeyoot.Account.Services
{
    public interface ITeeyootSocialLogOnService : IDependency
    {
        QuickLogOnResponse LogOn(QuickLogOnRequest request);
    }
}
