using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20241120_1203)]
public class NorthwindDB_20241120_1203_LanguageIdToCode : AutoReversingMigration
{
    public override void Up()
    {
        if (!this.IsSqlite())
        {
            Alter.Table("CategoryLang")
                .AlterColumn("LanguageID").AsString(10).NotNullable();

            Alter.Table("ProductLang")
                .AlterColumn("LanguageID").AsString(10).NotNullable();
        }
    }
}