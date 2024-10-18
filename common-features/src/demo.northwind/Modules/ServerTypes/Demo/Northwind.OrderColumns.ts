import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { EmployeeFormatter } from "../../Employee/EmployeeFormatter";
import { FreightFormatter } from "../../Order/FreightFormatter";
import { ShipperFormatter } from "../../Shipper/ShipperFormatter";
import { OrderRow } from "./Northwind.OrderRow";
import { OrderShippingState } from "./Northwind.OrderShippingState";

export interface OrderColumns {
    OrderID: Column<OrderRow>;
    CustomerCompanyName: Column<OrderRow>;
    OrderDate: Column<OrderRow>;
    EmployeeFullName: Column<OrderRow>;
    RequiredDate: Column<OrderRow>;
    ShippingState: Column<OrderRow>;
    ShippedDate: Column<OrderRow>;
    ShipViaCompanyName: Column<OrderRow>;
    ShipCountry: Column<OrderRow>;
    ShipCity: Column<OrderRow>;
    Freight: Column<OrderRow>;
}

export class OrderColumns extends ColumnsBase<OrderRow> {
    static readonly columnsKey = 'Northwind.Order';
    static readonly Fields = fieldsProxy<OrderColumns>();
}

queueMicrotask(() => [EmployeeFormatter, OrderShippingState, ShipperFormatter, FreightFormatter]); // referenced types