using Orchard;
using Orchard.Security;

namespace Teeyoot.Account.Services
{
    public interface ITeeyootUserService : IDependency
    {
        void SendWelcomeEmail(IUser user);
    }
}
