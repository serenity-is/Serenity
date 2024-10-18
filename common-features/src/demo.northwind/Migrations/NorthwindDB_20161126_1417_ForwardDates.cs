using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20161126_1417)]
public class NorthwindDB_20161126_1417_ForwardDates : Migration
{
    public override void Up()
    {
        var o = OrderRow.Fields;

        var dateAdd = "dateadd(day, datediff(day, (select max(orderdate) from Orders), getdate()), ";

        IfDatabase("SqlServer", "SqlServer2000", "SqlServerCe")
            .Execute.Sql(
                new SqlUpdate(o.TableName)
                    .SetTo(o.OrderDate, dateAdd + o.OrderDate.Name + ")")
                    .SetTo(o.RequiredDate, dateAdd + o.RequiredDate.Name + ")")
                    .SetTo(o.ShippedDate, dateAdd + o.ShippedDate.Name + ")")
                    .Where(o.OrderDate <= new DateTime(1999, 7, 1))
                    .DebugText);
    }

    public override void Down()
    {
    }
}