using Orchard.Security;

namespace Teeyoot.Account.Common
{
    public class ValidateLogOnResult
    {
        public IUser User { get; set; }
        public bool IsValid { get; set; }
        public string ValidationSummary { get; set; }
    }
}