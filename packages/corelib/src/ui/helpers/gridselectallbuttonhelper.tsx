import { CheckTreeEditorTexts } from "../../base";
import { IDataGrid } from "../datagrid/idatagrid";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { getWidgetFrom } from "../widgets/widgetutils";

export namespace GridSelectAllButtonHelper {
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
