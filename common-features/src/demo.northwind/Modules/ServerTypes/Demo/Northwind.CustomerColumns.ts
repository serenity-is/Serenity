import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { EmployeeListFormatter } from "../../Customer/EmployeeListFormatter";
import { CustomerRow } from "./Northwind.CustomerRow";

export interface CustomerColumns {
    CustomerID: Column<CustomerRow>;
    CompanyName: Column<CustomerRow>;
    ContactName: Column<CustomerRow>;
    ContactTitle: Column<CustomerRow>;
    Region: Column<CustomerRow>;
    PostalCode: Column<CustomerRow>;
    Country: Column<CustomerRow>;
    City: Column<CustomerRow>;
    Phone: Column<CustomerRow>;
    Fax: Column<CustomerRow>;
    Representatives: Column<CustomerRow>;
}

export class CustomerColumns extends ColumnsBase<CustomerRow> {
    static readonly columnsKey = 'Northwind.Customer';
    static readonly Fields = fieldsProxy<CustomerColumns>();
}

queueMicrotask(() => [EmployeeListFormatter]); // referenced types