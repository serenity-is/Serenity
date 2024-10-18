import { fieldsProxy } from "@serenity-is/corelib";

export interface OrderDetailRow {
    DetailID?: number;
    OrderID?: number;
    ProductID?: number;
    UnitPrice?: number;
    Quantity?: number;
    Discount?: number;
    OrderCustomerID?: string;
    OrderEmployeeID?: number;
    OrderDate?: string;
    ProductName?: string;
    LineTotal?: number;
}

export abstract class OrderDetailRow {
    static readonly idProperty = 'DetailID';
    static readonly localTextPrefix = 'Northwind.OrderDetail';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<OrderDetailRow>();
}