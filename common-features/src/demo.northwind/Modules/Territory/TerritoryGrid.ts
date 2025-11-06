import { EntityGrid } from "@serenity-is/corelib";
import { RegionDialog } from "../Region/RegionDialog";
import { TerritoryColumns, TerritoryRow, TerritoryService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { TerritoryDialog } from "./TerritoryDialog";

export class TerritoryGrid<P = {}> extends EntityGrid<TerritoryRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected override getColumnsKey() { return TerritoryColumns.columnsKey; }
    protected override getDialogType() { return <any>TerritoryDialog; }
    protected override getRowDefinition() { return TerritoryRow; }
    protected override getService() { return TerritoryService.baseUrl; }
}

[RegionDialog]