import { Decorators, EntityGrid } from "@serenity-is/corelib";
import { RegionColumns, RegionRow, RegionService } from "../ServerTypes/Demo";
import { RegionDialog } from "./RegionDialog";

@Decorators.registerClass('Serenity.Demo.Northwind.RegionGrid')
export class RegionGrid<P = {}> extends EntityGrid<RegionRow, P> {
    protected getColumnsKey() { return RegionColumns.columnsKey; }
    protected getDialogType() { return <any>RegionDialog; }
    protected getRowDefinition() { return RegionRow; }
    protected getService() { return RegionService.baseUrl; }
}
