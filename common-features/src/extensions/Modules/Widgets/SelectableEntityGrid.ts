import { EntityGrid } from "@serenity-is/corelib";
import { GridOptions, RowSelectionModel } from "@serenity-is/sleekgrid";
import { nsExtensions } from "../ServerTypes/Namespaces";

export class SelectableEntityGrid<TItem, TOptions> extends EntityGrid<TItem, TOptions> {
    static override[Symbol.typeInfo] = this.registerClass(nsExtensions);

    protected override getSlickOptions(): GridOptions {
        var opt = super.getSlickOptions();
        opt.enableTextSelectionOnCells = true;
        opt.selectedCellCssClass = "slick-row-selected";
        opt.enableCellNavigation = true;
        return opt;
    }

    protected override initSleekGrid() {
        super.initSleekGrid();
        
        this.sleekGrid.setSelectionModel(new RowSelectionModel());
    }
}
