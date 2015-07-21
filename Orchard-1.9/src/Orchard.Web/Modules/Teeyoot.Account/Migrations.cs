using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Roles.Services;

namespace Teeyoot.Account
{
    public class Migrations : DataMigrationImpl
    {
        private readonly IRoleService _roleService;

        public Migrations(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public int Create()
        {
            _roleService.CreateRole("Seller");

            ContentDefinitionManager.AlterTypeDefinition("TeeyootUser", builder =>
                builder.WithPart("UserRolesPart"));

            return 2;
        }

        public int UpdateFrom1()
        {
            ContentDefinitionManager.AlterTypeDefinition("TeeyootUser", builder =>
                builder.WithPart("UserRolesPart"));

            return 2;
        }
    }
}