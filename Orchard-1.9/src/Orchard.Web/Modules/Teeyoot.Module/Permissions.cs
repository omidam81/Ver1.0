using System.Collections.Generic;
using Orchard.Security.Permissions;
using Orchard.Environment.Extensions.Models;

namespace Teeyoot.Module
{
    public class Permissions : IPermissionProvider {
        public static readonly Permission ApproveCampaigns = new Permission { Description = "Approve Campaigns", Name = "ApproveCampaigns" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                ApproveCampaigns,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ApproveCampaigns}
                }
            };
        }
    }
}
