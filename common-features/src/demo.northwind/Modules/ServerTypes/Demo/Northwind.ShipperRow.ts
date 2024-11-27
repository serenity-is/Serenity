import { getLookup, getLookupAsync, fieldsProxy } from "@serenity-is/corelib";

export interface ShipperRow {
    ShipperID?: number;
    CompanyName?: string;
    Phone?: string;
}

export abstract class ShipperRow {
    static readonly idProperty = 'ShipperID';
    static readonly nameProperty = 'CompanyName';
    static readonly localTextPrefix = 'Northwind.Shipper';
    static readonly lookupKey = 'Northwind.Shipper';

    /** @deprecated use getLookupAsync instead */
    static getLookup() { return getLookup<ShipperRow>('Northwind.Shipper') }
    static async getLookupAsync() { return getLookupAsync<ShipperRow>('Northwind.Shipper') }

    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<ShipperRow>();
}