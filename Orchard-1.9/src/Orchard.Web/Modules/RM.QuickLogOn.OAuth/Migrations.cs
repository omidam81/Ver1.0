using System.Data;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth
{
    [OrchardFeature("RM.QuickLogOn.OAuth")]
    public class GoogleMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "GoogleSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(255))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Facebook")]
    public class FacebookMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "FacebookSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(255))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.LiveID")]
    public class LiveIDMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "LiveIDSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(255))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Twitter")]
    public class TwitterMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "TwitterSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ConsumerKey", DbType.String, command => command.WithLength(512))
                              .Column("AccessToken", DbType.String, command => command.WithLength(512))
                              .Column("EncryptedConsumerSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.LinkedIn")]
    public class LinkedInMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "LinkedInSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(255))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }
}
