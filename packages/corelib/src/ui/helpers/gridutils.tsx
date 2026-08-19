import { addDisposingListener } from "@serenity-is/domwise";
import { RowMoveManager, type ISleekGrid } from "@serenity-is/sleekgrid";
import { EntityGridTexts, SaveRequest, isArrayLike, serviceRequest } from "../../base";
import { IRemoteView } from "../../slick";
import { IDataGrid } from "../datagrid/idatagrid";
import { QuickSearchField, QuickSearchInput, type QuickSearchArgs } from "../datagrid/quicksearchinput";

/**
 * Utility functions for working with data grids.
 */
export namespace GridUtils {
    /**
     * Adds a toggle button to a toolbar.
     * @param toolDiv - The toolbar element (or array-like of elements).
     * @param cssClass - The CSS class to add to the button.
     * @param callback - Callback invoked with the new pressed state when toggled.
     * @param hint - The tooltip text for the button.
     * @param initial - Optional initial pressed state.
     */
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

    /**
     * Adds an "include deleted" toggle button that sets the IncludeDeleted
     * parameter on the view's submit requests.
     * @param toolDiv - The toolbar element (or array-like of elements).
     * @param view - The remote view.
     * @param hint - Optional tooltip text.
     * @param initial - Optional initial state.
     */
    export function addIncludeDeletedToggle(toolDiv: HTMLElement | ArrayLike<HTMLElement>,
        view: IRemoteView<any>, hint?: string, initial?: boolean): void {

        var includeDeleted = initial ?? false;
        var oldSubmit = view.onSubmit;
        view.onSubmit = function (v) {
            v.params.IncludeDeleted = includeDeleted;
            if (oldSubmit != null) {
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

        addDisposingListener(isArrayLike(toolDiv) ? toolDiv[0] : toolDiv, function () {
            view.onSubmit = null;
            oldSubmit = null;
        });
    }

    /**
     * Adds a quick search input to a container and wires it to the given view.
     * @param options - The quick search options.
     * @returns The created QuickSearchInput widget.
     */
    export function addQuickSearch({ container, fields, beforeSearch, search, view }: {
        container: HTMLElement | ArrayLike<HTMLElement>,
        fields?: QuickSearchField[],
        beforeSearch?: (args: QuickSearchArgs) => void,
        search?: (args: QuickSearchArgs) => void,
        view?: IRemoteView<any>,
    }): QuickSearchInput {

        const el = <input type="text" /> as HTMLInputElement;
        container = isArrayLike(container) ? container[0] : container;
        container.prepend(<div class={["s-QuickSearchBar", fields?.length && "has-quick-search-fields"]}>{el}</div>)

        let lastDoneEvent: ((found: boolean) => void) = null;
        const input = new QuickSearchInput({
            element: el,
            fields,
            beforeSearch,
            search: search ?? ((args) => {
                lastDoneEvent = args.done;
                if (view) {
                    view.seekToPage = 1;
                    view.populate();
                }
            })
        });

        if (view) {
            let oldSubmit = view.onSubmit;
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

            const onDataLoaded = function () {
                if (lastDoneEvent != null) {
                    lastDoneEvent(view.getLength() > 0);
                    lastDoneEvent = null;
                }
            }

            view.onDataLoaded.subscribe(onDataLoaded);


            addDisposingListener(el, function () {
                view?.onDataLoaded?.unsubscribe(onDataLoaded);
                if (view && oldSubmit) {
                    view.onSubmit = oldSubmit;
                    view = null;
                    oldSubmit = null;
                }
            });
        }

        return input;
    }


    /** @deprecated use addQuickSearch with named args
     * Adds a quick search input to a toolbar.
     * @param toolDiv - The toolbar element (or array-like of elements).
     * @param view - The remote view.
     * @param fields - Optional quick search fields.
     * @param onChange - Optional callback invoked before searching.
     * @returns The created QuickSearchInput widget.
     */
    export function addQuickSearchInput(toolDiv: HTMLElement | ArrayLike<HTMLElement>,
        view: IRemoteView<any>, fields?: QuickSearchField[], onChange?: () => void): QuickSearchInput {
        return addQuickSearch({
            container: toolDiv,
            fields: fields,
            view,
            beforeSearch: onChange,
            search: () => {
            }
        });
    }

    /** @deprecated use addQuickSearch with named args
     * Adds a quick search input with a custom search handler.
     * @param container - The container element (or array-like of elements).
     * @param search - The custom search handler.
     * @param fields - Optional quick search fields.
     * @returns The created QuickSearchInput widget.
     */
    export function addQuickSearchInputCustom(container: HTMLElement | ArrayLike<HTMLElement>,
        search: (field: QuickSearchArgs["field"], query: QuickSearchArgs["query"], done: QuickSearchArgs["done"]) => void,
        fields?: QuickSearchField[]): QuickSearchInput {

        return addQuickSearch({
            container: container,
            fields: fields,
            search: (args) => search(args.field, args.query, args.done)
        });
    }

    /**
     * Makes the rows of a grid reorderable by dragging.
     * @param grid - The sleek grid.
     * @param handleMove - Callback invoked with the moved row indexes and the insert-before index.
     */
    export function makeOrderable(grid: ISleekGrid,
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

    /**
     * Makes the rows of a data grid reorderable and persists the new order by
     * sending update requests to the server.
     * @typeParam TItem - The type of the row items.
     * @typeParam TId - The type of the item id.
     * @param dataGrid - The data grid.
     * @param getId - A function that returns the id of an item.
     * @param getDisplayOrder - A function that returns the display order of an item.
     * @param service - The service URL to send update requests to.
     * @param getUpdateRequest - A function that builds an update request for an item id and order.
     */
    export function makeOrderableWithUpdateRequest<TItem = any, TId = any>(dataGrid: IDataGrid,
        getId: (item: TItem) => TId, getDisplayOrder: (item: TItem) => any, service: string,
        getUpdateRequest: (id: TId, order: number) => SaveRequest<TItem>): void {

        makeOrderable(dataGrid.getGrid(), function (rows, insertBefore) {
            if (rows.length === 0) {
                return;
            }

            var order: number;
            var index = insertBefore;
            if (index < 0) {
                order = 1;
            }
            else if (insertBefore >= dataGrid.getGrid().getDataLength()) {
                order = (getDisplayOrder(dataGrid.getGrid().getDataItem(dataGrid.getGrid().getDataLength() - 1)) ?? 0);
                if (order === 0) {
                    order = insertBefore + 1;
                }
                else {
                    order = order + 1;
                }
            }
            else {
                order = (getDisplayOrder(dataGrid.getGrid().getDataItem(insertBefore)) ?? 0);
                if (order === 0) {
                    order = insertBefore + 1;
                }
            }

            var i = 0;
            var next: any = null;
            next = function () {
                serviceRequest(service, getUpdateRequest(getId(dataGrid.getGrid().getDataItem(rows[i])), order++),
                    () => {
                        i++;
                        if (i < rows.length) {
                            next();
                        }
                        else {
                            dataGrid.getView().populate();
                        }
                    });
            };
            next();
        });
    }
}
