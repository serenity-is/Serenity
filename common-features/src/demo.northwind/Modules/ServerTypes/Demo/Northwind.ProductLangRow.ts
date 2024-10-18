import { fieldsProxy } from "@serenity-is/corelib";

export interface ProductLangRow {
    Id?: number;
    ProductId?: number;
    LanguageId?: number;
    ProductName?: string;
}

export abstract class ProductLangRow {
    static readonly idProperty = 'Id';
    static readonly nameProperty = 'ProductName';
    static readonly localTextPrefix = 'Northwind.ProductLang';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<ProductLangRow>();
}