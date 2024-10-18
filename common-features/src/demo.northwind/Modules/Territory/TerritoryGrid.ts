import { Decorators, EntityGrid } from "@serenity-is/corelib";
import { TerritoryColumns, TerritoryRow, TerritoryService } from "../ServerTypes/Demo";
import { TerritoryDialog } from "./TerritoryDialog";

@Decorators.registerClass('Serenity.Demo.Northwind.TerritoryGrid')
export class TerritoryGrid<P = {}> extends EntityGrid<TerritoryRow, P> {
    protected getColumnsKey() { return TerritoryColumns.columnsKey; }
    protected getDialogType() { return <any>TerritoryDialog; }
    protected getRowDefinition() { return TerritoryRow; }
    protected getService() { return TerritoryService.baseUrl; }
}
