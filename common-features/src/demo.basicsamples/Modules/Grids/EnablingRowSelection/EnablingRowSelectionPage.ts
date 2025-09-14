import { gridPageInit } from "@serenity-is/corelib";
import { SupplierColumns, SupplierDialog, SupplierRow, SupplierService } from "@serenity-is/demo.northwind";
import { SelectableEntityGrid } from "@serenity-is/extensions";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";

export default () => gridPageInit(RowSelectionGrid);

export class RowSelectionGrid<P = {}> extends SelectableEntityGrid<SupplierRow, P> {
    static [Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    protected getColumnsKey() { return SupplierColumns.columnsKey; }
    protected getDialogType() { return <any>SupplierDialog; }
    protected getRowDefinition() { return SupplierRow; }
    protected getService() { return SupplierService.baseUrl; }
}