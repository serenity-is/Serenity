import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { SupplierRow } from "./Northwind.SupplierRow";

export interface SupplierColumns {
    SupplierID: Column<SupplierRow>;
    CompanyName: Column<SupplierRow>;
    ContactName: Column<SupplierRow>;
    ContactTitle: Column<SupplierRow>;
    Phone: Column<SupplierRow>;
    Region: Column<SupplierRow>;
    Country: Column<SupplierRow>;
    City: Column<SupplierRow>;
}

export class SupplierColumns extends ColumnsBase<SupplierRow> {
    static readonly columnsKey = 'Northwind.Supplier';
    static readonly Fields = fieldsProxy<SupplierColumns>();
}