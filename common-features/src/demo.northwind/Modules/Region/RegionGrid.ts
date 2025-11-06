import { EntityGrid } from "@serenity-is/corelib";
import { RegionColumns, RegionRow, RegionService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { RegionDialog } from "./RegionDialog";

export class RegionGrid<P = {}> extends EntityGrid<RegionRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected override getColumnsKey() { return RegionColumns.columnsKey; }
    protected override getDialogType() { return <any>RegionDialog; }
    protected override getRowDefinition() { return RegionRow; }
    protected override getService() { return RegionService.baseUrl; }
}
