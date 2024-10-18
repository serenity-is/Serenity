import { getLookup, getLookupAsync, fieldsProxy } from "@serenity-is/corelib";

export interface TerritoryRow {
    ID?: number;
    TerritoryID?: string;
    TerritoryDescription?: string;
    RegionID?: number;
    RegionDescription?: string;
}

export abstract class TerritoryRow {
    static readonly idProperty = 'ID';
    static readonly nameProperty = 'TerritoryID';
    static readonly localTextPrefix = 'Northwind.Territory';
    static readonly lookupKey = 'Northwind.Territory';

    /** @deprecated use getLookupAsync instead */
    static getLookup() { return getLookup<TerritoryRow>('Northwind.Territory') }
    static async getLookupAsync() { return getLookupAsync<TerritoryRow>('Northwind.Territory') }

    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<TerritoryRow>();
}