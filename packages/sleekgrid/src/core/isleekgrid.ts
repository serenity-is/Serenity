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

export interface ISleekGrid<TItem = any> extends CellNavigation, EditorHost, GridPluginHost {
    readonly onActiveCellChanged: EventEmitter<ArgsCell>;
    readonly onActiveCellPositionChanged: EventEmitter<ArgsGrid>;
    readonly onAddNewRow: EventEmitter<ArgsAddNewRow>;
    readonly onAfterInit: EventEmitter<ArgsGrid>;
    readonly onBeforeCellEditorDestroy: EventEmitter<ArgsEditorDestroy>;
    readonly onBeforeDestroy: EventEmitter<ArgsGrid>;
    readonly onBeforeEditCell: EventEmitter<ArgsCellEdit>;
    readonly onBeforeFooterRowCellDestroy: EventEmitter<ArgsColumnNode>;
    readonly onBeforeHeaderCellDestroy: EventEmitter<ArgsColumnNode>;
    readonly onBeforeHeaderRowCellDestroy: EventEmitter<ArgsColumnNode>;
    readonly onCellChange: EventEmitter<ArgsCellChange>;
    readonly onCellCssStylesChanged: EventEmitter<ArgsCssStyle>;
    readonly onClick: EventEmitter<ArgsCell, MouseEvent>;
    readonly onColumnsReordered: EventEmitter<ArgsGrid>;
    readonly onColumnsResized: EventEmitter<ArgsGrid>;
    readonly onContextMenu: EventEmitter<ArgsGrid, UIEvent>;
    readonly onDblClick: EventEmitter<ArgsCell, MouseEvent>;
    readonly onDrag: EventEmitter<ArgsDrag, UIEvent>;
    readonly onDragEnd: EventEmitter<ArgsDrag, UIEvent>;
    readonly onDragInit: EventEmitter<ArgsDrag, UIEvent>;
    readonly onDragStart: EventEmitter<ArgsDrag, UIEvent>;
    readonly onFooterRowCellRendered: EventEmitter<ArgsColumnNode>;
    readonly onHeaderCellRendered: EventEmitter<ArgsColumnNode>;
    readonly onHeaderClick: EventEmitter<ArgsColumn, MouseEvent>;
    readonly onHeaderContextMenu: EventEmitter<ArgsColumn, MouseEvent>;
    readonly onHeaderMouseEnter: EventEmitter<ArgsColumn, MouseEvent>;
    readonly onHeaderMouseLeave: EventEmitter<ArgsColumn, MouseEvent>;
    readonly onHeaderRowCellRendered: EventEmitter<ArgsColumnNode>;
    readonly onKeyDown: EventEmitter<ArgsCell, KeyboardEvent>;
    readonly onMouseEnter: EventEmitter<ArgsGrid, MouseEvent>;
    readonly onMouseLeave: EventEmitter<ArgsGrid, MouseEvent>;
    readonly onScroll: EventEmitter<ArgsScroll>;
    readonly onSelectedRowsChanged: EventEmitter<ArgsSelectedRowsChange>;
    readonly onSort: EventEmitter<ArgsSort>;
    readonly onValidationError: EventEmitter<ArgsValidationError>;
    readonly onViewportChanged: EventEmitter<ArgsGrid>;
    init(): void;
    addCellCssStyles(key: string, hash: CellStylesHash): void;
    autosizeColumns(): void;
    canCellBeActive(row: number, cell: number): boolean;
    canCellBeSelected(row: number, cell: number): boolean;
    clearTextSelection(): void;
    columnsResized(invalidate?: boolean): void;
    commitCurrentEdit(): boolean;
    destroy(): void;
    editActiveCell(editor?: EditorClass): void;
    flashCell(row: number, cell: number, speed?: number): void;
    focus(): void;
    getAbsoluteColumnMinWidth(): number;
    getActiveCanvasNode(e?: { target: EventTarget }): HTMLElement;
    getActiveCellNode(): HTMLElement;
    getActiveViewportNode(e?: { target: EventTarget }): HTMLElement;
    /** Returns all columns in the grid, including hidden ones, the order might not match visible columns due to pinning, ordering etc. */
    getAllColumns(): Column<TItem>[];
    getCanvases(): any | HTMLElement[];
    getCanvasNode(row?: number, cell?: number): HTMLElement;
    getCellCssStyles(key: string): CellStylesHash;
    getCellEditor(): Editor;
    getCellFromEvent(e: any): { row: number; cell: number; };
    getCellFromNode(cellNode: Element): number;
    getCellFromPoint(x: number, y: number): { row: number; cell: number; };
    getCellNode(row: number, cell: number): HTMLElement;
    getCellNodeBox(row: number, cell: number): { top: number; right: number; bottom: number; left: number; };
    getColspan(row: number, cell: number): number;
    /** Gets a column by its ID. May also return hidden columns. */
    getColumnById(id: string): Column<TItem>;
    getColumnFromNode(cellNode: Element): Column<TItem>;
    /** Returns a column's index in the visible columns list by its column ID. If opt.inAll is true, it will return index in all columns. */
    getColumnIndex(id: string, opt?: { inAll?: boolean }): number;
    /** Returns only the visible columns in order */
    getColumns(): Column<TItem>[];
    getContainerNode(): HTMLElement;
    getData(): any;
    getDataItem(row: number): TItem;
    getDataItemValueForColumn(item: TItem, columnDef: Column<TItem>): any;
    getDataLength(): number;
    getDisplayedScrollbarDimensions(): { width: number; height: number; };
    getEditController(): EditController;
    getEditorLock(): EditorLock;
    getFooterRow(): HTMLElement;
    getFooterRowColumn(columnIdOrIdx: string | number): HTMLElement;
    getFormatter(row: number, column: Column<TItem>): ColumnFormat<TItem>;
    getFormatterContext(row: number, cell: number): FormatterContext;
    getGridPosition(): Position;
    getGroupingPanel(): HTMLElement;
    getHeader(): HTMLElement;
    getHeaderColumn(columnIdOrIdx: string | number): HTMLElement;
    getHeaderRow(): HTMLElement;
    getHeaderRowColumn(columnIdOrIdx: string | number): HTMLElement;
    getLayoutInfo(): GridLayoutInfo;
    getOptions(): GridOptions<TItem>;
    getPreHeaderPanel(): HTMLElement;
    getRenderedRange(viewportTop?: number, viewportLeft?: number): ViewRange;
    getRowFromNode(rowNode: Element): number;
    getScrollBarDimensions(): { width: number; height: number; };
    getSelectedRows(): number[];
    getSelectionModel(): SelectionModel;
    getSortColumns(): ColumnSort[];
    getTopPanel(): HTMLElement;
    getTotalsFormatter(column: Column<TItem>): ColumnFormat<TItem>;
    getUID(): string;
    /** Gets the viewport range */
    getViewport(viewportTop?: number, viewportLeft?: number): ViewRange;
    getViewportNode(row?: number, cell?: number): HTMLElement;
    getVisibleRange(viewportTop?: number, viewportLeft?: number): ViewRange;
    gotoCell(row: number, cell: number, forceEdit?: boolean): void;
    invalidate(): void;
    invalidateAllRows(): void;
    /**
     * Invalidates various elements after properties of columns have changed.
     * Call this if you change columns properties that don't require a full setColumns call (e.g. width, name, visible etc.)
     */
    invalidateColumns(): void;
    invalidateRow(row: number): void;
    invalidateRows(rows: number[]): void;
    removeCellCssStyles(key: string): void;
    render: () => void;
    /**
     * Reorders columns based on their IDs and notifies onColumnsReordered by default.
     * @param columnIds
     * @param opt Whether to notify onColumnsReordered (default true). If setVisible is provided, it will also set visibility based on that.
     * This function is used by column picker and other plugins to reorder columns and set visibility in one shot.
     */
    reorderColumns(columnIds: string[], opt?: { notify?: boolean, setVisible?: string[] }): void;
    resetActiveCell(): void;
    resizeCanvas: () => void;
    scrollActiveCellIntoView(): void;
    scrollCellIntoView(row: number, cell: number, doPaging?: boolean): void;
    scrollColumnIntoView(cell: number): void;
    scrollRowIntoView(row: number, doPaging?: boolean): void;
    scrollRowToTop(row: number): void;
    setActiveCell(row: number, cell: number): void;
    setActiveRow(row: number, cell: number, suppressScrollIntoView?: boolean): void;
    setCellCssStyles(key: string, hash: CellStylesHash): void;
    setColumnHeaderVisibility(visible: boolean): void;
    setColumns(columns: Column<TItem>[]): void;
    /**
     * Sets the visible columns based on their IDs and reorders them to provided order
     * unless specified otherwise.
     * @param columnIds The IDs of the columns to be made visible.
     * @param opt Whether to reorder the visible columns based on the provided IDs (default true),
     * and notify onColumnsReordered (default true).
     */
    setVisibleColumns(columnIds: string[], opt?: { reorder?: boolean, notify?: boolean }): void;
    setData(newData: any, scrollToTop?: boolean): void;
    setFooterRowVisibility(visible: boolean): void;
    setGroupingPanelVisibility(visible: boolean): void;
    setHeaderRowVisibility(visible: boolean): void;
    setOptions(args: GridOptions<TItem>, suppressRender?: boolean, suppressColumnSet?: boolean, suppressSetOverflow?: boolean): void;
    setPreHeaderPanelVisibility(visible: boolean): void;
    setSelectedRows(rows: number[]): void;
    setSelectionModel(model: SelectionModel): void;
    setSortColumn(columnId: string, ascending: boolean): void;
    setSortColumns(cols: ColumnSort[]): void;
    setTopPanelVisibility(visible: boolean): void;
    updateCell(row: number, cell: number): void;
    updateColumnHeader(columnId: string, title?: string | ColumnFormat<any>, toolTip?: string): void;
    updatePagingStatusFromView(pagingInfo: { pageSize: number; pageNum: number; totalPages: number; }): void;
    updateRow(row: number): void;
    updateRowCount(): void;
}

export type GridLayoutInfo = {
    frozenTopRows: number;
    frozenBottomRows: number;
    pinnedStartCols: number;
    pinnedEndCols: number;
    supportFrozenRows: boolean
    supportFrozenBottom: boolean;
    supportPinnedCols: boolean;
    supportPinnedEnd: boolean;
};
