import { EntityGrid } from "@serenity-is/corelib";
import { SupplierColumns, SupplierRow, SupplierService } from "../ServerTypes/Demo";
import { SupplierDialog } from "./SupplierDialog";

export class SupplierGrid<P = {}> extends EntityGrid<SupplierRow, P> {
    static override typeInfo = this.registerClass("Serenity.Demo.Northwind.SupplierGrid");

    protected getColumnsKey() { return SupplierColumns.columnsKey; }
    protected getDialogType() { return <any>SupplierDialog; }
    protected getRowDefinition() { return SupplierRow; }
    protected getService() { return SupplierService.baseUrl; }
}
