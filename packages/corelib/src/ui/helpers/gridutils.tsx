import { Grid, RowMoveManager } from "@serenity-is/sleekgrid";
import { EntityGridTexts, Fluent, SaveRequest, isArrayLike, serviceRequest } from "../../base";
import { IRemoteView } from "../../slick";
import { IDataGrid } from "../datagrid/idatagrid";
import { QuickSearchField, QuickSearchInput } from "../datagrid/quicksearchinput";

export namespace GridUtils {
    export function addToggleButton(toolDiv: HTMLElement | ArrayLike<HTMLElement>, cssClass: string,
        callback: (p1: boolean) => void, hint: string, initial?: boolean): void {

        const btn = <div class={['s-ToggleButton', cssClass, initial && "pressed"]}>
            <a href="#" title={hint ?? ''} onClick={e => {
                e.preventDefault();
                btn.classList.toggle('pressed');
                const pressed = btn.classList.contains('pressed');
                callback && callback(pressed);
            }} />
        </div> as HTMLDivElement;

        (isArrayLike(toolDiv) ? toolDiv[0] : toolDiv).prepend(btn);
    }

    export function addIncludeDeletedToggle(toolDiv: HTMLElement | ArrayLike<HTMLElement>,
        view: IRemoteView<any>, hint?: string, initial?: boolean): void {

        var includeDeleted = false;
        var oldSubmit = view.onSubmit;
        view.onSubmit = function (v) {
            v.params.IncludeDeleted = includeDeleted;
            if (oldSubmit != null) {
                ""
                return oldSubmit(v);
            }
            return true;
        };

        if (hint == null)
            hint = EntityGridTexts.IncludeDeletedToggle;

        addToggleButton(toolDiv, 's-IncludeDeletedToggle',
            function (pressed) {
                includeDeleted = pressed;
                view.seekToPage = 1;
                view.populate();
            }, hint, initial);

        Fluent.one(isArrayLike(toolDiv) ? toolDiv[0] : toolDiv, "disposing", function () {
            view.onSubmit = null;
            oldSubmit = null;
        });
    }

    export function addQuickSearchInput(toolDiv: HTMLElement | ArrayLike<HTMLElement>,
        view: IRemoteView<any>, fields?: QuickSearchField[], onChange?: () => void): QuickSearchInput {

        var oldSubmit = view.onSubmit;
        var input: QuickSearchInput;
        view.onSubmit = function (v) {
            if (input) {
                var searchText = input.get_value();
                if (searchText && searchText.length > 0) {
                    v.params.ContainsText = searchText;
                }
                else {
                    delete v.params['ContainsText'];
                }
                var searchField = input.get_field()?.name;
                if (searchField != null && searchField.length > 0) {
                    v.params.ContainsField = searchField;
                }
                else {
                    delete v.params['ContainsField'];
                }
            }

            if (oldSubmit != null)
                return oldSubmit(v);

            return true;
        };

        var lastDoneEvent: any = null;
        input = addQuickSearchInputCustom(toolDiv, (field, query, done) => {
            onChange && onChange();
            view.seekToPage = 1;
            lastDoneEvent = done;
            view.populate();
        }, fields);

        view.onDataLoaded.subscribe(function (e, ui) {
            if (lastDoneEvent != null) {
                lastDoneEvent(view.getLength() > 0);
                lastDoneEvent = null;
            }
        });

        return input;
    }

    export function addQuickSearchInputCustom(container: HTMLElement | ArrayLike<HTMLElement>,
        onSearch: (p1: string, p2: string, done: (p3: boolean) => void) => void,
        fields?: QuickSearchField[]): QuickSearchInput {

        const input = <input type="text" /> as HTMLInputElement;
        (isArrayLike(container) ? container[0] : container).prepend(<div class={["s-QuickSearchBar", fields?.length && "has-quick-search-files"]}>{input}</div>)

        return new QuickSearchInput({
            element: input,
            fields: fields,
            onSearch: onSearch as any
        });
    }

    export function makeOrderable(grid: Grid,
        handleMove: (rows: number[], insertBefore: number) => void): void {

        var moveRowsPlugin = new RowMoveManager({ cancelEditOnDrag: true });
        moveRowsPlugin.onBeforeMoveRows.subscribe(function (e, data) {
            for (var i = 0; !!(i < data.rows.length); i++) {
                if (!!(data.rows[i] === data.insertBefore ||
                    data.rows[i] === data.insertBefore - 1)) {
                    e.stopPropagation();
                    return false;
                }
            }

            return true;
        });

        moveRowsPlugin.onMoveRows.subscribe(function (_, data) {
            handleMove(data.rows, data.insertBefore);
            try {
                grid.setSelectedRows([]);
            }
            catch {
            }
        });
        grid.registerPlugin(moveRowsPlugin);
    }

    export function makeOrderableWithUpdateRequest<TItem = any, TId = any>(grid: IDataGrid,
        getId: (item: TItem) => TId, getDisplayOrder: (item: TItem) => any, service: string,
        getUpdateRequest: (id: TId, order: number) => SaveRequest<TItem>): void {

        makeOrderable(grid.getGrid(), function (rows, insertBefore) {
            if (rows.length === 0) {
                return;
            }

            var order: number;
            var index = insertBefore;
            if (index < 0) {
                order = 1;
            }
            else if (insertBefore >= grid.getGrid().getDataLength()) {
                order = (getDisplayOrder(grid.getGrid().getDataItem(grid.getGrid().getDataLength() - 1)) ?? 0);
                if (order === 0) {
                    order = insertBefore + 1;
                }
                else {
                    order = order + 1;
                }
            }
            else {
                order = (getDisplayOrder(grid.getGrid().getDataItem(insertBefore)) ?? 0);
                if (order === 0) {
                    order = insertBefore + 1;
                }
            }

            var i = 0;
            var next: any = null;
            next = function () {
                serviceRequest(service, getUpdateRequest(getId(grid.getGrid().getDataItem(rows[i])), order++),
                    () => {
                        i++;
                        if (i < rows.length) {
                            next();
                        }
                        else {
                            grid.getView().populate();
                        }
                    });
            };
            next();
        });
    }
}
