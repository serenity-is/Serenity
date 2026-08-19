import type { CellNavigation } from "./cellnavigation";
import type { Column, ColumnSort } from "./column";
import type { EditController, Editor, EditorClass, EditorHost, EditorLock, Position } from "./editing";
import type { EventEmitter } from "./event";
import type { ArgsAddNewRow, ArgsCell, ArgsCellChange, ArgsCellEdit, ArgsColumn, ArgsColumnNode, ArgsCssStyle, ArgsDrag, ArgsEditorDestroy, ArgsGrid, ArgsScroll, ArgsSelectedRowsChange, ArgsSort, ArgsValidationError } from "./eventargs";
import type { CellStylesHash, ColumnFormat, FormatterContext } from "./formatting";
import type { GridPluginHost } from "./grid-plugin";
import type { GridOptions } from "./gridoptions";
import type { SelectionModel } from "./selection-model";
import type { ViewRange } from "./viewrange";

/**
 * Full grid surface exposed to plugins, editors and external code.
 * @template TItem - Row item type.
 */
export interface ISleekGrid<TItem = any> extends CellNavigation, EditorHost, GridPluginHost {
    /** Fired when the active cell changes. {@link ArgsCell} payload. */
    readonly onActiveCellChanged: EventEmitter<ArgsCell>;
    /** Fired when the active cell's pixel position changes (e.g. after scrolling). {@link ArgsGrid} payload. */
    readonly onActiveCellPositionChanged: EventEmitter<ArgsGrid>;
    /** Fired when a new row is about to be added via the add-new-row row. {@link ArgsAddNewRow} payload. */
    readonly onAddNewRow: EventEmitter<ArgsAddNewRow>;
    /** Fired once after `init()` completes. {@link ArgsGrid} payload. */
    readonly onAfterInit: EventEmitter<ArgsGrid>;
    /** Fired before a cell editor is destroyed; allows handlers to intercept. {@link ArgsEditorDestroy} payload. */
    readonly onBeforeCellEditorDestroy: EventEmitter<ArgsEditorDestroy>;
    /** Fired before the grid is destroyed. {@link ArgsGrid} payload. */
    readonly onBeforeDestroy: EventEmitter<ArgsGrid>;
    /** Fired before a cell enters edit mode; cancel with `e.preventDefault()`. {@link ArgsCellEdit} payload. */
    readonly onBeforeEditCell: EventEmitter<ArgsCellEdit>;
    /** Fired before a footer row cell is destroyed. {@link ArgsColumnNode} payload. */
    readonly onBeforeFooterRowCellDestroy: EventEmitter<ArgsColumnNode>;
    /** Fired before a header cell is destroyed. {@link ArgsColumnNode} payload. */
    readonly onBeforeHeaderCellDestroy: EventEmitter<ArgsColumnNode>;
    /** Fired before a header-row (filter) cell is destroyed. {@link ArgsColumnNode} payload. */
    readonly onBeforeHeaderRowCellDestroy: EventEmitter<ArgsColumnNode>;
    /** Fired after a cell value has changed and been committed. {@link ArgsCellChange} payload. */
    readonly onCellChange: EventEmitter<ArgsCellChange>;
    /** Fired when per-cell CSS styles change. {@link ArgsCssStyle} payload. */
    readonly onCellCssStylesChanged: EventEmitter<ArgsCssStyle>;
    /** Click on a body cell. {@link ArgsCell}, native `MouseEvent`. */
    readonly onClick: EventEmitter<ArgsCell, MouseEvent>;
    /** Fired after columns are reordered. {@link ArgsGrid} payload. */
    readonly onColumnsReordered: EventEmitter<ArgsGrid>;
    /** Fired after columns are resized. {@link ArgsGrid} payload. */
    readonly onColumnsResized: EventEmitter<ArgsGrid>;
    /** Context-menu event on the grid canvas. {@link ArgsGrid}, native `UIEvent`. */
    readonly onContextMenu: EventEmitter<ArgsGrid, UIEvent>;
    /** Double-click on a body cell. {@link ArgsCell}, native `MouseEvent`. */
    readonly onDblClick: EventEmitter<ArgsCell, MouseEvent>;
    /** Ongoing drag within the grid. {@link ArgsDrag}, native `UIEvent`. */
    readonly onDrag: EventEmitter<ArgsDrag, UIEvent>;
    /** Drag finished. {@link ArgsDrag}, native `UIEvent`. */
    readonly onDragEnd: EventEmitter<ArgsDrag, UIEvent>;
    /** Drag initialized (mousedown on a draggable surface). {@link ArgsDrag}, native `UIEvent`. */
    readonly onDragInit: EventEmitter<ArgsDrag, UIEvent>;
    /** Drag started (after minimum movement threshold). {@link ArgsDrag}, native `UIEvent`. */
    readonly onDragStart: EventEmitter<ArgsDrag, UIEvent>;
    /** Fired after a footer row cell is rendered. {@link ArgsColumnNode} payload. */
    readonly onFooterRowCellRendered: EventEmitter<ArgsColumnNode>;
    /** Fired after a header cell is rendered. {@link ArgsColumnNode} payload. */
    readonly onHeaderCellRendered: EventEmitter<ArgsColumnNode>;
    /** Click on a header cell. {@link ArgsColumn}, native `MouseEvent`. */
    readonly onHeaderClick: EventEmitter<ArgsColumn, MouseEvent>;
    /** Context menu on a header cell. {@link ArgsColumn}, native `MouseEvent`. */
    readonly onHeaderContextMenu: EventEmitter<ArgsColumn, MouseEvent>;
    /** Pointer entered a header cell. {@link ArgsColumn}, native `MouseEvent`. */
    readonly onHeaderMouseEnter: EventEmitter<ArgsColumn, MouseEvent>;
    /** Pointer left a header cell. {@link ArgsColumn}, native `MouseEvent`. */
    readonly onHeaderMouseLeave: EventEmitter<ArgsColumn, MouseEvent>;
    /** Fired after a header-row (filter) cell is rendered. {@link ArgsColumnNode} payload. */
    readonly onHeaderRowCellRendered: EventEmitter<ArgsColumnNode>;
    /** Key down while a body cell is active. {@link ArgsCell}, native `KeyboardEvent`. */
    readonly onKeyDown: EventEmitter<ArgsCell, KeyboardEvent>;
    /** Pointer entered the grid. {@link ArgsGrid}, native `MouseEvent`. */
    readonly onMouseEnter: EventEmitter<ArgsGrid, MouseEvent>;
    /** Pointer left the grid. {@link ArgsGrid}, native `MouseEvent`. */
    readonly onMouseLeave: EventEmitter<ArgsGrid, MouseEvent>;
    /** Grid scrolled. {@link ArgsScroll} payload. */
    readonly onScroll: EventEmitter<ArgsScroll>;
    /** Selected rows changed. {@link ArgsSelectedRowsChange} payload. */
    readonly onSelectedRowsChanged: EventEmitter<ArgsSelectedRowsChange>;
    /** Sorted columns changed. {@link ArgsSort} payload. */
    readonly onSort: EventEmitter<ArgsSort>;
    /** Editor validation failed. {@link ArgsValidationError} payload. */
    readonly onValidationError: EventEmitter<ArgsValidationError>;
    /** Visible viewport changed (scroll or resize). {@link ArgsGrid} payload. */
    readonly onViewportChanged: EventEmitter<ArgsGrid>;
    /**
     * Initializes the grid DOM, binds events and performs the first render.
     * Called automatically on construction unless `explicitInitialization` is set.
     */
    init(): void;
    /**
     * Adds per-cell CSS styles under the given key; multiple callers can coexist via different keys.
     * @param key - Bucket name to group styles so callers can later remove only their own styles.
     * @param hash - Map of `row -> columnId -> cssClass`.
     */
    addCellCssStyles(key: string, hash: CellStylesHash): void;
    /**
     * Auto-sizes columns to fit the container width, honouring `minWidth`/`maxWidth` and `forceFitColumns`.
     */
    autosizeColumns(): void;
    /**
     * Cancels the current cell edit without saving.
     * @returns `true` if cancelled (or no edit was active).
     */
    cancelCurrentEdit(): boolean;
    /**
     * Checks whether a cell can become the active cell (focusable and selectable).
     * @param row - Row index.
     * @param cell - Cell/column index.
     * @param tab - Whether the check is for tab navigation (affects `tabbable` handling).
     * @returns `true` if the cell may become active.
     */
    canCellBeActive(row: number, cell: number, tab?: boolean): boolean;
    /**
     * Checks whether a cell may be selected.
     * @param row - Row index.
     * @param cell - Cell/column index.
     * @returns `true` if the cell is selectable.
     */
    canCellBeSelected(row: number, cell: number): boolean;
    /** Clears any browser text selection within the grid. */
    clearTextSelection(): void;
    /**
     * Notifies the grid that column sizes have changed externally.
     * @param invalidate - Whether to invalidate and re-render visible rows (default `true` behaviour).
     */
    columnsResized(invalidate?: boolean): void;
    /**
     * Commits the current edit, running validation.
     * @param opt - Options; set `{ forceValueChange: true }` to force `onCellChange` even when the value did not appear to change.
     * @returns `true` if committed (or no edit was active), `false` if validation failed.
     */
    commitCurrentEdit(opt?: { forceValueChange?: boolean }): boolean;
    /** Destroys the grid, removing DOM and event listeners. */
    destroy(): void;
    /**
     * Activates the editor on the currently active cell.
     * @param editor - Optional editor class override; defaults to the column's editor.
     */
    editActiveCell(editor?: EditorClass): void;
    /**
     * Flashes a cell briefly for visual feedback (e.g. successful update).
     * @param row - Row index.
     * @param cell - Cell/column index.
     * @param speed - Flash duration in milliseconds; defaults to grid's configured speed.
     */
    flashCell(row: number, cell: number, speed?: number): void;
    /**
     * Focuses the grid's viewport so keyboard navigation works.
     */
    focus(): void;
    /** Returns the minimum allowed column width, considering absolute minima. */
    getAbsoluteColumnMinWidth(): number;
    /**
     * Gets the scrollable canvas node that is active for the given event target.
     * @param e - Optional event target hint for viewport disambiguation.
     */
    getActiveCanvasNode(e?: { target: EventTarget }): HTMLElement;
    /** Gets the DOM node for the currently active cell, if any. */
    getActiveCellNode(): HTMLElement;
    /**
     * Gets the viewport node that is active for the given event target.
     * @param e - Optional event target hint for viewport disambiguation.
     */
    getActiveViewportNode(e?: { target: EventTarget }): HTMLElement;
    /** Returns all columns in the grid, including hidden ones; order may differ from visible columns due to pinning/reordering. */
    getAllColumns(): Column<TItem>[];
    /** Returns the grid's canvas elements (one per viewport when frozen rows/cols are used). */
    getCanvases(): any | HTMLElement[];
    /**
     * Gets the canvas element for the given row/cell viewport.
     * @param row - Optional row hint for viewport selection.
     * @param cell - Optional cell hint for viewport selection.
     */
    getCanvasNode(row?: number, cell?: number): HTMLElement;
    /**
     * Gets the cell CSS styles hash associated with the given key.
     * @param key - Style bucket name.
     * @returns Hash of `row -> columnId -> cssClass`.
     */
    getCellCssStyles(key: string): CellStylesHash;
    /** Returns the currently active editor instance, if any. */
    getCellEditor(): Editor;
    /**
     * Resolves a row/cell coordinate from a mouse or keyboard event.
     * @param e - Native event with target coordinates.
     * @returns Row/cell indexes.
     */
    getCellFromEvent(e: any): { row: number; cell: number; };
    /**
     * Resolves the cell/column index from a cell DOM node.
     * @param cellNode - Cell element.
     */
    getCellFromNode(cellNode: Element): number;
    /**
     * Resolves a row/cell from pixel coordinates relative to the canvas.
     * @param x - Horizontal pixel offset.
     * @param y - Vertical pixel offset.
     * @returns Row/cell indexes.
     */
    getCellFromPoint(x: number, y: number): { row: number; cell: number; };
    /**
     * Gets the DOM node for a specific cell.
     * @param row - Row index.
     * @param cell - Cell/column index.
     * @returns Cell element, or `null` when not rendered.
     */
    getCellNode(row: number, cell: number): HTMLElement;
    /**
     * Gets the bounding rectangle for a specific cell.
     * @param row - Row index.
     * @param cell - Cell/column index.
     * @returns Box with `top`, `right`, `bottom`, `left` in pixels.
     */
    getCellNodeBox(row: number, cell: number): { top: number; right: number; bottom: number; left: number; };
    /**
     * Gets the column span for the cell at the given row/col, taking `colspan` metadata into account.
     * @param row - Row index.
     * @param cell - Cell/column index.
     * @returns Number of columns spanned (at least `1`).
     */
    getColspan(row: number, cell: number): number;
    /**
     * Gets a column by its id; may return hidden columns.
     * @param id - Column id.
     * @returns Matching column definition or `undefined`.
     */
    getColumnById(id: string): Column<TItem>;
    /**
     * Resolves the column definition from a cell DOM node.
     * @param cellNode - Cell element.
     * @returns Corresponding column definition.
     */
    getColumnFromNode(cellNode: Element): Column<TItem>;
    /**
     * Gets the index of a column by its id.
     * @param id - Column id.
     * @param opt - When `opt.inAll` is `true`, searches all columns; otherwise only visible columns.
     * @returns Column index or `-1` when not found.
     */
    getColumnIndex(id: string, opt?: { inAll?: boolean }): number;
    /** Returns only the visible columns in display order. */
    getColumns(): Column<TItem>[];
    /** Returns the root container element of the grid. */
    getContainerNode(): HTMLElement;
    /** Returns the data source / `DataView` attached to the grid. */
    getData(): any;
    /**
     * Returns the data item at the given view row.
     * @param row - View row index.
     * @returns Data item for that row (or `Group`/`GroupTotals` for group rows).
     */
    getDataItem(row: number): TItem;
    /**
     * Extracts the raw cell value for a given column and item.
     * @param item - Row data item.
     * @param columnDef - Column definition.
     * @returns Cell value.
     */
    getDataItemValueForColumn(item: TItem, columnDef: Column<TItem>): any;
    /** Returns the number of rows in the grid's data source/view. */
    getDataLength(): number;
    /** Returns the currently displayed scrollbar dimensions (accounts for auto-hiding etc.). */
    getDisplayedScrollbarDimensions(): { width: number; height: number; };
    /** Returns the edit controller that manages the active editor lock. */
    getEditController(): EditController;
    /** Returns the `EditorLock` instance controlling concurrent edits. */
    getEditorLock(): EditorLock;
    /** Returns the footer row container element. */
    getFooterRow(): HTMLElement;
    /**
     * Returns the footer row cell element for the given column.
     * @param columnIdOrIdx - Column id or visible index.
     */
    getFooterRowColumn(columnIdOrIdx: string | number): HTMLElement;
    /**
     * Resolves the formatter to use for a body cell, considering column, row metadata and factory.
     * @param row - Row index.
     * @param column - Column definition.
     * @returns Formatter function for that cell.
     */
    getFormatter(row: number, column: Column<TItem>): ColumnFormat<TItem>;
    /**
     * Creates a formatter context for a body cell.
     * @param row - Row index.
     * @param cell - Cell/column index.
     * @returns Populated {@link FormatterContext}.
     */
    getFormatterContext(row: number, cell: number): FormatterContext;
    /** Returns the grid container's bounding position (as used for editor placement). */
    getGridPosition(): Position;
    /** Returns the grouping panel container, if enabled. */
    getGroupingPanel(): HTMLElement;
    /** Returns the header row container element. */
    getHeader(): HTMLElement;
    /**
     * Returns the header cell element for the given column.
     * @param columnIdOrIdx - Column id or visible index.
     */
    getHeaderColumn(columnIdOrIdx: string | number): HTMLElement;
    /** Returns the header-row (filter row) container element. */
    getHeaderRow(): HTMLElement;
    /**
     * Returns the header-row cell element for the given column.
     * @param columnIdOrIdx - Column id or visible index.
     */
    getHeaderRowColumn(columnIdOrIdx: string | number): HTMLElement;
    /** Returns summarized layout/pinning information for the current grid configuration. */
    getLayoutInfo(): GridLayoutInfo;
    /** Returns the current grid options. */
    getOptions(): GridOptions<TItem>;
    /** Returns the pre-header panel element (grouping panel alternative). */
    getPreHeaderPanel(): HTMLElement;
    /**
     * Returns the currently rendered view range as managed by the render loop.
     * @param viewportTop - Optional scroll top override.
     * @param viewportLeft - Optional scroll left override.
     */
    getRenderedRange(viewportTop?: number, viewportLeft?: number): ViewRange;
    /**
     * Resolves the view row index from a row DOM node.
     * @param rowNode - Row element.
     */
    getRowFromNode(rowNode: Element): number;
    /** Returns the native scrollbar width/height for the grid, measured from the layout. */
    getScrollBarDimensions(): { width: number; height: number; };
    /** Returns the currently selected row indices. */
    getSelectedRows(): number[];
    /** Returns the active selection model plugin, if any. */
    getSelectionModel(): SelectionModel;
    /** Returns the active sort column descriptors. */
    getSortColumns(): ColumnSort[];
    /** Returns the top panel container element. */
    getTopPanel(): HTMLElement;
    /**
     * Resolves the group-totals formatter for a column.
     * @param column - Column to resolve a totals formatter for.
     * @returns Formatter for that column's totals row.
     */
    getTotalsFormatter(column: Column<TItem>): ColumnFormat<TItem>;
    /** Returns the unique identifier assigned to this grid instance. */
    getUID(): string;
    /**
     * Gets the viewport range for the active viewports.
     * @param viewportTop - Optional scroll top override.
     * @param viewportLeft - Optional scroll left override.
     */
    getViewport(viewportTop?: number, viewportLeft?: number): ViewRange;
    /**
     * Gets the viewport container node for the given row/cell.
     * @param row - Optional row hint for viewport selection.
     * @param cell - Optional cell hint for viewport selection.
     */
    getViewportNode(row?: number, cell?: number): HTMLElement;
    /**
     * Gets the visible (fully within viewport) row/cell range.
     * @param viewportTop - Optional scroll top override.
     * @param viewportLeft - Optional scroll left override.
     */
    getVisibleRange(viewportTop?: number, viewportLeft?: number): ViewRange;
    /**
     * Scrolls to and optionally edits the given cell.
     * @param row - Row index to go to.
     * @param cell - Cell/column index to go to.
     * @param forceEdit - Whether to immediately enter edit mode.
     */
    gotoCell(row: number, cell: number, forceEdit?: boolean): void;
    /** Invalidates the entire grid, requiring a full re-render on the next frame. */
    invalidate(): void;
    /** Invalidates all rows, forcing them to be re-rendered. */
    invalidateAllRows(): void;
    /**
     * Invalidates header/column chrome after column properties change without a full `setColumns()` call
     * (e.g. width, name, `visible` etc.). Forces header/footer re-rendering.
     */
    invalidateColumns(): void;
    /**
     * Invalidates a single row so it is re-rendered on the next frame.
     * @param row - View row index to invalidate.
     */
    invalidateRow(row: number): void;
    /**
     * Invalidates multiple rows so they are re-rendered on the next frame.
     * @param rows - View row indices to invalidate.
     */
    invalidateRows(rows: number[]): void;
    /**
     * Removes all cell CSS styles associated with the given key.
     * @param key - Style bucket name.
     */
    removeCellCssStyles(key: string): void;
    /**
     * Immediately renders the grid (row/cell DOM), synchronizing canvases and headers.
     * Usually called internally via `invalidate()` + animation frame; call manually after batch updates.
     */
    render: () => void;
    /**
     * Reorders columns based on their ids and optionally updates visibility.
     * @param columnIds - Ordered list of column ids to become the new visible order.
     * @param opt - When `opt.notify` is `false`, suppresses `onColumnsReordered`; when `opt.setVisible` is provided, visible columns are set to that list.
     */
    reorderColumns(columnIds: string[], opt?: { notify?: boolean, setVisible?: string[] }): void;
    /** Clears the active cell without scrolling. */
    resetActiveCell(): void;
    /**
     * Recalculates canvas/viewport sizes and re-renders headers and rows. Call after external
     * container resize when `autoHeight` is off.
     */
    resizeCanvas: () => void;
    /** Scrolls the viewport so the active cell is visible. */
    scrollActiveCellIntoView(): void;
    /**
     * Scrolls a specific cell into view.
     * @param row - Row index.
     * @param cell - Cell/column index.
     * @param doPaging - Whether to page the view when the row is far outside the viewport.
     */
    scrollCellIntoView(row: number, cell: number, doPaging?: boolean): void;
    /**
     * Scrolls a column into view without changing the active row.
     * @param cell - Visible column index to bring into view.
     */
    scrollColumnIntoView(cell: number): void;
    /**
     * Scrolls a row into view.
     * @param row - Row index.
     * @param doPaging - Whether to page the view when the row is far outside the viewport.
     */
    scrollRowIntoView(row: number, doPaging?: boolean): void;
    /**
     * Scrolls so that the given row is at the top of the viewport.
     * @param row - Row index to position at the top.
     */
    scrollRowToTop(row: number): void;
    /**
     * Sets the active cell, committing or cancelling any pending edit as needed.
     * @param row - Row index to activate.
     * @param cell - Cell/column index to activate.
     */
    setActiveCell(row: number, cell: number): void;
    /**
     * Sets the active row, optionally suppressing the automatic scroll into view.
     * @param row - Row index to become active.
     * @param cell - Cell/column index to become active.
     * @param suppressScrollIntoView - When `true`, the grid does not scroll to show the cell.
     */
    setActiveRow(row: number, cell: number, suppressScrollIntoView?: boolean): void;
    /**
     * Sets per-cell CSS styles under the given key, replacing any previous styles for that key.
     * @param key - Bucket name.
     * @param hash - Map of `row -> columnId -> cssClass`.
     */
    setCellCssStyles(key: string, hash: CellStylesHash): void;
    /**
     * Shows or hides the column header row.
     * @param visible - `true` to show, `false` to hide.
     */
    setColumnHeaderVisibility(visible: boolean): void;
    /**
     * Replaces the column set and re-renders headers/rows.
     * @param columns - New ordered list of column definitions.
     */
    setColumns(columns: Column<TItem>[]): void;
    /**
     * Sets the visible columns by id and optionally reorders them.
     * @param columnIds - Ids of columns to make visible, in desired order.
     * @param opt - When `opt.reorder` is `false`, current order is preserved; when `opt.notify` is `false`, `onColumnsReordered` is suppressed.
     */
    setVisibleColumns(columnIds: string[], opt?: { reorder?: boolean, notify?: boolean }): void;
    /**
     * Replaces the data source and refreshes the view.
     * @param newData - New data array or `DataView`-like object.
     * @param scrollToTop - Whether to scroll to the top after the replacement.
     */
    setData(newData: any, scrollToTop?: boolean): void;
    /**
     * Shows or hides the footer row.
     * @param visible - `true` to show, `false` to hide.
     */
    setFooterRowVisibility(visible: boolean): void;
    /**
     * Shows or hides the grouping panel.
     * @param visible - `true` to show, `false` to hide.
     */
    setGroupingPanelVisibility(visible: boolean): void;
    /**
     * Shows or hides the header row (filter row).
     * @param visible - `true` to show, `false` to hide.
     */
    setHeaderRowVisibility(visible: boolean): void;
    /**
     * Merges the given options into the current options and optionally re-renders.
     * @param args - Options to merge.
     * @param suppressRender - When `true`, no render is triggered.
     * @param suppressColumnSet - When `true`, columns are not re-set from `args.columns`.
     * @param suppressSetOverflow - When `true`, the canvas overflow recalculation is skipped.
     */
    setOptions(args: GridOptions<TItem>, suppressRender?: boolean, suppressColumnSet?: boolean, suppressSetOverflow?: boolean): void;
    /**
     * Shows or hides the pre-header panel (deprecated grouping-panel variant).
     * @param visible - `true` to show, `false` to hide.
     */
    setPreHeaderPanelVisibility(visible: boolean): void;
    /**
     * Selects the given rows (used by legacy row-selection integration).
     * @param rows - Row indices to select.
     */
    setSelectedRows(rows: number[]): void;
    /**
     * Attaches a selection-model plugin.
     * @param model - Selection model to activate.
     */
    setSelectionModel(model: SelectionModel): void;
    /**
     * Sets single-column sort state.
     * @param columnId - Column id to sort by.
     * @param ascending - `true` for ascending, `false` for descending.
     */
    setSortColumn(columnId: string, ascending: boolean): void;
    /**
     * Sets multi-column sort state.
     * @param cols - Array of sort descriptors.
     */
    setSortColumns(cols: ColumnSort[]): void;
    /**
     * Shows or hides the top panel.
     * @param visible - `true` to show, `false` to hide.
     */
    setTopPanelVisibility(visible: boolean): void;
    /**
     * Invalidates and re-renders a single cell.
     * @param row - Row index.
     * @param cell - Cell/column index.
     */
    updateCell(row: number, cell: number): void;
    /**
     * Updates a header cell's title/tooltip in place without a full column reset.
     * @param columnId - Column id whose header should be updated.
     * @param title - New title text or header formatter.
     * @param toolTip - New tooltip text.
     */
    updateColumnHeader(columnId: string, title?: string | ColumnFormat<any>, toolTip?: string): void;
    /**
     * Updates the grid's paging UI from a view/page change.
     * @param pagingInfo - Paging descriptor with `pageSize`, `pageNum` and `totalPages`.
     */
    updatePagingStatusFromView(pagingInfo: { pageSize: number; pageNum: number; totalPages: number; }): void;
    /**
     * Invalidates and re-renders an entire row.
     * @param row - View row index to update.
     */
    updateRow(row: number): void;
    /** Recalculates row count after the data view changes and re-renders as needed. */
    updateRowCount(): void;
}

/**
 * Summarized description of the grid's layout and pinning support/counters.
 */
export type GridLayoutInfo = {
    /** Number of rows frozen at the top. */
    frozenTopRows: number;
    /** Number of rows frozen at the bottom. */
    frozenBottomRows: number;
    /** Number of columns pinned at the start side. */
    pinnedStartCols: number;
    /** Number of columns pinned at the end side. */
    pinnedEndCols: number;
    /** Whether the current layout engine supports frozen rows. */
    supportFrozenRows: boolean
    /** Whether the layout engine supports bottom-frozen rows. */
    supportFrozenBottom: boolean;
    /** Whether the layout engine supports pinned columns. */
    supportPinnedCols: boolean;
    /** Whether the layout engine supports end-pinned columns. */
    supportPinnedEnd: boolean;
};
