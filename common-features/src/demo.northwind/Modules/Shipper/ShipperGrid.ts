import { EntityGrid } from "@serenity-is/corelib";
import { ShipperColumns, ShipperRow, ShipperService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { ShipperDialog } from "./ShipperDialog";

export class ShipperGrid<P = {}> extends EntityGrid<ShipperRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected override getColumnsKey() { return ShipperColumns.columnsKey; }
    protected override getDialogType() { return <any>ShipperDialog; }
    protected override getRowDefinition() { return ShipperRow; }
    protected override getService() { return ShipperService.baseUrl; }
}
