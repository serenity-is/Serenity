import { EntityGrid } from "@serenity-is/corelib";
import { RegionColumns, RegionRow, RegionService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { RegionDialog } from "./RegionDialog";

export class RegionGrid<P = {}> extends EntityGrid<RegionRow, P> {
    static [Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected getColumnsKey() { return RegionColumns.columnsKey; }
    protected getDialogType() { return <any>RegionDialog; }
    protected getRowDefinition() { return RegionRow; }
    protected getService() { return RegionService.baseUrl; }
}
