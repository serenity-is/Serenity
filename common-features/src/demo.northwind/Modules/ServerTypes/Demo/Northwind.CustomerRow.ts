import { getLookup, getLookupAsync, fieldsProxy } from "@serenity-is/corelib";
import { NoteRow } from "./Northwind.NoteRow";

export interface CustomerRow {
    ID?: number;
    CustomerID?: string;
    CompanyName?: string;
    ContactName?: string;
    ContactTitle?: string;
    Address?: string;
    City?: string;
    Region?: string;
    PostalCode?: string;
    Country?: string;
    Phone?: string;
    Fax?: string;
    NoteList?: NoteRow[];
    Representatives?: number[];
    LastContactDate?: string;
    LastContactedBy?: number;
    Email?: string;
    SendBulletin?: boolean;
}

export abstract class CustomerRow {
    static readonly idProperty = 'ID';
    static readonly nameProperty = 'CompanyName';
    static readonly localTextPrefix = 'Northwind.Customer';
    static readonly lookupKey = 'Northwind.Customer';

    /** @deprecated use getLookupAsync instead */
    static getLookup() { return getLookup<CustomerRow>('Northwind.Customer') }
    static async getLookupAsync() { return getLookupAsync<CustomerRow>('Northwind.Customer') }

    static readonly deletePermission = 'Northwind:Customer:Delete';
    static readonly insertPermission = 'Northwind:Customer:Modify';
    static readonly readPermission = 'Northwind:Customer:View';
    static readonly updatePermission = 'Northwind:Customer:Modify';

    static readonly Fields = fieldsProxy<CustomerRow>();
}