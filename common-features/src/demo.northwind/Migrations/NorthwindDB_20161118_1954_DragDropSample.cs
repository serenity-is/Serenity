using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20161118_1954)]
public class NorthwindDB_20161118_1954_DragDropSample : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("DragDropSample")
            .WithColumn("Id").AsInt32().IdentityKey(this)
            .WithColumn("ParentId").AsInt32().Nullable()
            .WithColumn("Title").AsString(100).NotNullable();
    }
}