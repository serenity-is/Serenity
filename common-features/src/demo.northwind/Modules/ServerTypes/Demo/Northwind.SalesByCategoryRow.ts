import { fieldsProxy } from "@serenity-is/corelib";

export interface SalesByCategoryRow {
    CategoryId?: number;
    CategoryName?: string;
    ProductName?: string;
    ProductSales?: number;
}

export abstract class SalesByCategoryRow {
    static readonly nameProperty = 'CategoryName';
    static readonly localTextPrefix = 'Northwind.SalesByCategory';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<SalesByCategoryRow>();
}