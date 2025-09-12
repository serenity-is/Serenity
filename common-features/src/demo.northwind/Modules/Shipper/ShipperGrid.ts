import { EntityGrid } from "@serenity-is/corelib";
import { ShipperColumns, ShipperRow, ShipperService } from "../ServerTypes/Demo";
import { ShipperDialog } from "./ShipperDialog";

export class ShipperGrid<P = {}> extends EntityGrid<ShipperRow, P> {
    static override typeInfo = this.registerClass("Serenity.Demo.Northwind.ShipperGrid");

    protected getColumnsKey() { return ShipperColumns.columnsKey; }
    protected getDialogType() { return <any>ShipperDialog; }
    protected getRowDefinition() { return ShipperRow; }
    protected getService() { return ShipperService.baseUrl; }
}
