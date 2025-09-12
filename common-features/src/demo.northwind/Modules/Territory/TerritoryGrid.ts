import { EntityGrid } from "@serenity-is/corelib";
import { RegionDialog } from "../Region/RegionDialog";
import { TerritoryColumns, TerritoryRow, TerritoryService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { TerritoryDialog } from "./TerritoryDialog";

export class TerritoryGrid<P = {}> extends EntityGrid<TerritoryRow, P> {
    static override typeInfo = this.registerClass(nsDemoNorthwind);

    protected getColumnsKey() { return TerritoryColumns.columnsKey; }
    protected getDialogType() { return <any>TerritoryDialog; }
    protected getRowDefinition() { return TerritoryRow; }
    protected getService() { return TerritoryService.baseUrl; }
}

[RegionDialog]