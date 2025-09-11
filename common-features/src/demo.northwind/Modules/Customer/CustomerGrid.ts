import { EntityGrid, FilterableAttribute } from "@serenity-is/corelib";
import { CustomerColumns, CustomerRow, CustomerService } from "../ServerTypes/Demo";
import { CustomerDialog } from "./CustomerDialog";

export class CustomerGrid<P = {}> extends EntityGrid<CustomerRow, P> {
    static override typeInfo = this.classTypeInfo("Serenity.Demo.Norhtwind.CustomerGrid", [new FilterableAttribute()]);

    protected getColumnsKey() { return CustomerColumns.columnsKey; }
    protected getDialogType() { return <any>CustomerDialog; }
    protected getRowDefinition() { return CustomerRow; }
    protected getService() { return CustomerService.baseUrl; }
}
