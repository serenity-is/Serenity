import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { SalesByCategoryRow } from "./Northwind.SalesByCategoryRow";

export interface SalesByCategoryColumns {
    CategoryName: Column<SalesByCategoryRow>;
    ProductName: Column<SalesByCategoryRow>;
    ProductSales: Column<SalesByCategoryRow>;
}

export class SalesByCategoryColumns extends ColumnsBase<SalesByCategoryRow> {
    static readonly columnsKey = 'Northwind.SalesByCategory';
    static readonly Fields = fieldsProxy<SalesByCategoryColumns>();
}