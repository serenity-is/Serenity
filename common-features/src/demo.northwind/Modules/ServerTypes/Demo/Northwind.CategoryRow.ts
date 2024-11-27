import { getLookup, getLookupAsync, fieldsProxy } from "@serenity-is/corelib";

export interface CategoryRow {
    CategoryID?: number;
    CategoryName?: string;
    Description?: string;
    Picture?: number[];
}

export abstract class CategoryRow {
    static readonly idProperty = 'CategoryID';
    static readonly nameProperty = 'CategoryName';
    static readonly localTextPrefix = 'Northwind.Category';
    static readonly lookupKey = 'Northwind.Category';

    /** @deprecated use getLookupAsync instead */
    static getLookup() { return getLookup<CategoryRow>('Northwind.Category') }
    static async getLookupAsync() { return getLookupAsync<CategoryRow>('Northwind.Category') }

    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<CategoryRow>();
}