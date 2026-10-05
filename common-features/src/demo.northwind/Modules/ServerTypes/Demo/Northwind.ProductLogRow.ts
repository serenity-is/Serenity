import { CaptureOperationType, fieldsProxy } from "@serenity-is/corelib";

export interface ProductLogRow {
    ProductLogID?: number;
    OperationType?: CaptureOperationType;
    ChangingUserId?: any;
    ValidFrom?: string;
    ValidUntil?: string;
    ProductID?: number;
    ProductName?: string;
    ProductImage?: string;
    Discontinued?: boolean;
    SupplierID?: number;
    CategoryID?: number;
    QuantityPerUnit?: string;
    UnitPrice?: number;
    UnitsInStock?: number;
    UnitsOnOrder?: number;
    ReorderLevel?: number;
}

export abstract class ProductLogRow {
    static readonly idProperty = 'ProductLogID';
    static readonly localTextPrefix = 'Northwind.ProductLog';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<ProductLogRow>();
}