using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20160908_1801)]
public class NorthwindDB_20160908_1801_CustomerGrossSales : AutoReversingMigration
{
    public override void Up()
    {
        IfDatabase("SqlServer", "SqlServer2000", "SqlServerCe")
            .Execute.Sql(@"
CREATE PROCEDURE [dbo].[CustomerGrossSales]
    @startDate DATE,
    @endDate DATE
AS
BEGIN
    SELECT
        o.CustomerID, 
        c.ContactName, 
        d.ProductID, 
        p.ProductName,
        GrossAmount = Sum(d.UnitPrice* d.Quantity)
    FROM
        Orders o
        LEFT OUTER JOIN
            Customers c on c.CustomerID = o.CustomerID
        LEFT OUTER JOIN
            [Order Details] d on d.OrderID = o.OrderID
        LEFT OUTER JOIN
            Products p on p.ProductID = d.ProductID
    WHERE
        (@startDate IS NULL OR o.OrderDate >= @startDate) AND
        (@endDate IS NULL OR o.OrderDate <= @endDate)
    GROUP BY c.ContactName, p.ProductName, o.CustomerID, d.ProductID
    ORDER BY c.ContactName, Sum(d.UnitPrice* d.Quantity) DESC
END");

    }
}