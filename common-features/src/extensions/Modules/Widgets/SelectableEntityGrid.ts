import { EntityGrid } from "@serenity-is/corelib";
import { GridOptions, RowSelectionModel } from "@serenity-is/sleekgrid";
import { nsExtensions } from "../ServerTypes/Namespaces";

export class SelectableEntityGrid<TItem, TOptions> extends EntityGrid<TItem, TOptions> {
    static override[Symbol.typeInfo] = this.registerClass(nsExtensions);

    protected override getSlickOptions(): GridOptions {
        const opt = super.getSlickOptions();
        opt.enableTextSelectionOnCells = true;
        // the selection model marks the cells of the rows it selects with the default css class of the
        // grid, "selected", which the themes style; the class this used to set had no style of its own,
        // so a selected row looked like an active cell and nothing else
        opt.enableCellNavigation = true;
        return opt;
    }

    protected override initSleekGrid() {
        super.initSleekGrid();
        
        this.sleekGrid.setSelectionModel(new RowSelectionModel());
    }
}
