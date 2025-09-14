import { EntityGrid } from "@serenity-is/corelib";
import { ShipperColumns, ShipperRow, ShipperService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { ShipperDialog } from "./ShipperDialog";

export class ShipperGrid<P = {}> extends EntityGrid<ShipperRow, P> {
    static [Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected getColumnsKey() { return ShipperColumns.columnsKey; }
    protected getDialogType() { return <any>ShipperDialog; }
    protected getRowDefinition() { return ShipperRow; }
    protected getService() { return ShipperService.baseUrl; }
}
