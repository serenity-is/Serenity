import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { DeleteRowActionFormatter } from "@serenity-is/extensions";
import { Column } from "@serenity-is/sleekgrid";
import { OrderDetailRow } from "./Northwind.OrderDetailRow";

export interface OrderDetailColumns {
    RowEditActions: Column<OrderDetailRow>;
    ProductName: Column<OrderDetailRow>;
    UnitPrice: Column<OrderDetailRow>;
    Quantity: Column<OrderDetailRow>;
    Discount: Column<OrderDetailRow>;
    LineTotal: Column<OrderDetailRow>;
}

export class OrderDetailColumns extends ColumnsBase<OrderDetailRow> {
    static readonly columnsKey = 'Northwind.OrderDetail';
    static readonly Fields = fieldsProxy<OrderDetailColumns>();
}

[DeleteRowActionFormatter]; // referenced types