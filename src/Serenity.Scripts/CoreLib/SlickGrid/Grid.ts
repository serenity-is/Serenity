import { Column } from "./Column";
import { GridOptions } from "./GridOptions";

export declare class Grid {
    constructor(container: JQuery, data: any, columns: Column[], options: GridOptions);
}

export interface ColumnSort {
    columnId?: string;
    sortAsc?: boolean;
}

export interface RowCell {
    row: number;
    cell: number;
}

export interface PositionInfo {
    bottom: number;
    height: number;
    left: number;
    right: number;
    top: number;
    visible: boolean;
    width: number;
}

export interface RangeInfo {
    top?: number;
    bottom?: number;
    leftPx?: number;
    rightPx?: number;
}

export declare class RowSelectionModel {
}

export interface Grid {
    init(): void;
    destroy(): void;
    getData(): any[];
    getDataItem(index: number): any;
    setData(data: any[], scrollToTop: boolean): void;
    getDataLength(): number;
    getOptions(): GridOptions;
    setOptions(options: GridOptions): void;
    getSelectedRows(): any;
    getSelectionModel(): any;
    setSelectionModel(model: any): any;
    setSelectedRows(rows: any): void;
    autoSizeColumns(): void;
    getColumnIndex(id: string): number;
    getColumns(): Column[];
    getUID(): string;
    setColumns(columns: Column[]): void;
    setSortColumn(columnId: string, ascending: boolean): void;
    setSortColumns(cols: ColumnSort[]): void;
    updateColumnHeader(columnId: string, title: string, toolTip: string): void;
    addCellCssStyles(key: string, hash: any): void;
    canCellBeActive(row: number, col: number): boolean;
    canCellBeSelected(row: number, col: number): boolean;
    editActiveCell(editor: Function): void;
    flashCell(row: number, cell: number, speed: number): void;
    getActiveCell(): RowCell;
    getActiveCellNode(): any;
    getActiveCellPosition(): PositionInfo;
    getCellCssStyles(key: string): any;
    getCellEditor(): any;
    getCellFromEvent(e: any): RowCell;
    getCellFromPoint(x: number, y: number): RowCell;
    getCellNode(row: number, cell: number): any;
    getCellNodeBox(row: number, cell: number): PositionInfo;
    goToCell(row: number, cell: number, forceEdit: boolean): void;
    navigateDown(): void;
    navigateLeft(): void;
    navigateNext(): void;
    navigatePrev(): void;
    navigateRight(): void;
    navigateUp(): void;
    removeCellCssStyles(key: string): void;
    resetActiveCell(): void;
    registerPlugin(plugin: any): void;
    setActiveCell(row: number, cell: number): void;
    setCellCssStyles(key: string, hash: any): void;
    getCanvasNode(): any;
    getGridPosition(): PositionInfo;
    getRenderedRange(viewportTop: number, viewportLeft: number): RangeInfo;
    getViewport(viewportTop: number, viewportLeft: number): RangeInfo;
    getViewport(): RangeInfo;
    invalidate(): void;
    invalidateAllRows(): void;
    invalidateRow(row: number): void;
    invalidateRows(rows: any): void;
    render(): void;
    resizeCanvas(): void;
    scrollCellIntoView(row: number, cell: number): void;
    scrollRowIntoView(row: number, doPaging: boolean): void;
    scrollRowToTop(row: number): void;
    updateCell(row: number, cell: number): void;
    updateRow(row: number): void;
    updateRowCount(): void;
    updateColumnHeader(columnId: string, title?: string, toolTip?: string): void;
    getGroupingPanel(): HTMLDivElement;
    getHeaderRow(): any;
    getEditorLock(): any;
    getHeaderRowColumn(columnId: string): any;
    getSortColumns(): any;
    getTopPanel(): any;
    setHeaderRowVisibility(visible: boolean): void;
    onScroll?: Slick.Event;
    onSort?: Slick.Event;
    onHeaderContextMenu?: Slick.Event;
    onHeaderClick?: Slick.Event;
    onMouseEnter?: Slick.Event;
    onMouseLeave?: Slick.Event;
    onClick?: Slick.Event;
    onDblClick?: Slick.Event;
    onContextMenu?: Slick.Event;
    onKeyDown?: Slick.Event;
    onAddNewRow?: Slick.Event;
    onValidationError?: Slick.Event;
    onViewportChanged?: Slick.Event;
    onColumnsReordered?: Slick.Event;
    onColumnsResized?: Slick.Event;
    onCellChange?: Slick.Event;
    onBeforeEditCell?: Slick.Event;
    onBeforeCellEditorDestroy?: Slick.Event;
    onHeaderCellRendered?: Slick.Event;
    onBeforeHeaderCellDestroy?: Slick.Event;
    onBeforeDestroy?: Slick.Event;
    onActiveCellChanged?: Slick.Event;
    onActiveCellPositionChanged?: Slick.Event;
    onDragInit?: Slick.Event;
    onDragStart?: Slick.Event;
    onDrag?: Slick.Event;
    onDragEnd?: Slick.Event;
    onSelectedRowsChanged?: Slick.Event;
    onCellCssStylesChanged?: Slick.Event;
}