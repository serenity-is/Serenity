import { fieldsProxy } from "@serenity-is/corelib";
import { Gender } from "./Northwind.Gender";
import { OrderDetailRow } from "./Northwind.OrderDetailRow";
import { OrderShippingState } from "./Northwind.OrderShippingState";

export interface OrderRow {
    OrderID?: number;
    CustomerID?: string;
    EmployeeID?: number;
    OrderDate?: string;
    RequiredDate?: string;
    ShippedDate?: string;
    ShipVia?: number;
    Freight?: number;
    ShipName?: string;
    ShipAddress?: string;
    ShipCity?: string;
    ShipRegion?: string;
    ShipPostalCode?: string;
    ShipCountry?: string;
    CustomerCompanyName?: string;
    CustomerContactName?: string;
    CustomerContactTitle?: string;
    CustomerCity?: string;
    CustomerRegion?: string;
    CustomerCountry?: string;
    CustomerPhone?: string;
    CustomerFax?: string;
    EmployeeFullName?: string;
    EmployeeGender?: Gender;
    ShipViaCompanyName?: string;
    ShippingState?: OrderShippingState;
    DetailList?: OrderDetailRow[];
}

export abstract class OrderRow {
    static readonly idProperty = 'OrderID';
    static readonly nameProperty = 'CustomerID';
    static readonly localTextPrefix = 'Northwind.Order';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<OrderRow>();
}