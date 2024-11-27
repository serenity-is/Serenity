using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20160121_1412)]
public class NorthwindDB_20160121_1412_CustomerRepresentatives : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("CustomerRepresentatives")
            .WithColumn("RepresentativeID").AsInt32().IdentityKey(this)
            .WithColumn("CustomerID").AsInt32().NotNullable()
            .WithColumn("EmployeeID").AsInt32().NotNullable();
    }
}