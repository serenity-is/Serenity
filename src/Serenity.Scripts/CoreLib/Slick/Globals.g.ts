
declare namespace Slick {
    /***
     * A base class that all special / non-data rows (like Group and GroupTotals) derive from.
     */
    export class NonDataRow {
        __nonDataRow: boolean;
    }
    export const preClickClassName = "slick-edit-preclick";

    export type Handler<TArgs, TEventData extends IEventData = IEventData> = (e: TEventData, args: TArgs) => void;
    export interface IEventData {
        readonly type?: string;
        currentTarget?: EventTarget;
        target?: EventTarget;
        originalEvent?: any;
        defaultPrevented?: boolean;
        preventDefault?(): void;
        stopPropagation?(): void;
        stopImmediatePropagation?(): void;
        isDefaultPrevented?(): boolean;
        isImmediatePropagationStopped?(): boolean;
        isPropagationStopped?(): boolean;
    }
    /***
     * An event object for passing data to event handlers and letting them control propagation.
     * <p>This is pretty much identical to how W3C and jQuery implement events.</p>
     * @class EventData
     * @constructor
     */
    export class EventData implements IEventData {
        private _isPropagationStopped;
        private _isImmediatePropagationStopped;
        /***
         * Stops event from propagating up the DOM tree.
         * @method stopPropagation
         */
        stopPropagation(): void;
        /***
         * Returns whether stopPropagation was called on this event object.
         * @method isPropagationStopped
         * @return {Boolean}
         */
        isPropagationStopped(): boolean;
        /***
         * Prevents the rest of the handlers from being executed.
         * @method stopImmediatePropagation
         */
        stopImmediatePropagation(): void;
        /***
         * Returns whether stopImmediatePropagation was called on this event object.\
         * @method isImmediatePropagationStopped
         * @return {Boolean}
         */
        isImmediatePropagationStopped(): boolean;
    }
    /***
     * A simple publisher-subscriber implementation.
     * @class Event
     * @constructor
     */
    export class Event<TArgs = any, TEventData extends IEventData = IEventData> {
        private _handlers;
        /***
         * Adds an event handler to be called when the event is fired.
         * <p>Event handler will receive two arguments - an <code>EventData</code> and the <code>data</code>
         * object the event was fired with.<p>
         * @method subscribe
         * @param fn {Function} Event handler.
         */
        subscribe(fn: Handler<TArgs, TEventData>): void;
        /***
         * Removes an event handler added with <code>subscribe(fn)</code>.
         * @method unsubscribe
         * @param fn {Function} Event handler to be removed.
         */
        unsubscribe(fn: Handler<TArgs, TEventData>): void;
        /***
         * Fires an event notifying all subscribers.
         * @method notify
         * @param args {Object} Additional data object to be passed to all handlers.
         * @param e {EventData}
         *      Optional.
         *      An <code>EventData</code> object to be passed to all handlers.
         *      For DOM events, an existing W3C/jQuery event object can be passed in.
         * @param scope {Object}
         *      Optional.
         *      The scope ("this") within which the handler will be executed.
         *      If not specified, the scope will be set to the <code>Event</code> instance.
         */
        notify(args?: any, e?: TEventData, scope?: object): any;
        clear(): void;
    }
    export class EventHandler<TArgs = any, TEventData extends IEventData = IEventData> {
        private _handlers;
        subscribe(event: Event<TArgs, TEventData>, handler: Handler<TArgs, TEventData>): this;
        unsubscribe(event: Event<TArgs, TEventData>, handler: Handler<TArgs, TEventData>): this;
        unsubscribeAll(): EventHandler<TArgs, TEventData>;
    }
    export const keyCode: {
        BACKSPACE: number;
        DELETE: number;
        DOWN: number;
        END: number;
        ENTER: number;
        ESCAPE: number;
        HOME: number;
        INSERT: number;
        LEFT: number;
        PAGEDOWN: number;
        PAGEUP: number;
        RIGHT: number;
        TAB: number;
        UP: number;
    };

    export interface EditController {
        commitCurrentEdit(): boolean;
        cancelCurrentEdit(): boolean;
    }
    /***
     * A locking helper to track the active edit controller and ensure that only a single controller
     * can be active at a time.  This prevents a whole class of state and validation synchronization
     * issues.  An edit controller (such as SleekGrid) can query if an active edit is in progress
     * and attempt a commit or cancel before proceeding.
     * @class EditorLock
     * @constructor
     */
    export class EditorLock {
        private activeEditController;
        /***
         * Returns true if a specified edit controller is active (has the edit lock).
         * If the parameter is not specified, returns true if any edit controller is active.
         * @method isActive
         * @param editController {EditController}
         * @return {Boolean}
         */
        isActive(editController?: EditController): boolean;
        /***
         * Sets the specified edit controller as the active edit controller (acquire edit lock).
         * If another edit controller is already active, and exception will be thrown.
         * @method activate
         * @param editController {EditController} edit controller acquiring the lock
         */
        activate(editController: EditController): void;
        /***
         * Unsets the specified edit controller as the active edit controller (release edit lock).
         * If the specified edit controller is not the active one, an exception will be thrown.
         * @method deactivate
         * @param editController {EditController} edit controller releasing the lock
         */
        deactivate(editController: EditController): void;
        /***
         * Attempts to commit the current edit by calling "commitCurrentEdit" method on the active edit
         * controller and returns whether the commit attempt was successful (commit may fail due to validation
         * errors, etc.).  Edit controller's "commitCurrentEdit" must return true if the commit has succeeded
         * and false otherwise.  If no edit controller is active, returns true.
         * @method commitCurrentEdit
         * @return {Boolean}
         */
        commitCurrentEdit(): boolean;
        /***
         * Attempts to cancel the current edit by calling "cancelCurrentEdit" method on the active edit
         * controller and returns whether the edit was successfully cancelled.  If no edit controller is
         * active, returns true.
         * @method cancelCurrentEdit
         * @return {Boolean}
         */
        cancelCurrentEdit(): boolean;
    }
    /***
     * A global singleton editor lock.
     * @class GlobalEditorLock
     * @static
     * @constructor
     */
    export const GlobalEditorLock: EditorLock;

    /***
     * Information about a group of rows.
     */
    export class Group<TEntity = any> extends NonDataRow {
        readonly __group = true;
        /**
         * Grouping level, starting with 0.
         * @property level
         * @type {Number}
         */
        level: number;
        /***
         * Number of rows in the group.
         * @property count
         * @type {Integer}
         */
        count: number;
        /***
         * Grouping value.
         * @property value
         * @type {Object}
         */
        value: any;
        /***
         * Formatted display value of the group.
         * @property title
         * @type {String}
         */
        title: string;
        /***
         * Whether a group is collapsed.
         * @property collapsed
         * @type {Boolean}
         */
        collapsed: boolean;
        /***
         * GroupTotals, if any.
         * @property totals
         * @type {GroupTotals}
         */
        totals: GroupTotals<TEntity>;
        /**
         * Rows that are part of the group.
         * @property rows
         * @type {Array}
         */
        rows: TEntity[];
        /**
         * Sub-groups that are part of the group.
         * @property groups
         * @type {Array}
         */
        groups: Group<TEntity>[];
        /**
         * A unique key used to identify the group.  This key can be used in calls to DataView
         * collapseGroup() or expandGroup().
         * @property groupingKey
         * @type {Object}
         */
        groupingKey: string;
    }
    /***
     * Information about group totals.
     * An instance of GroupTotals will be created for each totals row and passed to the aggregators
     * so that they can store arbitrary data in it.  That data can later be accessed by group totals
     * formatters during the display.
     * @class GroupTotals
     * @extends Sleek.NonDataRow
     * @constructor
     */
    export class GroupTotals<TEntity = any> extends NonDataRow {
        readonly __groupTotals = true;
        /***
         * Parent Group.
         * @param group
         * @type {Group}
         */
        group: Group<TEntity>;
        /***
         * Whether the totals have been fully initialized / calculated.
         * Will be set to false for lazy-calculated group totals.
         * @param initialized
         * @type {Boolean}
         */
        initialized: boolean;
        /**
         * Contains sum
         */
        sum?: number;
        /**
         * Contains avg
         */
        avg?: number;
        /**
         * Contains min
         */
        min?: any;
        /**
         * Contains max
         */
        max?: any;
    }

    export class Range {
        fromRow: number;
        fromCell: number;
        toRow: number;
        toCell: number;
        constructor(fromRow: number, fromCell: number, toRow?: number, toCell?: number);
        /***
         * Returns whether a range represents a single row.
         */
        isSingleRow(): boolean;
        /***
         * Returns whether a range represents a single cell.
         */
        isSingleCell(): boolean;
        /***
         * Returns whether a range contains a given cell.
         */
        contains(row: number, cell: number): boolean;
        /***
         * Returns a readable representation of a range.
         */
        toString(): string;
    }

    export interface IPlugin {
        init(grid: Grid): void;
        pluginName?: string;
        destroy?: () => void;
    }
    export interface Position {
        bottom?: number;
        height?: number;
        left?: number;
        right?: number;
        top?: number;
        visible?: boolean;
        width?: number;
    }
    export interface RowCell {
        row: number;
        cell: number;
    }
    export interface SelectionModel extends IPlugin {
        setSelectedRanges(ranges: Range[]): void;
        onSelectedRangesChanged: Event<Range[]>;
        refreshSelections?(): void;
    }
    export interface ViewRange {
        top?: number;
        bottom?: number;
        leftPx?: number;
        rightPx?: number;
    }

    export interface EditorOptions {
        grid: Grid;
        gridPosition?: Position;
        position?: Position;
        column?: Column;
        container?: HTMLElement;
        item?: any;
        event: IEventData;
        commitChanges?: () => void;
        cancelChanges?: () => void;
    }
    export interface Editor {
        new (options: EditorOptions): Editor;
        destroy(): void;
        applyValue(item: any, value: any): void;
        focus(): void;
        isValueChanged(): boolean;
        keyCaptureList?: number[];
        loadValue(value: any): void;
        serializeValue(): any;
        position?(pos: Position): void;
        preClick?(): void;
        hide?(): void;
        show?(): void;
        suppressClearOnEdit?: boolean;
        validate?(): ValidationResult;
    }
    export interface EditorFactory {
        getEditor(column: Column): Editor;
    }
    export interface EditCommand {
        row: number;
        cell: number;
        editor: Editor;
        serializedValue: any;
        prevSerializedValue: any;
        execute: () => void;
        undo: () => void;
    }
    export interface ValidationResult {
        valid: boolean;
        msg?: string;
    }

    export interface FormatterFactory<TItem = any> {
        getFormatter(column: Column<TItem>): ColumnFormatter<TItem>;
    }
    export interface FormatterResult {
        addClass?: string;
        addAttrs?: {
            [key: string]: string;
        };
        text?: string;
        toolTip?: string;
    }
    export type ColumnFormatter<TItem = any> = (row: number, cell: number, value: any, column: Column<TItem>, item: TItem, grid?: Grid<TItem>) => string | FormatterResult;
    export type AsyncPostRender<TItem = any> = (cellNode: HTMLElement, row: number, item: TItem, column: Column<TItem>, reRender: boolean) => void;
    export type AsyncPostCleanup<TItem = any> = (cellNode: HTMLElement, row?: number, column?: Column<TItem>) => void;
    export type CellStylesHash = {
        [row: number]: {
            [cell: number]: string;
        };
    };

    export interface ArgsGrid {
        grid?: Grid;
    }
    export interface ArgsColumn extends ArgsGrid {
        column: Column;
    }
    export interface ArgsColumnNode extends ArgsColumn {
        node: HTMLElement;
    }
    export type ArgsSortCol = {
        sortCol: Column;
        sortAsc: boolean;
    };
    export interface ArgsSort extends ArgsGrid {
        multiColumnSort: boolean;
        sortAsc?: boolean;
        sortCol?: Column;
        sortCols?: ArgsSortCol[];
    }
    export interface ArgsSelectedRowsChange extends ArgsGrid {
        rows: number[];
        changedSelectedRows?: number[];
        changedUnselectedRows?: number[];
        previousSelectedRows?: number[];
        caller: any;
    }
    export interface ArgsScroll extends ArgsGrid {
        scrollLeft: number;
        scrollTop: number;
    }
    export interface ArgsCssStyle extends ArgsGrid {
        key: string;
        hash: CellStylesHash;
    }
    export interface ArgsCell extends ArgsGrid {
        row: number;
        cell: number;
    }
    export interface ArgsCellChange extends ArgsCell {
        item: any;
    }
    export interface ArgsCellEdit extends ArgsCellChange {
        column: Column;
    }
    export interface ArgsAddNewRow extends ArgsColumn {
        item: any;
    }
    export interface ArgsEditorDestroy extends ArgsGrid {
        editor: Editor;
    }
    export interface ArgsValidationError extends ArgsCell {
        editor: Editor;
        column: Column;
        cellNode: HTMLElement;
        validationResults: ValidationResult;
    }

    export interface GridOptions<TItem = any> {
        addNewRowCssClass?: string;
        alwaysAllowHorizontalScroll?: boolean;
        alwaysShowVerticalScroll?: boolean;
        asyncEditorLoadDelay?: number;
        asyncEditorLoading?: boolean;
        asyncPostCleanupDelay?: number;
        asyncPostRenderDelay?: number;
        autoEdit?: boolean;
        autoHeight?: boolean;
        cellFlashingCssClass?: string;
        cellHighlightCssClass?: string;
        columns?: Column<TItem>[];
        dataItemColumnValueExtractor?: (item: TItem, column: Column<TItem>) => void;
        defaultColumnWidth?: number;
        defaultFormatter?: ColumnFormatter<TItem>;
        editable?: boolean;
        editCommandHandler?: (item: TItem, column: Column<TItem>, command: EditCommand) => void;
        editorFactory?: EditorFactory;
        editorLock?: EditorLock;
        enableAddRow?: boolean;
        enableAsyncPostRender?: boolean;
        enableAsyncPostRenderCleanup?: boolean;
        enableCellNavigation?: boolean;
        enableCellRangeSelection?: boolean;
        enableColumnReorder?: boolean;
        enableRowReordering?: boolean;
        enableTabKeyNavigation?: boolean;
        enableTextSelectionOnCells?: boolean;
        explicitInitialization?: boolean;
        footerRowHeight?: number;
        forceFitColumns?: boolean;
        forceSyncScrolling?: boolean;
        formatterFactory?: FormatterFactory;
        frozenBottom?: boolean;
        frozenColumn?: number;
        frozenRow?: number;
        fullWidthRows?: boolean;
        groupingPanel?: boolean;
        groupingPanelHeight?: number;
        headerRowHeight?: number;
        leaveSpaceForNewRows?: boolean;
        minBuffer?: number;
        multiColumnSort?: boolean;
        multiSelect?: boolean;
        renderAllCells?: boolean;
        rowHeight?: number;
        selectedCellCssClass?: string;
        showCellSelection?: boolean;
        showColumnHeader?: boolean;
        showFooterRow?: boolean;
        showGroupingPanel?: boolean;
        showHeaderRow?: boolean;
        showTopPanel?: boolean;
        slickCompat?: boolean;
        suppressActiveCellChangeOnEdit?: boolean;
        syncColumnCellResize?: boolean;
        topPanelHeight?: number;
        useLegacyUI?: boolean;
        viewportClass?: string;
    }
    export const gridDefaults: GridOptions;

    export class Grid<TItem = any> {
        private _absoluteColMinWidth;
        private _activeCanvasNode;
        private _activeCell;
        private _activeCellNode;
        private _activePosX;
        private _activeRow;
        private _activeViewportNode;
        private _actualFrozenRow;
        private _canvasWidth;
        private _canvasWidthL;
        private _canvasWidthR;
        private _cellCssClasses;
        private _cellHeightDiff;
        private _cellWidthDiff;
        private _colById;
        private _colDefaults;
        private _colLeft;
        private _colRight;
        private _cols;
        private _columnCssRulesL;
        private _columnCssRulesR;
        private _currentEditor;
        private _data;
        private _editController;
        private _frozenCols;
        private _footerRowH;
        private _frozenRowsHeight;
        private _groupingPanelH;
        private _hasFrozenRows;
        private _headerColumnWidthDiff;
        private _headerRowH;
        private _headersWidthL;
        private _headersWidthR;
        private _hEditorLoader;
        private _hPostRender;
        private _hPostRenderCleanup;
        private _hRender;
        private _ignoreScrollUntil;
        private _initColById;
        private _initCols;
        private _initialized;
        private _jQueryNewWidthBehaviour;
        private _jumpinessCoefficient;
        private _numberOfPages;
        private _numVisibleRows;
        private _options;
        private _page;
        private _pageHeight;
        private _pageOffset;
        private _pagingActive;
        private _pagingIsLastPage;
        private _paneBottomH;
        private _paneTopH;
        private _plugins;
        private _postProcessCleanupQueue;
        private _postProcessedRows;
        private _postProcessFromRow;
        private _postProcessGroupId;
        private _postProcessToRow;
        private _realScrollHeight;
        private _rowsCache;
        private _rtl;
        private _rtlE;
        private _rtlS;
        private _scrollDims;
        private _scrollLeft;
        private _scrollLeftPrev;
        private _scrolLLeftRendered;
        private _scrollTop;
        private _scrollTopPrev;
        private _scrollTopRendered;
        private _selectedRows;
        private _selectionModel;
        private _serializedEditorValue;
        private _sortColumns;
        private _styleNode;
        private _stylesheet;
        private _tabbingDirection;
        private _topPanelH;
        private _uid;
        private _viewportH;
        private _viewportHasHScroll;
        private _viewportHasVScroll;
        private _viewportTopH;
        private _viewportW;
        private _virtualHeight;
        private _vScrollDir;
        private _boundAncestorScroll;
        private _canvasBottomL;
        private _canvasBottomR;
        private _canvasTopL;
        private _canvasTopR;
        private _container;
        private _focusSink1;
        private _focusSink2;
        private _groupingPanel;
        private _headerColsL;
        private _headerColsR;
        private _headerRowColsL;
        private _headerRowColsR;
        private _headerRowSpacerL;
        private _headerRowSpacerR;
        private _footerRowColsL;
        private _footerRowR;
        private _footerRowSpacerL;
        private _footerRowSpacerR;
        private _paneBottomL;
        private _paneBottomR;
        private _paneHeaderL;
        private _paneHeaderR;
        private _paneTopL;
        private _paneTopR;
        private _scrollContainerX;
        private _scrollContainerY;
        private _topPanelL;
        private _topPanelR;
        private _viewportBottomL;
        private _viewportBottomR;
        private _viewportTopL;
        private _viewportTopR;
        readonly onActiveCellChanged: Event<ArgsCell, IEventData>;
        readonly onActiveCellPositionChanged: Event<ArgsGrid, IEventData>;
        readonly onAddNewRow: Event<ArgsAddNewRow, IEventData>;
        readonly onBeforeCellEditorDestroy: Event<ArgsEditorDestroy, IEventData>;
        readonly onBeforeDestroy: Event<ArgsGrid, IEventData>;
        readonly onBeforeEditCell: Event<ArgsCellEdit, IEventData>;
        readonly onBeforeFooterRowCellDestroy: Event<ArgsColumnNode, IEventData>;
        readonly onBeforeHeaderCellDestroy: Event<ArgsColumnNode, IEventData>;
        readonly onBeforeHeaderRowCellDestroy: Event<ArgsColumnNode, IEventData>;
        readonly onCellChange: Event<ArgsCellChange, IEventData>;
        readonly onCellCssStylesChanged: Event<ArgsCssStyle, IEventData>;
        readonly onClick: Event<ArgsCell, JQueryMouseEventObject>;
        readonly onColumnsReordered: Event<ArgsGrid, IEventData>;
        readonly onColumnsResized: Event<ArgsGrid, IEventData>;
        readonly onContextMenu: Event<ArgsGrid, JQueryEventObject>;
        readonly onDblClick: Event<ArgsCell, JQueryMouseEventObject>;
        readonly onDrag: Event<ArgsGrid, JQueryEventObject>;
        readonly onDragEnd: Event<ArgsGrid, JQueryEventObject>;
        readonly onDragInit: Event<ArgsGrid, JQueryEventObject>;
        readonly onDragStart: Event<ArgsGrid, JQueryEventObject>;
        readonly onFooterRowCellRendered: Event<ArgsColumnNode, IEventData>;
        readonly onHeaderCellRendered: Event<ArgsColumnNode, IEventData>;
        readonly onHeaderClick: Event<ArgsColumn, IEventData>;
        readonly onHeaderContextMenu: Event<ArgsColumn, IEventData>;
        readonly onHeaderMouseEnter: Event<ArgsColumn, JQueryMouseEventObject>;
        readonly onHeaderMouseLeave: Event<ArgsColumn, IEventData>;
        readonly onHeaderRowCellRendered: Event<ArgsColumnNode, IEventData>;
        readonly onKeyDown: Event<ArgsCell, JQueryKeyEventObject>;
        readonly onMouseEnter: Event<ArgsGrid, JQueryMouseEventObject>;
        readonly onMouseLeave: Event<ArgsGrid, JQueryMouseEventObject>;
        readonly onScroll: Event<ArgsScroll, IEventData>;
        readonly onSelectedRowsChanged: Event<ArgsSelectedRowsChange, IEventData>;
        readonly onSort: Event<ArgsSort, IEventData>;
        readonly onValidationError: Event<ArgsValidationError, IEventData>;
        readonly onViewportChanged: Event<ArgsGrid, IEventData>;
        constructor(container: JQuery | HTMLElement, data: any, columns: Column<TItem>[], options: GridOptions<TItem>);
        init(): void;
        private hasFrozenColumns;
        registerPlugin(plugin: IPlugin): void;
        unregisterPlugin(plugin: IPlugin): void;
        getPluginByName(name: string): IPlugin;
        setSelectionModel(model: SelectionModel): void;
        getScrollBarDimensions(): {
            width: number;
            height: number;
        };
        getDisplayedScrollbarDimensions(): {
            width: number;
            height: number;
        };
        getAbsoluteColumnMinWidth(): number;
        getSelectionModel(): SelectionModel;
        getCanvasNode(): HTMLDivElement;
        getCanvases(): JQuery;
        getActiveCanvasNode(e?: IEventData): HTMLElement;
        setActiveCanvasNode(e?: IEventData): void;
        getViewportNode(): HTMLDivElement;
        private getViewports;
        getActiveViewportNode(e?: IEventData): HTMLElement;
        setActiveViewportNode(e?: IEventData): void;
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
        getHeaderRowColumn(columnId: string): HTMLElement;
        getFooterRow(): HTMLDivElement;
        getFooterRowColumn(columnId: string): HTMLElement;
        private createColumnFooters;
        private formatGroupTotal;
        private groupTotalText;
        private groupTotalsFormatter;
        private createColumnHeaders;
        private setupColumnSort;
        private setupColumnReorder;
        private setupColumnResize;
        private getVBoxDelta;
        private adjustFrozenRowOption;
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
        getEditController(): EditController;
        getColumnIndex(id: string): number;
        getInitialColumnIndex(id: string): number;
        autosizeColumns(): void;
        private applyColumnHeaderWidths;
        private applyColumnWidths;
        setSortColumn(columnId: string, ascending: boolean): void;
        setSortColumns(cols: ColumnSort[]): void;
        getSortColumns(): ColumnSort[];
        private handleSelectedRangesChanged;
        getColumns(): Column<TItem>[];
        getInitialColumns(): Column<TItem>[];
        private updateViewColLeftRight;
        private setInitialCols;
        setColumns(columns: Column<TItem>[]): void;
        getOptions(): GridOptions<TItem>;
        setOptions(args: GridOptions<TItem>, suppressRender?: boolean, suppressColumnSet?: boolean, suppressSetOverflow?: boolean): void;
        private validateAndEnforceOptions;
        private viewOnRowCountChanged;
        private viewOnRowsChanged;
        private viewOnDataChanged;
        private bindToData;
        private unbindFromData;
        setData(newData: any, scrollToTop?: boolean): void;
        getData(): any;
        getDataLength(): number;
        private getDataLengthIncludingAddNew;
        getDataItem(i: number): TItem;
        getTopPanel(): HTMLDivElement;
        setTopPanelVisibility(visible: boolean): void;
        setColumnHeaderVisibility(visible: boolean, animate?: boolean): void;
        setFooterRowVisibility(visible: boolean): void;
        setGroupingPanelVisibility(visible: boolean): void;
        setHeaderRowVisibility(visible: boolean): void;
        getContainerNode(): HTMLElement;
        getUID(): string;
        private getRowTop;
        private getRowFromPosition;
        private scrollTo;
        getFormatter(row: number, column: Column<TItem>): ColumnFormatter<TItem>;
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
        applyFormatResultToCellNode(fmtResult: FormatterResult | string, cellNode: HTMLElement): void;
        updateCell(row: number, cell: number): void;
        updateRow(row: number): void;
        private getViewportHeight;
        private getViewportWidth;
        resizeCanvas(): void;
        updatePagingStatusFromView(pagingInfo: {
            pageSize: number;
            pageNum: number;
            totalPages: number;
        }): void;
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
        scrollColumnIntoView(cell: number): void;
        internalScrollColumnIntoView(left: number, right: number): void;
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
        getCellEditor(): Editor;
        getActiveCell(): RowCell;
        getActiveCellNode(): HTMLElement;
        scrollActiveCellIntoView(): void;
        scrollRowIntoView(row: number, doPaging?: boolean): void;
        scrollRowToTop(row: number): void;
        private scrollPage;
        navigatePageDown(): void;
        navigatePageUp(): void;
        navigateTop(): void;
        navigateBottom(): void;
        navigateToRow(row: number): boolean;
        getColspan(row: number, cell: number): number;
        private findFirstFocusableCell;
        private findLastFocusableCell;
        private gotoRight;
        private gotoLeft;
        private gotoDown;
        private gotoUp;
        private gotoNext;
        private gotoPrev;
        private gotoRowStart;
        private gotoRowEnd;
        navigateRight(): boolean;
        navigateLeft(): boolean;
        navigateDown(): boolean;
        navigateUp(): boolean;
        navigateNext(): boolean;
        navigatePrev(): boolean;
        navigateRowStart(): boolean;
        navigateRowEnd(): boolean;
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
        getSelectedRows(): number[];
        setSelectedRows(rows: number[]): void;
    }

    export interface Column<TItem = any> {
        asyncPostRender?: AsyncPostRender<TItem>;
        asyncPostRenderCleanup?: AsyncPostCleanup<TItem>;
        behavior?: any;
        cannotTriggerInsert?: boolean;
        cssClass?: string;
        defaultSortAsc?: boolean;
        editor?: Editor;
        field: string;
        frozen?: boolean;
        focusable?: boolean;
        footerCssClass?: string;
        formatter?: ColumnFormatter<TItem>;
        groupTotalsFormatter?: (p1?: GroupTotals<TItem>, p2?: Column<TItem>, grid?: Grid<TItem>) => string;
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
        sortOrder?: number;
        toolTip?: string;
        validator?: (value: any) => ValidationResult;
        visible?: boolean;
        width?: number;
    }
    export interface ColumnMetadata<TItem = any> {
        colspan: number | '*';
        formatter?: ColumnFormatter<TItem>;
    }
    export interface ColumnSort {
        columnId: string;
        sortAsc?: boolean;
    }
    export interface ItemMetadata<TItem = any> {
        columns?: {
            [key: string]: ColumnMetadata<TItem>;
        };
        formatter?: ColumnFormatter<TItem>;
    }
}
