import { CheckTreeEditorTexts } from "../../base";
import { IDataGrid } from "../datagrid/idatagrid";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { getWidgetFrom } from "../widgets/widgetutils";

/**
 * Helper functions for managing a "select all" toolbar button on a data grid.
 */
export namespace GridSelectAllButtonHelper {
    /**
     * Updates the checked state of the select-all button based on whether all
     * items in the grid are selected.
     * @param grid - The data grid.
     * @param getSelected - A function that returns whether an item is selected.
     */
    export function update(grid: IDataGrid, getSelected: (p1: any) => boolean): void {
        var toolbar = grid.getElement().querySelector('.s-Toolbar');
        if (!toolbar) {
            return;
        }
        var btn = getWidgetFrom(toolbar, Toolbar).findButton('select-all-button');
        var items = grid.getView().getItems();
        btn.toggleClass('checked', items.length > 0 && !items.some(function (x) {
            return !getSelected(x);
        }));
    }

    /**
     * Defines a select-all toolbar button that selects or deselects all items.
     * @param getGrid - A function that returns the data grid.
     * @param getId - A function that returns the id of an item.
     * @param getSelected - A function that returns whether an item is selected.
     * @param setSelected - A function that sets the selected state of an item.
     * @param text - Optional button title text. Defaults to the "Select All" text.
     * @param onClick - Optional callback invoked after the selection is updated.
     * @returns The toolbar button definition.
     */
    export function define(getGrid: () => IDataGrid, getId: (p1: any) => any,
        getSelected: (p1: any) => boolean,
        setSelected: (p1: any, p2: boolean) => void,
        text?: string, onClick?: () => void): ToolButton {

        if (text == null) {
            text = CheckTreeEditorTexts.SelectAll ?? "Select All";
        }
        return {
            title: text,
            action: "select-all",
            cssClass: 'select-all-button',
            onClick: function (e: Event) {
                var grid = getGrid();
                var view = grid.getView();
                var btn = (e.target as HTMLElement).closest('.select-all-button');
                var makeSelected = !btn?.classList.contains('checked');
                view.beginUpdate();
                try {
                    for (var item of view.getItems()) {
                        setSelected(item, makeSelected);
                        view.updateItem(getId(item), item);
                    }
                    onClick && onClick();
                }
                finally {
                    view.endUpdate();
                }

                btn?.classList.toggle('checked', makeSelected);
            }
        };
    }
}
