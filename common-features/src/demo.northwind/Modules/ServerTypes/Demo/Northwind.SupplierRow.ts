import { getLookup, getLookupAsync, fieldsProxy } from "@serenity-is/corelib";

export interface SupplierRow {
    SupplierID?: number;
    CompanyName?: string;
    ContactName?: string;
    ContactTitle?: string;
    Address?: string;
    City?: string;
    Region?: string;
    PostalCode?: string;
    Country?: string;
    Phone?: string;
    Fax?: string;
    HomePage?: string;
}

export abstract class SupplierRow {
    static readonly idProperty = 'SupplierID';
    static readonly nameProperty = 'CompanyName';
    static readonly localTextPrefix = 'Northwind.Supplier';
    static readonly lookupKey = 'Northwind.Supplier';

    /** @deprecated use getLookupAsync instead */
    static getLookup() { return getLookup<SupplierRow>('Northwind.Supplier') }
    static async getLookupAsync() { return getLookupAsync<SupplierRow>('Northwind.Supplier') }

    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<SupplierRow>();
}