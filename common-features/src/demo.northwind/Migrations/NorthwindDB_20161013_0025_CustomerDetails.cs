using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20161013_0025)]
public class NorthwindDB_20161013_0025_CustomerDetails : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("CustomerDetails")
            .WithColumn("ID").AsInt32().PrimaryKey().NotNullable()
            .WithColumn("LastContactDate").AsDateTime().Nullable()
            .WithColumn("LastContactedBy").AsInt32().Nullable()
                .ForeignKey("FK_CustomerDetails_LastContactedBy", "Employees", "EmployeeID")
            .WithColumn("Email").AsString(100).Nullable()
            .WithColumn("SendBulletin").AsBoolean().NotNullable().WithDefaultValue(true);
    }
}