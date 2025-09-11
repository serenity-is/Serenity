import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { ProductRow } from "./Northwind.ProductRow";

export interface ProductColumns {
    ProductID: Column<ProductRow>;
    ProductName: Column<ProductRow>;
    Discontinued: Column<ProductRow>;
    SupplierCompanyName: Column<ProductRow>;
    CategoryName: Column<ProductRow>;
    QuantityPerUnit: Column<ProductRow>;
    UnitPrice: Column<ProductRow>;
    UnitsInStock: Column<ProductRow>;
    UnitsOnOrder: Column<ProductRow>;
    ReorderLevel: Column<ProductRow>;
}

export class ProductColumns extends ColumnsBase<ProductRow> {
    static readonly columnsKey = 'Northwind.Product';
    static readonly Fields = fieldsProxy<ProductColumns>();
}