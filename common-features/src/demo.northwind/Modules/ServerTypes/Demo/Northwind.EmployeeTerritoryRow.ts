import { fieldsProxy } from "@serenity-is/corelib";

export interface EmployeeTerritoryRow {
    EmployeeID?: number;
    TerritoryID?: string;
    EmployeeFullName?: string;
    TerritoryDescription?: string;
}

export abstract class EmployeeTerritoryRow {
    static readonly idProperty = 'EmployeeID';
    static readonly nameProperty = 'TerritoryID';
    static readonly localTextPrefix = 'Northwind.EmployeeTerritory';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<EmployeeTerritoryRow>();
}