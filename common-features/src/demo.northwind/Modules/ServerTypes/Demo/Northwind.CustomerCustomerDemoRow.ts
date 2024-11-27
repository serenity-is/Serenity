import { fieldsProxy } from "@serenity-is/corelib";

export interface CustomerCustomerDemoRow {
    ID?: number;
    CustomerID?: string;
    CustomerTypeID?: string;
    CustomerCompanyName?: string;
}

export abstract class CustomerCustomerDemoRow {
    static readonly idProperty = 'ID';
    static readonly nameProperty = 'CustomerID';
    static readonly localTextPrefix = 'Northwind.CustomerCustomerDemo';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<CustomerCustomerDemoRow>();
}