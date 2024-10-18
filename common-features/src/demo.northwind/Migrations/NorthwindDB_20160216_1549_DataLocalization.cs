using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20160216_1549)]
public class NorthwindDB_20160216_1549_DataLocalization : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("CategoryLang")
            .WithColumn("ID").AsInt32().IdentityKey(this)
            .WithColumn("CategoryID").AsInt32().NotNullable()
            .WithColumn("LanguageID").AsInt32().NotNullable()
            .WithColumn("CategoryName").AsString(15).Nullable()
            .WithColumn("Description").AsString(int.MaxValue).Nullable();

        Create.Table("ProductLang")
            .WithColumn("ID").AsInt32().IdentityKey(this)
            .WithColumn("ProductID").AsInt32().NotNullable()
            .WithColumn("LanguageID").AsInt32().NotNullable()
            .WithColumn("ProductName").AsString(40).Nullable();
    }
}