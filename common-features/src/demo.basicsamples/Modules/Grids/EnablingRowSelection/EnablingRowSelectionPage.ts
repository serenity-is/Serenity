import { gridPageInit } from "@serenity-is/corelib";
import { SupplierColumns, SupplierDialog, SupplierRow, SupplierService } from "@serenity-is/demo.northwind";
import { SelectableEntityGrid } from "@serenity-is/extensions";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";

export default () => gridPageInit(RowSelectionGrid);

export class RowSelectionGrid<P = {}> extends SelectableEntityGrid<SupplierRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    protected override getColumnsKey() { return SupplierColumns.columnsKey; }
    protected override getDialogType() { return <any>SupplierDialog; }
    protected override getRowDefinition() { return SupplierRow; }
    protected override getService() { return SupplierService.baseUrl; }
}