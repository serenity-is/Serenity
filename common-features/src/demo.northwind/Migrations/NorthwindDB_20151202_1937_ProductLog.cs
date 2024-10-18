using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20151202_1937)]
public class NorthwindDB_20151202_1937_ProductLog : AutoReversingMigration
{
    public override void Up()
    {

        Create.Table("ProductLog")
            .WithColumn("ProductLogID").AsInt64().IdentityKey(this)
            .WithColumn("OperationType").AsInt16().NotNullable()
            .WithColumn("ChangingUserId").AsInt32().Nullable()
            .WithColumn("ValidFrom").AsDateTime().NotNullable()
            .WithColumn("ValidUntil").AsDateTime().NotNullable()
            .WithColumn("ProductID").AsInt32().NotNullable()
            .WithColumn("ProductName").AsString(40).Nullable()
            .WithColumn("ProductImage").AsString(100).Nullable()
            .WithColumn("Discontinued").AsBoolean().Nullable()
            .WithColumn("SupplierID").AsInt32().Nullable()
            .WithColumn("CategoryID").AsInt32().Nullable()
            .WithColumn("QuantityPerUnit").AsString(20).Nullable()
            .WithColumn("UnitPrice").AsCurrency().Nullable()
            .WithColumn("UnitsInStock").AsInt16().Nullable()
            .WithColumn("UnitsOnOrder").AsInt16().Nullable()
            .WithColumn("ReorderLevel").AsInt16().Nullable();
    }
}