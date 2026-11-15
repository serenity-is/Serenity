import { CategoryDialog } from "../Category/CategoryDialog";
import { CustomerDialog } from "../Customer/CustomerDialog";
import { OrderDialog } from "../Order/OrderDialog";
import { ProductDialog } from "../Product/ProductDialog";
import { RegionDialog } from "../Region/RegionDialog";
import { ShipperDialog } from "../Shipper/ShipperDialog";
import { SupplierDialog } from "../Supplier/SupplierDialog";
import { TerritoryDialog } from "../Territory/TerritoryDialog";

export const lazyTypeGroupNorthwindDialogs: Record<string, any> = {
    "Serenity.Demo.Northwind.CategoryDialog": CategoryDialog,
    "Serenity.Demo.Northwind.CustomerDialog": CustomerDialog,
    "Serenity.Demo.Northwind.OrderDialog": OrderDialog,
    "Serenity.Demo.Northwind.ProductDialog": ProductDialog,
    "Serenity.Demo.Northwind.RegionDialog": RegionDialog,
    "Serenity.Demo.Northwind.ShipperDialog": ShipperDialog,
    "Serenity.Demo.Northwind.SupplierDialog": SupplierDialog,
    "Serenity.Demo.Northwind.TerritoryDialog": TerritoryDialog
}