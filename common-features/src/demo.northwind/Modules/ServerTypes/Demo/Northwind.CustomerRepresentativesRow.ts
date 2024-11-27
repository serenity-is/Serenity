import { fieldsProxy } from "@serenity-is/corelib";

export interface CustomerRepresentativesRow {
    RepresentativeId?: number;
    CustomerId?: number;
    EmployeeId?: number;
}

export abstract class CustomerRepresentativesRow {
    static readonly idProperty = 'RepresentativeId';
    static readonly localTextPrefix = 'Northwind.CustomerRepresentatives';
    static readonly deletePermission = 'Northwind:Customer:View';
    static readonly insertPermission = 'Northwind:Customer:View';
    static readonly readPermission = 'Northwind:Customer:View';
    static readonly updatePermission = 'Northwind:Customer:View';

    static readonly Fields = fieldsProxy<CustomerRepresentativesRow>();
}