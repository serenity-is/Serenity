import { fieldsProxy } from "@serenity-is/corelib";

export interface CategoryLangRow {
    Id?: number;
    CategoryId?: number;
    LanguageId?: number;
    CategoryName?: string;
    Description?: string;
}

export abstract class CategoryLangRow {
    static readonly idProperty = 'Id';
    static readonly nameProperty = 'CategoryName';
    static readonly localTextPrefix = 'Northwind.CategoryLang';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<CategoryLangRow>();
}