export const loaderByKey: Record<string, () => Promise<any>> = {
    "Serenity.Demo.Northwind.CategoryDialog": async () => (await import("./LazyTypeGroup.NorthwindDialogs")).lazyTypeGroupNorthwindDialogs["Serenity.Demo.Northwind.CategoryDialog"],
    "Serenity.Demo.Northwind.CountryWithFlagFormatter": async () => (await import("../Shared/CountryWithFlagFormatter")).CountryWithFlagFormatter,
    "Serenity.Demo.Northwind.CustomerDialog": async () => (await import("./LazyTypeGroup.NorthwindDialogs")).lazyTypeGroupNorthwindDialogs["Serenity.Demo.Northwind.CustomerDialog"],
    "Serenity.Demo.Northwind.EmployeeFormatter": async () => (await import("../Employee/EmployeeFormatter")).EmployeeFormatter,
    "Serenity.Demo.Northwind.FreightFormatter": async () => (await import("../Order/FreightFormatter")).FreightFormatter,
    "Serenity.Demo.Northwind.OrderDialog": async () => (await import("./LazyTypeGroup.NorthwindDialogs")).lazyTypeGroupNorthwindDialogs["Serenity.Demo.Northwind.OrderDialog"],
    "Serenity.Demo.Northwind.ProductDialog": async () => (await import("./LazyTypeGroup.NorthwindDialogs")).lazyTypeGroupNorthwindDialogs["Serenity.Demo.Northwind.ProductDialog"],
    "Serenity.Demo.Northwind.RegionDialog": async () => (await import("./LazyTypeGroup.NorthwindDialogs")).lazyTypeGroupNorthwindDialogs["Serenity.Demo.Northwind.RegionDialog"],
    "Serenity.Demo.Northwind.ShipperDialog": async () => (await import("./LazyTypeGroup.NorthwindDialogs")).lazyTypeGroupNorthwindDialogs["Serenity.Demo.Northwind.ShipperDialog"],
    "Serenity.Demo.Northwind.ShipperFormatter": async () => (await import("../Shipper/ShipperFormatter")).ShipperFormatter,
    "Serenity.Demo.Northwind.SupplierDialog": async () => (await import("./LazyTypeGroup.NorthwindDialogs")).lazyTypeGroupNorthwindDialogs["Serenity.Demo.Northwind.SupplierDialog"],
    "Serenity.Demo.Northwind.TerritoryDialog": async () => (await import("./LazyTypeGroup.NorthwindDialogs")).lazyTypeGroupNorthwindDialogs["Serenity.Demo.Northwind.TerritoryDialog"]
}