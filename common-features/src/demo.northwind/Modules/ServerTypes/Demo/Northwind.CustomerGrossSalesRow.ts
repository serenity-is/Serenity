import { fieldsProxy } from "@serenity-is/corelib";

export interface CustomerGrossSalesRow {
    CustomerId?: string;
    ContactName?: string;
    ProductId?: number;
    ProductName?: string;
    GrossAmount?: number;
}

export abstract class CustomerGrossSalesRow {
    static readonly nameProperty = 'ContactName';
    static readonly localTextPrefix = 'Northwind.CustomerGrossSales';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<CustomerGrossSalesRow>();
}