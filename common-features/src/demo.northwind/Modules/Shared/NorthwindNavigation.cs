using Serenity.Navigation;
using Northwind = Serenity.Demo.Northwind;

[assembly: NavigationMenu(7000, "Northwind", icon: "fa-anchor")]
[assembly: NavigationLink(7100, "Northwind/Customers", typeof(Northwind.CustomerPage), icon: "fa-credit-card")]
[assembly: NavigationLink(7200, "Northwind/Orders", typeof(Northwind.OrderPage), icon: "fa-shopping-cart")]
[assembly: NavigationLink(7300, "Northwind/Products", typeof(Northwind.ProductPage), icon: "fa-cube")]
[assembly: NavigationLink(7400, "Northwind/Suppliers", typeof(Northwind.SupplierPage), icon: "fa-truck")]
[assembly: NavigationLink(7500, "Northwind/Shippers", typeof(Northwind.ShipperPage), icon: "fa-ship")]
[assembly: NavigationLink(7600, "Northwind/Categories", typeof(Northwind.CategoryPage), icon: "fa-folder-o")]
[assembly: NavigationLink(7700, "Northwind/Regions", typeof(Northwind.RegionPage), icon: "fa-map-o")]
[assembly: NavigationLink(7800, "Northwind/Territories", typeof(Northwind.TerritoryPage), icon: "fa-puzzle-piece")]
[assembly: NavigationLink(7900, "Northwind/Reports", typeof(Northwind.ReportsPage), icon: "fa-files-o")]
