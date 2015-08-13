using Orchard;
using Orchard.Security;

namespace Teeyoot.Account.Services
{
    public interface ITeeyootMembershipService : IDependency
    {
        IUser CreateUser(string email, string password, string name, string phone);
    }
}
