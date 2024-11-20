using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20241120_1203)]
public class NorthwindDB_20241120_1203_LanguageIdToCode : AutoReversingMigration
{
    public override void Up()
    {
        Alter.Table("CategoryLang")
            .AlterColumn("LanguageId").AsString(10).NotNullable();

        Alter.Table("ProductLang")
            .AlterColumn("LanguageId").AsString(10).NotNullable();
    }
}