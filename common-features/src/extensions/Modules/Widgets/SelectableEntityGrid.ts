import { Decorators, EntityGrid } from "@serenity-is/corelib";
import { Grid, GridOptions, RowSelectionModel } from "@serenity-is/sleekgrid";

@Decorators.registerClass("Serenity.Extensions.SelectableEntityGrid")
export class SelectableEntityGrid<TItem, TOptions> extends EntityGrid<TItem, TOptions> {

    protected getSlickOptions(): GridOptions {
        var opt = super.getSlickOptions();
        opt.enableTextSelectionOnCells = true;
        opt.selectedCellCssClass = "slick-row-selected";
        opt.enableCellNavigation = true;
        return opt;
    }

    protected createSlickGrid(): Grid {
        var grid = super.createSlickGrid();
        grid.setSelectionModel(new RowSelectionModel());
        return grid;
    }
}
