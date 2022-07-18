/**
 * @license
 * (c) 2017-2022 Serenity.is
 * Ported to TypeScript
 *
 * (c) 2009-2013 Michael Leibman
 * michael{dot}leibman{at}gmail{dot}com
 * http://github.com/mleibman/slickgrid
 *
 * Distributed under MIT license.
 * All rights reserved.
 *
 * SlickGrid v2.2
 *
 * NOTES:
 *     Cell/row DOM manipulations are done directly bypassing jQuery's DOM manipulation methods.
 *     This increases the speed dramatically, but can only be done safely because there are no event handlers
 *     or data associated with any cell/row DOM nodes.  Cell editors must make sure they implement .destroy()
 *     and do proper cleanup.
 */
/// <reference types="jquery" />
declare namespace Slick {
    type ColumnFormatter<TItem = any> = (row: number, cell: number, value: any, column: Column, item: TItem, grid?: Grid, colMeta?: ColumnMetadata) => string;
    type ColumnFormat<TItem = any> = (ctx: Slick.FormatterContext<TItem>) => string;
    type AsyncPostRender<TItem = any> = (cellNode: HTMLElement, row: number, item: TItem, column: Column) => void;
    type AsyncPostCleanup<TItem = any> = (cellNode: HTMLElement, row?: number, column?: Column<TItem>) => void;
    interface FormatterContext<TItem = any> {
        row?: number;
        cell?: number;
        value?: any;
        column?: Column<TItem>;
        item?: TItem;
    }
    interface Plugin {
        init(grid: Grid): void;
        destroy?: () => void;
    }
    interface SelectionModel {
        init(grid: Grid): void;
        destroy?: () => void;
        setSelectedRanges(ranges: Range[]): void;
        onSelectedRangesChanged: Event<Range[]>;
    }
    interface ColumnSort {
        columnId: string;
        sortAsc?: boolean;
    }
    interface RowCell {
        row: number;
        cell: number;
    }
    interface Position {
        bottom?: number;
        height?: number;
        left?: number;
        right?: number;
        top?: number;
        visible?: boolean;
        width?: number;
    }
    interface ViewRange {
        top?: number;
        bottom?: number;
        leftPx?: number;
        rightPx?: number;
    }
    interface EditorOptions<TItem> {
        grid: Grid<TItem>;
        gridPosition?: Position;
        position?: Position;
        container?: HTMLElement;
        column?: Column;
        item?: TItem;
        commitChanges?: () => void;
        cancelChanges?: () => void;
    }
    interface Editor<TItem = any> {
        new (options: EditorOptions<TItem>): Editor<TItem>;
        destroy(): void;
        applyValue(item: TItem, value: any): void;
        focus(): void;
        isValueChanged(): boolean;
        loadValue(value: any): void;
        serializeValue(): any;
        position?(pos: Position): void;
        hide?(): void;
        show?(): void;
        validate?(): {
            valid: boolean;
        };
    }
    interface EditorFactory<TItem = any> {
        getEditor(column: Column<TItem>): Editor;
    }
    interface EditCommand<TItem = any> {
        row: number;
        cell: number;
        editor: Editor<TItem>;
        serializedValue: any;
        prevSerializedValue: any;
        execute: () => void;
        undo: () => void;
    }
    interface FormatterFactory<TItem = any> {
        getFormatter(column: Column<TItem>): ColumnFormatter<TItem>;
    }
    interface ColumnMetadata<TItem = any> {
        colspan: number | '*';
        formatter?: ColumnFormatter<TItem>;
    }
    interface ItemMetadata<TItem = any> {
        columns?: {
            [key: string]: ColumnMetadata<TItem>;
        };
        formatter?: ColumnFormatter<TItem>;
    }
    interface GridEventArgs {
        grid?: Grid;
    }
    interface ColNodeEventArgs extends GridEventArgs {
        column: Column;
        node: HTMLElement;
    }
    type SortEventCol = {
        sortCol: Column;
        sortAsc: boolean;
    };
    interface SortEventArgs extends GridEventArgs {
        multiColumnSort: boolean;
        sortAsc?: boolean;
        sortCol?: Column;
        sortCols?: SortEventCol[];
    }
    interface ColumnReorderEventArgs extends GridEventArgs {
        impactedColumns: Column[];
    }
    interface RowNumbersEventArgs extends GridEventArgs {
        rows: number[];
    }
    interface ScrollEventArgs extends GridEventArgs {
        scrollLeft: number;
        scrollTop: number;
    }
    interface CellCssStyleEventArgs extends GridEventArgs {
        key: string;
        hash: CellStylesHash;
    }
    interface RowCellEventArgs extends GridEventArgs {
        row: number;
        cell: number;
    }
    type CellStylesHash = {
        [row: number]: {
            [cell: number]: string;
        };
    };
    interface Column<TItem = any> {
        asyncPostRender?: AsyncPostRender<TItem>;
        asyncPostCleanup?: AsyncPostCleanup<TItem>;
        behavior?: any;
        cannotTriggerInsert?: boolean;
        cssClass?: string;
        defaultSortAsc?: boolean;
        editor?: Editor;
        field: string;
        focusable?: boolean;
        footerCssClass?: string;
        format?: ColumnFormat<TItem>;
        formatter?: ColumnFormatter<TItem>;
        groupTotalsFormatter?: (p1?: GroupTotals<TItem>, p2?: Column<TItem>, grid?: Grid) => string;
        headerCssClass?: string;
        id?: string;
        maxWidth?: any;
        minWidth?: number;
        name?: string;
        previousWidth?: number;
        referencedFields?: string[];
        rerenderOnResize?: boolean;
        resizable?: boolean;
        selectable?: boolean;
        sortable?: boolean;
        toolTip?: string;
        width?: number;
        sortOrder?: number;
        visible?: boolean;
    }
    interface GridOptions<TItem = any> {
        addNewRowCssClass?: string;
        asyncEditorLoading?: boolean;
        asyncEditorLoadDelay?: number;
        asyncPostRenderDelay?: number;
        asyncPostCleanupDelay?: number;
        autoEdit?: boolean;
        autoHeight?: boolean;
        cellFlashingCssClass?: string;
        cellHighlightCssClass?: string;
        columns?: Column[];
        dataItemColumnValueExtractor?: (item: TItem, column: Column) => void;
        groupingPanel?: boolean;
        groupingPanelHeight?: number;
        setGroupingPanelVisibility?: (value: boolean) => void;
        defaultColumnWidth?: number;
        defaultFormatter?: ColumnFormatter;
        editable?: boolean;
        editCommandHandler?: (item: TItem, column: Column<TItem>, command: EditCommand) => void;
        editorFactory?: EditorFactory<TItem>;
        editorLock?: EditorLock;
        enableAddRow?: boolean;
        enableAsyncPostCleanup?: boolean;
        enableAsyncPostRender?: boolean;
        enableCellRangeSelection?: boolean;
        enableCellNavigation?: boolean;
        enableColumnReorder?: boolean;
        enableRowReordering?: boolean;
        enableTextSelectionOnCells?: boolean;
        explicitInitialization?: boolean;
        footerRowHeight?: number;
        forceFitColumns?: boolean;
        forceSyncScrolling?: boolean;
        formatterFactory?: FormatterFactory;
        fullWidthRows?: boolean;
        frozenColumn?: number;
        frozenRow?: number;
        frozenBottom?: boolean;
        headerRowHeight?: number;
        leaveSpaceForNewRows?: boolean;
        minBuffer?: number;
        multiColumnSort?: boolean;
        multiSelect?: boolean;
        renderAllCells?: boolean;
        rowHeight?: number;
        selectedCellCssClass?: string;
        showGroupingPanel?: boolean;
        showHeaderRow?: boolean;
        showFooterRow?: boolean;
        showTopPanel?: boolean;
        syncColumnCellResize?: boolean;
        topPanelHeight?: number;
    }
    /**
     * Creates a new instance of the grid.
     * @class Grid
     * @constructor
     * @param {Node}              container   Container node to create the grid in.
     * @param {Array,Object}      data        An array of objects for databinding.
     * @param {Array}             columns     An array of column definitions.
     * @param {Object}            options     Grid options.
     **/
    class Grid<TItem = any> {
        private _options;
        private _columns;
        private _columnDefaults;
        private _data;
        private th;
        private h;
        private ph;
        private n;
        private cj;
        private page;
        private offset;
        private vScrollDir;
        private initialized;
        private $container;
        private uid;
        private $focusSink;
        private $focusSink2;
        private $headerScroller;
        private $headers;
        private $headerRow;
        private $headerRowScroller;
        private $headerRowSpacerL;
        private $headerRowSpacerR;
        private $footerRow;
        private $footerRowScroller;
        private $footerRowSpacerL;
        private $footerRowSpacerR;
        private $groupingPanel;
        private $topPanelScroller;
        private $topPanel;
        private $viewport;
        private $canvas;
        private $style;
        private $boundAncestors;
        private stylesheet;
        private columnCssRulesL;
        private columnCssRulesR;
        private viewportH;
        private viewportW;
        private canvasWidth;
        private canvasWidthL;
        private canvasWidthR;
        private headersWidthL;
        private headersWidthR;
        private viewportHasHScroll;
        private viewportHasVScroll;
        private headerColumnWidthDiff;
        private cellWidthDiff;
        private cellHeightDiff;
        private jQueryNewWidthBehaviour;
        private absoluteColumnMinWidth;
        private hasFrozenRows;
        private frozenRowsHeight;
        private actualFrozenRow;
        private paneTopH;
        private paneBottomH;
        private viewportTopH;
        private topPanelH;
        private groupPanelH;
        private headerH;
        private headerRowH;
        private footerRowH;
        private tabbingDirection;
        private activePosX;
        private activeRow;
        private activeCell;
        private activeCellNode;
        private currentEditor;
        private serializedEditorValue;
        private editController;
        private rowsCache;
        private renderedRows;
        private numVisibleRows;
        private prevScrollTop;
        private scrollTop;
        private lastRenderedScrollTop;
        private lastRenderedScrollLeft;
        private prevScrollLeft;
        private scrollLeft;
        private selectionModel;
        private selectedRows;
        private plugins;
        private cellCssClasses;
        private columnsById;
        private sortColumns;
        private columnPosLeft;
        private columnPosRight;
        private rtl;
        private xLeft;
        private xRight;
        private h_editorLoader;
        private h_render;
        private h_postrender;
        private h_postrenderCleanup;
        private postProcessedRows;
        private postProcessToRow;
        private postProcessFromRow;
        private postProcessedCleanupQueue;
        private postProcessgroupId;
        private $paneHeaderL;
        private $paneHeaderR;
        private $paneTopL;
        private $paneTopR;
        private $paneBottomL;
        private $paneBottomR;
        private $headerScrollerL;
        private $headerScrollerR;
        private $headerL;
        private $headerR;
        private $headerRowScrollerL;
        private $headerRowScrollerR;
        private $footerRowScrollerL;
        private $footerRowScrollerR;
        private $headerRowL;
        private $headerRowR;
        private $footerRowL;
        private $footerRowR;
        private $topPanelScrollerL;
        private $topPanelScrollerR;
        private $topPanelL;
        private $topPanelR;
        private $viewportTopL;
        private $viewportTopR;
        private $viewportBottomL;
        private $viewportBottomR;
        private $canvasTopL;
        private $canvasTopR;
        private $canvasBottomL;
        private $canvasBottomR;
        private $viewportScrollContainerX;
        private $viewportScrollContainerY;
        private $headerScrollContainer;
        private $headerRowScrollContainer;
        private $footerRowScrollContainer;
        readonly onScroll: Event<ScrollEventArgs, IEventData>;
        readonly onSort: Event<SortEventArgs, IEventData>;
        readonly onHeaderMouseEnter: Event<any>;
        readonly onHeaderMouseLeave: Event<any>;
        readonly onHeaderContextMenu: Event<any>;
        readonly onHeaderClick: Event<any>;
        readonly onHeaderCellRendered: Event<ColNodeEventArgs, IEventData>;
        readonly onBeforeHeaderCellDestroy: Event<ColNodeEventArgs, IEventData>;
        readonly onHeaderRowCellRendered: Event<ColNodeEventArgs, IEventData>;
        readonly onFooterRowCellRendered: Event<ColNodeEventArgs, IEventData>;
        readonly onBeforeHeaderRowCellDestroy: Event<ColNodeEventArgs, IEventData>;
        readonly onBeforeFooterRowCellDestroy: Event<ColNodeEventArgs, IEventData>;
        readonly onMouseEnter: Event<any>;
        readonly onMouseLeave: Event<any>;
        readonly onClick: Event<RowCellEventArgs, JQueryMouseEventObject>;
        readonly onDblClick: Event<any>;
        readonly onContextMenu: Event<GridEventArgs, JQueryEventObject>;
        readonly onKeyDown: Event<RowCellEventArgs, JQueryKeyEventObject>;
        readonly onAddNewRow: Event<any>;
        readonly onValidationError: Event<any>;
        readonly onViewportChanged: Event<GridEventArgs, IEventData>;
        readonly onColumnsReordered: Event<ColumnReorderEventArgs, IEventData>;
        readonly onColumnsResized: Event<GridEventArgs, IEventData>;
        readonly onCellChange: Event<any>;
        readonly onBeforeEditCell: Event<any>;
        readonly onBeforeCellEditorDestroy: Event<any>;
        readonly onBeforeDestroy: Event<GridEventArgs, IEventData>;
        readonly onActiveCellChanged: Event<any>;
        readonly onActiveCellPositionChanged: Event<any>;
        readonly onDragInit: Event<any, JQueryEventObject>;
        readonly onDragStart: Event<any, JQueryEventObject>;
        readonly onDrag: Event<any, JQueryEventObject>;
        readonly onDragEnd: Event<any, JQueryEventObject>;
        readonly onSelectedRowsChanged: Event<RowNumbersEventArgs, IEventData>;
        readonly onCellCssStylesChanged: Event<CellCssStyleEventArgs, IEventData>;
        constructor(container: JQuery, data: any, columns: Column[], options: GridOptions<TItem>);
        init(): void;
        private hasFrozenColumns;
        registerPlugin(plugin: Plugin): void;
        private unregisterPlugin;
        setSelectionModel(model: SelectionModel): void;
        getSelectionModel(): SelectionModel;
        getCanvasNode(): HTMLDivElement;
        getCanvasNodes(): JQuery;
        getViewportNode(): HTMLDivElement;
        getViewportNodes(): JQuery;
        private measureScrollbar;
        private calcHeaderWidths;
        private getCanvasWidth;
        private updateCanvasWidth;
        private bindAncestorScrollEvents;
        private unbindAncestorScrollEvents;
        updateColumnHeader(columnId: string, title?: string, toolTip?: string): void;
        getHeader(): HTMLDivElement;
        getHeaderColumn(columnIdOrIdx: string | number): HTMLDivElement;
        getGroupingPanel(): HTMLDivElement;
        getHeaderRow(): HTMLDivElement;
        getHeaderRowColumn(columnId: string): any;
        getFooterRow(): HTMLDivElement;
        getFooterRowColumn(columnId: string): HTMLElement;
        private createColumnFooter;
        private formatGroupTotal;
        private groupTotalText;
        private groupTotalsFormatter;
        private createColumnHeaders;
        private setupColumnSort;
        private setupColumnReorder;
        private getImpactedColumns;
        private setupColumnResize;
        private getVBoxDelta;
        private setFrozenOptions;
        private setPaneVisibility;
        private setOverflow;
        private setScroller;
        private measureCellPaddingAndBorder;
        private createCssRules;
        private getColumnCssRules;
        private removeCssRules;
        destroy(): void;
        private trigger;
        getEditorLock(): EditorLock;
        getEditController(): any;
        getColumnIndex(id: string): any;
        autosizeColumns(): void;
        private applyColumnHeaderWidths;
        private applyColumnWidths;
        setSortColumn(columnId: string, ascending: boolean): void;
        setSortColumns(cols: ColumnSort[]): void;
        getSortColumns(): ColumnSort[];
        private handleSelectedRangesChanged;
        private getColumns;
        private updateColumnCaches;
        private setColumns;
        getOptions(): GridOptions<TItem>;
        setOptions(args: GridOptions<TItem>): void;
        private validateAndEnforceOptions;
        private viewOnRowCountChanged;
        private viewOnRowsChanged;
        private viewOnDataChanged;
        private bindToData;
        private unbindFromData;
        setData(newData: any, scrollToTop?: boolean): void;
        getData(): any;
        getDataLength(): number;
        getDataLengthIncludingAddNew(): number;
        getDataItem(i: number): TItem;
        getTopPanel(): HTMLDivElement;
        setTopPanelVisibility(visible: boolean): void;
        setHeaderRowVisibility(visible: boolean): void;
        setFooterRowVisibility(visible: boolean): void;
        setGroupingPanelVisibility(visible: boolean): void;
        getContainerNode(): HTMLDivElement;
        getUID(): string;
        private getRowTop;
        private getRowFromPosition;
        private scrollTo;
        private getFormatter;
        private callFormatter;
        private getEditor;
        private getDataItemValueForColumn;
        private appendRowHtml;
        private appendCellHtml;
        private cleanupRows;
        invalidate(): void;
        invalidateAllRows(): void;
        private queuePostProcessedRowForCleanup;
        private queuePostProcessedCellForCleanup;
        private removeRowFromCache;
        invalidateRows(rows: number[]): void;
        invalidateRow(row: number): void;
        updateCell(row: number, cell: number): void;
        updateRow(row: number): void;
        private calcViewportHeight;
        private calcViewportWidth;
        private resizeCanvas;
        private updateRowCount;
        /**
         * @param viewportTop optional viewport top
         * @param viewportLeft optional viewport left
         * @returns viewport range
         */
        getViewport(viewportTop?: number, viewportLeft?: number): ViewRange;
        getVisibleRange(viewportTop?: number, viewportLeft?: number): ViewRange;
        getRenderedRange(viewportTop?: number, viewportLeft?: number): ViewRange;
        private ensureCellNodesInRowsCache;
        private cleanUpCells;
        private cleanUpAndRenderCells;
        private renderRows;
        private startPostProcessing;
        private startPostProcessingCleanup;
        private invalidatePostProcessingResults;
        private updateRowPositions;
        private updateFooterTotals;
        private render;
        private handleHeaderRowScroll;
        private handleFooterRowScroll;
        private handleMouseWheel;
        private handleScroll;
        private _handleScroll;
        private asyncPostProcessRows;
        private asyncPostProcessCleanupRows;
        private updateCellCssStylesOnRenderedRows;
        addCellCssStyles(key: string, hash: CellStylesHash): void;
        removeCellCssStyles(key: string): void;
        setCellCssStyles(key: string, hash: CellStylesHash): void;
        getCellCssStyles(key: string): CellStylesHash;
        flashCell(row: number, cell: number, speed?: number): void;
        private handleDragInit;
        private handleDragStart;
        private handleDrag;
        private handleDragEnd;
        private handleKeyDown;
        private handleClick;
        private handleContextMenu;
        private handleDblClick;
        private handleHeaderMouseEnter;
        private handleHeaderMouseLeave;
        private handleHeaderContextMenu;
        private handleHeaderClick;
        private handleMouseEnter;
        private handleMouseLeave;
        private cellExists;
        getCellFromPoint(x: number, y: number): {
            row: number;
            cell: number;
        };
        getCellFromNode(cellNode: HTMLElement): number;
        getRowFromNode(rowNode: HTMLElement): number;
        private getFrozenRowOffset;
        getCellFromEvent(e: any): {
            row: number;
            cell: number;
        };
        getCellNodeBox(row: number, cell: number): {
            top: number;
            right: number;
            bottom: number;
            left: number;
        };
        resetActiveCell(): void;
        focus(): void;
        private setFocus;
        scrollCellIntoView(row: number, cell: number, doPaging?: boolean): void;
        private setActiveCellInternal;
        clearTextSelection(): void;
        private isCellPotentiallyEditable;
        private makeActiveCellNormal;
        editActiveCell(editor?: Editor): void;
        private makeActiveCellEditable;
        private commitEditAndSetFocus;
        private cancelEditAndSetFocus;
        private absBox;
        private getActiveCellPosition;
        getGridPosition(): Position;
        private handleActiveCellPositionChange;
        getCellEditor(): Editor<any>;
        getActiveCell(): RowCell;
        getActiveCellNode(): HTMLElement;
        scrollActiveCellIntoView(): void;
        scrollRowIntoView(row: number, doPaging?: boolean): void;
        scrollRowToTop(row: number): void;
        private scrollPage;
        navigatePageDown(): void;
        navigatePageUp(): void;
        getColspan(row: number, cell: number): number;
        findFirstFocusableCell(row: number): number;
        findLastFocusableCell(row: number): number;
        gotoRight(row?: number, cell?: number, posX?: number): {
            row: any;
            cell: any;
            posX: any;
        };
        gotoLeft(row?: number, cell?: number, posX?: number): {
            row: number;
            cell: number;
            posX: number;
        };
        gotoDown(row?: number, cell?: number, posX?: number): {
            row: number;
            cell: number;
            posX: number;
        };
        gotoUp(row?: number, cell?: number, posX?: number): {
            row: number;
            cell: number;
            posX: number;
        };
        gotoNext(row?: number, cell?: number, posX?: number): {
            row: any;
            cell: any;
            posX: any;
        };
        gotoPrev(row?: number, cell?: number, posX?: number): {
            row: number;
            cell: number;
            posX: number;
        };
        navigateRight(): boolean;
        navigateLeft(): boolean;
        navigateDown(): boolean;
        navigateUp(): boolean;
        navigateNext(): boolean;
        navigatePrev(): boolean;
        /**
         * @param {string} dir Navigation direction.
         * @return {boolean} Whether navigation resulted in a change of active cell.
         */
        navigate(dir: string): boolean;
        getCellNode(row: number, cell: number): HTMLElement;
        setActiveCell(row: number, cell: number): void;
        private canCellBeActive;
        canCellBeSelected(row: number, cell: number): any;
        gotoCell(row: number, cell: number, forceEdit?: boolean): void;
        commitCurrentEdit(): boolean;
        private cancelCurrentEdit;
        private rowsToRanges;
        private getSelectedRows;
        private setSelectedRows;
    }
}
