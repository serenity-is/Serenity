export const loaderByKey: Record<string, () => Promise<any>> = {
    "Serenity.Demo.Northwind.CountryWithFlagFormatter": async () => (await import("../Shared/CountryWithFlagFormatter")).CountryWithFlagFormatter,
    "Serenity.Demo.Northwind.EmployeeFormatter": async () => (await import("../Employee/EmployeeFormatter")).EmployeeFormatter,
    "Serenity.Demo.Northwind.FreightFormatter": async () => (await import("../Order/FreightFormatter")).FreightFormatter,
    "Serenity.Demo.Northwind.ShipperFormatter": async () => (await import("../Shipper/ShipperFormatter")).ShipperFormatter
}