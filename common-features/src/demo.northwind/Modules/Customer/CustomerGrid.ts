import { Attributes, EntityGrid } from "@serenity-is/corelib";
import { CustomerColumns, CustomerRow, CustomerService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { CustomerDialog } from "./CustomerDialog";

export class CustomerGrid<P = {}> extends EntityGrid<CustomerRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind, [Attributes.advancedFiltering]);

    protected override getColumnsKey() { return CustomerColumns.columnsKey; }
    protected override getDialogType() { return <any>CustomerDialog; }
    protected override getRowDefinition() { return CustomerRow; }
    protected override getService() { return CustomerService.baseUrl; }
}
