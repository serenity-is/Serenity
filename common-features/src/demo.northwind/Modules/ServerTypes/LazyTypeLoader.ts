import { Config } from "@serenity-is/corelib";

const loaderByKey = {
    "Serenity.Demo.Norhtwind.CustomerGrid": async () => (await import("../Customer/CustomerGrid")).CustomerGrid,
    "Serenity.Demo.Northwind.CategoryDialog": async () => (await import("../Category/CategoryDialog")).CategoryDialog,
    "Serenity.Demo.Northwind.CategoryGrid": async () => (await import("../Category/CategoryGrid")).CategoryGrid,
    "Serenity.Demo.Northwind.CustomerDialog": async () => (await import("../Customer/CustomerDialog")).CustomerDialog,
    "Serenity.Demo.Northwind.CustomerEditor": async () => (await import("../Customer/CustomerEditor")).CustomerEditor,
    "Serenity.Demo.Northwind.CustomerOrderDialog": async () => (await import("../Customer/CustomerOrderDialog")).CustomerOrderDialog,
    "Serenity.Demo.Northwind.EmployeeFormatter": async () => (await import("../Employee/EmployeeFormatter")).EmployeeFormatter,
    "Serenity.Demo.Northwind.EmployeeListFormatter": async () => (await import("../Customer/EmployeeListFormatter")).EmployeeListFormatter,
    "Serenity.Demo.Northwind.FreightFormatter": async () => (await import("../Order/FreightFormatter")).FreightFormatter,
    "Serenity.Demo.Northwind.NoteDialog": async () => (await import("../Note/NoteDialog")).NoteDialog,
    "Serenity.Demo.Northwind.NotesEditor": async () => (await import("../Note/NotesEditor")).NotesEditor,
    "Serenity.Demo.Northwind.OrderDetailDialog": async () => (await import("../OrderDetail/OrderDetailDialog")).OrderDetailDialog,
    "Serenity.Demo.Northwind.OrderDetailsEditor": async () => (await import("../OrderDetail/OrderDetailsEditor")).OrderDetailsEditor,
    "Serenity.Demo.Northwind.OrderDialog": async () => (await import("../Order/OrderDialog")).OrderDialog,
    "Serenity.Demo.Northwind.OrderGrid": async () => (await import("../Order/OrderGrid")).OrderGrid,
    "Serenity.Demo.Northwind.PhoneEditor": async () => (await import("../Shared/PhoneEditor")).PhoneEditor,
    "Serenity.Demo.Northwind.RegionDialog": async () => (await import("../Region/RegionDialog")).RegionDialog,
    "Serenity.Demo.Northwind.RegionGrid": async () => (await import("../Region/RegionGrid")).RegionGrid,
    "Serenity.Demo.Northwind.ShipperFormatter": async () => (await import("../Shipper/ShipperFormatter")).ShipperFormatter,
    "Serenity.Demo.Northwind.SupplierDialog": async () => (await import("../Supplier/SupplierDialog")).SupplierDialog,
    "Serenity.Demo.Northwind.SupplierGrid": async () => (await import("../Supplier/SupplierGrid")).SupplierGrid,
    "Serenity.Demo.Northwind.TerritoryDialog": async () => (await import("../Territory/TerritoryDialog")).TerritoryDialog,
    "Serenity.Demo.Northwind.TerritoryGrid": async () => (await import("../Territory/TerritoryGrid")).TerritoryGrid
}

Config.lazyTypeLoader = (function(org: any) {
    return (key: string, type: any) => loaderByKey[key]?.() || org?.(key, type);
})(Config.lazyTypeLoader);