using Serenity;
using Serenity.Navigation;
using Northwind = BasicApplication.Northwind.Pages;
using Administration = BasicApplication.Administration.Pages;

[assembly: NavigationLink(1000, "Dashboard", url: "~/", permission: "", icon: "icon-speedometer")]

[assembly: NavigationMenu(8000, "Northwind", icon: "icon-anchor")]
[assembly: NavigationLink(8200, "Northwind/Customers", typeof(Northwind.CustomerController), icon: "icon-wallet")]
[assembly: NavigationLink(8300, "Northwind/Products", typeof(Northwind.ProductController), icon: "icon-present")]
[assembly: NavigationLink(8400, "Northwind/Suppliers", typeof(Northwind.SupplierController), icon: "icon-magic-wand")]
[assembly: NavigationLink(8500, "Northwind/Shippers", typeof(Northwind.ShipperController), icon: "icon-plane")]
[assembly: NavigationLink(8600, "Northwind/Categories", typeof(Northwind.CategoryController), icon: "icon-folder-alt")]
[assembly: NavigationLink(8700, "Northwind/Regions", typeof(Northwind.RegionController), icon: "icon-map")]
[assembly: NavigationLink(8800, "Northwind/Territories", typeof(Northwind.TerritoryController), icon: "icon-puzzle")]

[assembly: NavigationMenu(9000, "Administration", icon: "icon-screen-desktop")]
[assembly: NavigationLink(9100, "Administration/Languages", typeof(Administration.LanguageController), icon: "icon-bubbles")]
[assembly: NavigationLink(9200, "Administration/Translations", typeof(Administration.TranslationController), icon: "icon-speech")]
[assembly: NavigationLink(9900, "Administration/User Management", typeof(Administration.UserController), icon: "icon-users")]