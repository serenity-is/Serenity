using FluentMigrator;

namespace Serenity.Demo.Northwind.Migrations;

[NorthwindDB, MigrationKey(20141123_1551)]
public class NorthwindDB_20141123_1551_Initial : Migration
{
    public override void Up()
    {
        Create.Table("Categories")
            .WithColumn("CategoryID").AsInt32().IdentityKey(this)
            .WithColumn("CategoryName").AsString(15).NotNullable()
            .WithColumn("Description").AsString(int.MaxValue).Nullable()
            .WithColumn("PicturePath").AsString(200).Nullable();

        Create.Table("CategoryLang")
            .WithColumn("ID").AsInt32().IdentityKey(this)
            .WithColumn("CategoryID").AsInt32().NotNullable()
            .WithColumn("LanguageID").AsString(10).NotNullable()
            .WithColumn("CategoryName").AsString(15).Nullable()
            .WithColumn("Description").AsString(int.MaxValue).Nullable();

        Create.Table("Region")
            .WithColumn("RegionID").AsInt32().IdentityKey(this)
            .WithColumn("RegionDescription").AsString(50).NotNullable();

        Create.Table("Territories")
            .WithColumn("TerritoryID").AsString(20).NotNullable().PrimaryKey()
            .WithColumn("TerritoryDescription").AsString(50).NotNullable()
            .WithColumn("RegionID").AsInt32().NotNullable()
                .ForeignKey("FK_Territories_RegionID", "Region", "RegionID");

        Create.Table("Customers")
            .WithColumn("CustomerID").AsString(5).NotNullable().PrimaryKey()
            .WithColumn("CompanyName").AsString(40).NotNullable().Indexed("IX_Customers_CompanyName")
            .WithColumn("ContactName").AsString(30).Nullable()
            .WithColumn("ContactTitle").AsString(30).Nullable()
            .WithColumn("Address").AsString(60).Nullable()
            .WithColumn("City").AsString(30).Nullable()
            .WithColumn("Region").AsString(30).Nullable()
            .WithColumn("PostalCode").AsString(10).Nullable()
            .WithColumn("Country").AsString(30).Nullable()
            .WithColumn("Phone").AsString(24).Nullable()
            .WithColumn("Fax").AsString(24).Nullable();

        Create.Table("CustomerDemographics")
            .WithColumn("ID").AsInt32().AutoIncrement(this)
            .WithColumn("CustomerTypeID").AsString(10).NotNullable().PrimaryKey()
            .WithColumn("CustomerDesc").AsString(int.MaxValue).Nullable();

        Create.Table("CustomerCustomerDemo")
            .WithColumn("ID").AsInt32().AutoIncrement(this)
            .WithColumn("CustomerID").AsString(5).PrimaryKey().NotNullable()
                .ForeignKey("FK_CustCustDemo_CustomerID", "Customers", "CustomerID")
            .WithColumn("CustomerTypeID").AsString(10).PrimaryKey().NotNullable()
                .ForeignKey("FK_CustCustDemo_CustomerTypeID", "CustomerDemographics", "CustomerTypeID");

        Create.Table("Employees")
            .WithColumn("EmployeeID").AsInt32().IdentityKey(this)
            .WithColumn("LastName").AsString(20).NotNullable()
            .WithColumn("FirstName").AsString(10).NotNullable()
            .WithColumn("Title").AsString(30).Nullable()
            .WithColumn("TitleOfCourtesy").AsString(25).Nullable()
            .WithColumn("BirthDate").AsDateTime().Nullable()
            .WithColumn("HireDate").AsDateTime().Nullable()
            .WithColumn("PhotoPath").AsString(200).Nullable()
            .WithColumn("ReportsTo").AsInt32().Nullable()
                .ForeignKey("FK_Employees_ReportsTo", "Employees", "EmployeeID")
            .WithColumn("Address").AsString(60).Nullable()
            .WithColumn("City").AsString(30).Nullable()
            .WithColumn("Region").AsString(30).Nullable()
            .WithColumn("PostalCode").AsString(10).Nullable()
            .WithColumn("Country").AsString(30).Nullable()
            .WithColumn("HomePhone").AsString(24).Nullable()
            .WithColumn("Extension").AsString(4).Nullable()
            .WithColumn("Notes").AsString(int.MaxValue).Nullable();

        Create.Table("Shippers")
            .WithColumn("ShipperID").AsInt32().IdentityKey(this)
            .WithColumn("CompanyName").AsString(40).NotNullable()
            .WithColumn("Phone").AsString(24).Nullable();

        Create.Table("Suppliers")
            .WithColumn("SupplierID").AsInt32().IdentityKey(this)
            .WithColumn("CompanyName").AsString(40).NotNullable()
            .WithColumn("ContactName").AsString(30).Nullable()
            .WithColumn("ContactTitle").AsString(30).Nullable()
            .WithColumn("Address").AsString(60).Nullable()
            .WithColumn("City").AsString(30).Nullable()
            .WithColumn("Region").AsString(30).Nullable()
            .WithColumn("PostalCode").AsString(10).Nullable()
            .WithColumn("Country").AsString(30).Nullable()
            .WithColumn("Phone").AsString(24).Nullable()
            .WithColumn("Fax").AsString(24).Nullable()
            .WithColumn("HomePage").AsString(200).Nullable();

        Create.Table("CustomerDetails")
            .WithColumn("CustomerID").AsString(5).PrimaryKey().NotNullable()
                .ForeignKey("FK_CustomerDetails_CustomerID", "Customers", "CustomerID")
            .WithColumn("LastContactDate").AsDateTime().Nullable()
            .WithColumn("LastContactedBy").AsInt32().Nullable()
                .ForeignKey("FK_CustomerDetails_LastContactedBy", "Employees", "EmployeeID")
            .WithColumn("Email").AsString(100).Nullable()
            .WithColumn("SendBulletin").AsBoolean().NotNullable().WithDefaultValue(true);

        Create.Table("CustomerRepresentatives")
            .WithColumn("RepresentativeID").AsInt32().AutoIncrement(this)
            .WithColumn("CustomerID").AsString(5).NotNullable().PrimaryKey()
                .ForeignKey("FK_CustomerRepresentatives_CustomerID", "Customers", "CustomerID")
            .WithColumn("EmployeeID").AsInt32().NotNullable().PrimaryKey();

        Create.Table("EmployeeTerritories")
            .WithColumn("ID").AsInt32().IdentityKey(this)
            .WithColumn("EmployeeID").AsInt32().NotNullable()
                .ForeignKey("FK_EmployeeTerritories_EmployeeID", "Employees", "EmployeeID")
            .WithColumn("TerritoryID").AsString(20).NotNullable()
                .ForeignKey("FK_EmployeeTerritories_TerritoryID", "Territories", "TerritoryID");

        Create.Table("Products")
            .WithColumn("ProductID").AsInt32().IdentityKey(this)
            .WithColumn("ProductName").AsString(40).NotNullable()
            .WithColumn("ProductImage").AsString(200).Nullable()
            .WithColumn("SupplierID").AsInt32().NotNullable()
                .ForeignKey("FK_Products_SupplierID", "Suppliers", "SupplierID")
            .WithColumn("CategoryID").AsInt32().NotNullable()
                .ForeignKey("FK_Products_CategoryID", "Categories", "CategoryID")
            .WithColumn("QuantityPerUnit").AsString(20).Nullable()
            .WithColumn("UnitPrice").AsCurrency().Nullable().WithDefaultValue(0)
            .WithColumn("UnitsInStock").AsInt16().Nullable().WithDefaultValue(0)
            .WithColumn("UnitsOnOrder").AsInt16().Nullable().WithDefaultValue(0)
            .WithColumn("ReorderLevel").AsInt16().Nullable().WithDefaultValue(0)
            .WithColumn("Discontinued").AsBoolean().NotNullable().WithDefaultValue(false);

        Create.Table("ProductLang")
            .WithColumn("ID").AsInt32().IdentityKey(this)
            .WithColumn("ProductID").AsInt32().NotNullable()
            .WithColumn("LanguageID").AsString(10).NotNullable()
            .WithColumn("ProductName").AsString(40).Nullable();

        Create.Table("Orders")
            .WithColumn("OrderID").AsInt32().IdentityKey(this)
            .WithColumn("CustomerID").AsString(5).Nullable()
                .ForeignKey("FK_Orders_CustomerID", "Customers", "CustomerID")
            .WithColumn("EmployeeID").AsInt32().Nullable()
                .ForeignKey("FK_Orders_EmployeeID", "Employees", "EmployeeID")
            .WithColumn("OrderDate").AsDateTime().Nullable()
            .WithColumn("RequiredDate").AsDateTime().Nullable()
            .WithColumn("ShippedDate").AsDateTime().Nullable()
            .WithColumn("ShipVia").AsInt32().Nullable()
                .ForeignKey("FK_Orders_ShipVia", "Shippers", "ShipperID")
            .WithColumn("Freight").AsCurrency().Nullable().WithDefaultValue(0)
            .WithColumn("ShipName").AsString(40).Nullable()
            .WithColumn("ShipAddress").AsString(60).Nullable()
            .WithColumn("ShipCity").AsString(30).Nullable()
            .WithColumn("ShipRegion").AsString(30).Nullable()
            .WithColumn("ShipPostalCode").AsString(10).Nullable()
            .WithColumn("ShipCountry").AsString(30).Nullable();

        Create.Table("OrderDetails")
            .WithColumn("DetailID").AsInt32().IdentityKey(this)
            .WithColumn("OrderID").AsInt32().NotNullable()
                .ForeignKey("FK_OrderDetails_OrderID", "Orders", "OrderID")
            .WithColumn("ProductID").AsInt32().NotNullable()
                .ForeignKey("FK_OrderDetails_ProductID", "Products", "ProductID")
            .WithColumn("UnitPrice").AsCurrency().NotNullable().WithDefaultValue(0)
            .WithColumn("Quantity").AsInt16().NotNullable().WithDefaultValue(1)
            .WithColumn("Discount").AsDouble().NotNullable().WithDefaultValue(0);

        Create.Table("Notes")
            .WithColumn("NoteID").AsInt64().IdentityKey(this)
            .WithColumn("EntityType").AsString(100).NotNullable()
            .WithColumn("EntityID").AsString().NotNullable()
            .WithColumn("Text").AsString(int.MaxValue).NotNullable()
            .WithColumn("InsertUserId").AsInt32().Nullable()
            .WithColumn("InsertDate").AsDateTime().NotNullable();

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

        Create.Table("DragDropSample")
            .WithColumn("Id").AsInt32().IdentityKey(this)
            .WithColumn("ParentId").AsInt32().Nullable()
            .WithColumn("Title").AsString(100).NotNullable();

        IfDatabase("SqlServer")
            .Execute.Sql("""
            create view [dbo].[OrderDetailsExtended] AS
            SELECT OrderDetails.OrderID, OrderDetails.ProductID, Products.ProductName, 
                OrderDetails.UnitPrice, OrderDetails.Quantity, OrderDetails.Discount, 
                (CONVERT(money,(OrderDetails.UnitPrice*Quantity*(1-Discount)/100))*100) AS ExtendedPrice
            FROM Products INNER JOIN OrderDetails ON Products.ProductID = OrderDetails.ProductID
            GO
            create view [dbo].[SalesByCategory] AS
            SELECT Categories.CategoryID, Categories.CategoryName, Products.ProductName, 
                Sum("OrderDetailsExtended".ExtendedPrice) AS ProductSales
            FROM 	Categories INNER JOIN 
                    (Products INNER JOIN 
                        (Orders INNER JOIN "OrderDetailsExtended" ON Orders.OrderID = "OrderDetailsExtended".OrderID) 
                    ON Products.ProductID = "OrderDetailsExtended".ProductID) 
                ON Categories.CategoryID = Products.CategoryID
            GROUP BY Categories.CategoryID, Categories.CategoryName, Products.ProductName
            
            """);

        IfDatabase("Sqlite")
            .Execute.Sql("""
            DROP VIEW IF EXISTS [OrderDetailsExtended];
            CREATE VIEW [OrderDetailsExtended] AS
            SELECT [OrderDetails].OrderID,
                   [OrderDetails].ProductID,
                   Products.ProductName,
                   [OrderDetails].UnitPrice,
                   [OrderDetails].Quantity,
                   [OrderDetails].Discount,
                 ([OrderDetails].UnitPrice*Quantity*(1-Discount)/100)*100 AS ExtendedPrice
            FROM Products 
                 JOIN [OrderDetails] ON Products.ProductID = [OrderDetails].ProductID;

            DROP VIEW IF EXISTS [SalesByCategory];
            CREATE VIEW [SalesByCategory] AS
            SELECT Categories.CategoryID,
                   Categories.CategoryName,
                     Products.ProductName,
               Sum([OrderDetailsExtended].ExtendedPrice) AS ProductSales
            FROM  Categories 
                JOIN Products 
                  ON Categories.CategoryID = Products.CategoryID
                   JOIN [OrderDetailsExtended] 
                     ON Products.ProductID = [OrderDetailsExtended].ProductID
                       JOIN Orders 
                         ON Orders.OrderID = [OrderDetailsExtended].OrderID 
            WHERE Orders.OrderDate BETWEEN DATETIME('1997-01-01') And DATETIME('1997-12-31')
            GROUP BY Categories.CategoryID, Categories.CategoryName, Products.ProductName;
            """);
    }

    public override void Down()
    {
    }
}