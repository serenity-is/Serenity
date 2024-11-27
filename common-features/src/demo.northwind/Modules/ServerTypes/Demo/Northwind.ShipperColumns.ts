import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { ShipperRow } from "./Northwind.ShipperRow";

export interface ShipperColumns {
    ShipperID: Column<ShipperRow>;
    CompanyName: Column<ShipperRow>;
    Phone: Column<ShipperRow>;
}

export class ShipperColumns extends ColumnsBase<ShipperRow> {
    static readonly columnsKey = 'Northwind.Shipper';
    static readonly Fields = fieldsProxy<ShipperColumns>();
}