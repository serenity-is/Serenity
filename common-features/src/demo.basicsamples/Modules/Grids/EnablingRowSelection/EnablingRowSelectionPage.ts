import { Decorators, gridPageInit } from "@serenity-is/corelib";
import { SupplierColumns, SupplierDialog, SupplierRow, SupplierService } from "@serenity-is/demo.northwind";
import { SelectableEntityGrid } from "@serenity-is/extensions";

export default () => gridPageInit(RowSelectionGrid);

@Decorators.registerClass('Serenity.Demo.BasicSamples.RowSelectionGrid')
export class RowSelectionGrid<P = {}> extends SelectableEntityGrid<SupplierRow, P> {
    protected getColumnsKey() { return SupplierColumns.columnsKey; }
    protected getDialogType() { return <any>SupplierDialog; }
    protected getRowDefinition() { return SupplierRow; }
    protected getService() { return SupplierService.baseUrl; }
}