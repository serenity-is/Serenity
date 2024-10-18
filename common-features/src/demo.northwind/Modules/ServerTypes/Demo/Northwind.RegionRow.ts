import { getLookup, getLookupAsync, fieldsProxy } from "@serenity-is/corelib";

export interface RegionRow {
    RegionID?: number;
    RegionDescription?: string;
}

export abstract class RegionRow {
    static readonly idProperty = 'RegionID';
    static readonly nameProperty = 'RegionDescription';
    static readonly localTextPrefix = 'Northwind.Region';
    static readonly lookupKey = 'Northwind.Region';

    /** @deprecated use getLookupAsync instead */
    static getLookup() { return getLookup<RegionRow>('Northwind.Region') }
    static async getLookupAsync() { return getLookupAsync<RegionRow>('Northwind.Region') }

    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<RegionRow>();
}