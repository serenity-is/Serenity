import { fieldsProxy } from "@serenity-is/corelib";

export interface CustomerDemographicRow {
    ID?: number;
    CustomerTypeID?: string;
    CustomerDesc?: string;
}

export abstract class CustomerDemographicRow {
    static readonly idProperty = 'ID';
    static readonly nameProperty = 'CustomerTypeID';
    static readonly localTextPrefix = 'Northwind.CustomerDemographic';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<CustomerDemographicRow>();
}