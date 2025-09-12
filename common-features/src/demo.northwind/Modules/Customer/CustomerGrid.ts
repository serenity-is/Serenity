import { EntityGrid, FilterableAttribute } from "@serenity-is/corelib";
import { CustomerColumns, CustomerRow, CustomerService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { CustomerDialog } from "./CustomerDialog";

export class CustomerGrid<P = {}> extends EntityGrid<CustomerRow, P> {
    static override typeInfo = this.registerClass(nsDemoNorthwind, [new FilterableAttribute()]);

    protected getColumnsKey() { return CustomerColumns.columnsKey; }
    protected getDialogType() { return <any>CustomerDialog; }
    protected getRowDefinition() { return CustomerRow; }
    protected getService() { return CustomerService.baseUrl; }
}
