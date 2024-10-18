using FluentMigrator;
using System.IO;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20141123_1551)]
public class NorthwindDB_20141123_1551_Initial : Migration
{
    private string GetScript(string name)
    {
        using var sr = new StreamReader(GetType().Assembly.GetManifestResourceStream(name));
        return sr.ReadToEnd();
    }

    public override void Up()
    {
        IfDatabase("SqlServer", "SqlServer2000", "SqlServerCe")
            .Execute.Sql(GetScript("Serenity.Demo.Northwind.Migrations.NorthwindDBScript_SqlServer.sql"));

        IfDatabase("Postgres")
            .Execute.Sql(GetScript("Serenity.Demo.Northwind.Migrations.NorthwindDBScript_Postgres.sql"));

        IfDatabase("Postgres")
            .Execute.Sql(GetScript("Serenity.Demo.Northwind.Migrations.NorthwindDBScript_PostgresData.sql"));

        IfDatabase("MySql")
            .Execute.Sql(GetScript("Serenity.Demo.Northwind.Migrations.NorthwindDBScript_MySql.sql"));

        IfDatabase("Oracle")
            .Execute.Sql(GetScript("Serenity.Demo.Northwind.Migrations.NorthwindDBScript_Oracle.sql"));
        IfDatabase("Sqlite")
            .Execute.Sql(GetScript("Serenity.Demo.Northwind.Migrations.NorthwindDBScript_Sqlite.sql"));

        IfDatabase("SqlServer", "SqlServer2000", "SqlServerCe", "Postgres")
            .Alter.Table("Customers")
                .AddColumn("ID").AsInt32().Identity().NotNullable();

        IfDatabase("Oracle")
            .Alter.Table("Customers")
                .AddColumn("ID").AsInt32().Nullable();

        MigrationUtils.AddOracleIdentity(this, "Customers", "ID");
        IfDatabase("Oracle")
            .Execute.Sql("UPDATE Customers SET ID = CUSTOMERS_SEQ.nextval");

        IfDatabase("Oracle")
            .Alter.Column("ID").OnTable("Customers")
                .AsInt32().NotNullable();

        IfDatabase("SqlServer", "SqlServer2000", "SqlServerCe", "Postgres")
            .Alter.Table("Territories")
                .AddColumn("ID").AsInt32().Identity();

        IfDatabase("Oracle")
            .Alter.Table("Territories")
                .AddColumn("ID").AsInt32().Nullable();

        MigrationUtils.AddOracleIdentity(this, "Territories", "ID");

        IfDatabase("Oracle")
            .Execute.Sql("UPDATE Territories SET ID = Territories_SEQ.nextval");

        IfDatabase("Oracle")
            .Alter.Column("ID").OnTable("Territories")
                .AsInt32().NotNullable();

        Alter.Table("Products")
            .AddColumn("ProductImage").AsString(100).Nullable();
    }

    public override void Down()
    {
    }
}