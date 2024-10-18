import { Decorators, EntityGrid } from "@serenity-is/corelib";
import { CustomerColumns, CustomerRow, CustomerService } from "../ServerTypes/Demo";
import { CustomerDialog } from "./CustomerDialog";

@Decorators.registerClass('Serenity.Demo.Norhtwind.CustomerGrid')
@Decorators.filterable()
export class CustomerGrid<P={}> extends EntityGrid<CustomerRow, P> {
    protected getColumnsKey() { return CustomerColumns.columnsKey; }
    protected getDialogType() { return <any>CustomerDialog; }
    protected getRowDefinition() { return CustomerRow; }
    protected getService() { return CustomerService.baseUrl; }
}
