import { fieldsProxy } from "@serenity-is/corelib";

export interface CustomerDetailsRow {
    Id?: number;
    LastContactDate?: string;
    LastContactedBy?: number;
    Email?: string;
    SendBulletin?: boolean;
    LastContactedByFullName?: string;
}

export abstract class CustomerDetailsRow {
    static readonly idProperty = 'Id';
    static readonly nameProperty = 'Email';
    static readonly localTextPrefix = 'Northwind.CustomerDetails';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<CustomerDetailsRow>();
}