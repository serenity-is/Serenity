using FluentMigrator;

namespace Serene.Migrations.DefaultDB;

[DefaultDB, MigrationKey(20160515_0726)]
public class DefaultDB_20160515_0726_UserPreferences : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("UserPreferences")
            .WithColumn("UserPreferenceId").AsInt32().IdentityKey(this)
            .WithColumn("UserId").AsInt64().NotNullable()
            .WithColumn("PreferenceType").AsString(100).NotNullable()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Value").AsString(int.MaxValue).Nullable();

        Create.Index("IX_UserPref_UID_PrefType_Name")
            .OnTable("UserPreferences")
            .OnColumn("UserId").Ascending()
            .OnColumn("PreferenceType").Ascending()
            .OnColumn("Name").Ascending()
            .WithOptions().Unique();
    }
}