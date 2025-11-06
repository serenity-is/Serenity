import { EntityGrid } from "@serenity-is/corelib";
import { SupplierColumns, SupplierRow, SupplierService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { SupplierDialog } from "./SupplierDialog";

export class SupplierGrid<P = {}> extends EntityGrid<SupplierRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected override getColumnsKey() { return SupplierColumns.columnsKey; }
    protected override getDialogType() { return <any>SupplierDialog; }
    protected override getRowDefinition() { return SupplierRow; }
    protected override getService() { return SupplierService.baseUrl; }
}
