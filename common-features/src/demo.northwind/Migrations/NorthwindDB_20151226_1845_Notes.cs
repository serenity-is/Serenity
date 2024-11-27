using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20151226_1845)]
public class NorthwindDB_20151226_1845_Notes : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("Notes")
            .WithColumn("NoteID").AsInt64().IdentityKey(this)
            .WithColumn("EntityType").AsString(100).NotNullable()
            .WithColumn("EntityID").AsInt64().NotNullable()
            .WithColumn("Text").AsString(int.MaxValue).NotNullable()
            .WithColumn("InsertUserId").AsInt32().NotNullable()
            .WithColumn("InsertDate").AsDateTime().NotNullable();
    }
}