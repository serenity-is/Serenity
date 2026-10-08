using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20261008_1200)]
public class NorthwindDB_20261008_1200_ProductConcurrencyVersion : Migration
{
    public override void Up()
    {
        // Adds an optimistic concurrency version column to Products, maintained by a trigger.
        // Rows implementing IConcurrencyVersionRow (see ProductRow) then use it in the update
        // WHERE clause. On SQL Server only, a native rowversion column could be used instead.
        Alter.Table("Products")
            .AddColumn("RowVersion").AsInt32().NotNullable().WithDefaultValue(0)
            .AddConcurrencyVersionTrigger(this, idField: "ProductID");
    }

    public override void Down()
    {
        this.DropConcurrencyVersionTrigger("Products", "RowVersion");
        Delete.Column("RowVersion").FromTable("Products");
    }
}
