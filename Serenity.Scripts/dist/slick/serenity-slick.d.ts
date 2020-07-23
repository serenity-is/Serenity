/// <reference types="jquery" />
declare namespace Slick {
    type Handler<TArgs> = (e: JQueryEventObject, args: TArgs) => void;
    interface IEventData {
        isPropagationStopped(): boolean;
        isImmediatePropagationStopped(): boolean;
    }
    class EventData {
        constructor();
        isPropagationStopped(): boolean;
        isImmediatePropagationStopped(): boolean;
    }
    class Event<TArgs = any> {
        subscribe(handler: Handler<TArgs>): void;
        unsubscribe(handler: Handler<TArgs>): void;
        notify(args?: TArgs, e?: IEventData, scope?: any): void;
        clear(): void;
    }
    class EventHandler<TArgs = any> {
        subscribe<TArgs>(event: Event<TArgs>, handler: Handler<TArgs>): EventHandler<TArgs>;
        unsubscribe<TArgs>(event: Event<TArgs>, handler: Handler<TArgs>): EventHandler<TArgs>;
        unsubscribeAll(): EventHandler<TArgs>;
    }
}
declare namespace Slick.Data {
    class GroupItemMetadataProvider implements Slick.GroupItemMetadataProvider {
        constructor();
        getGroupRowMetadata(item: any): any;
        getTotalsRowMetadata(item: any): any;
    }
}
declare namespace Slick {
    interface GroupItemMetadataProvider {
        getGroupRowMetadata(item: any): any;
        getTotalsRowMetadata(item: any): any;
    }
    class Group<TEntity> {
        isGroup: boolean;
        level: number;
        count: number;
        value: any;
        title: string;
        collapsed: boolean;
        totals: any;
        rows: any;
        groups: Group<TEntity>[];
        groupingKey: string;
    }
    class GroupTotals<TEntity> {
        isGroupTotals: boolean;
        group: Group<TEntity>;
        initialized: boolean;
        sum: any;
        avg: any;
        min: any;
        max: any;
    }
    interface GroupInfo<TItem> {
        getter?: any;
        formatter?: (p1: Slick.Group<TItem>) => string;
        comparer?: (a: Slick.Group<TItem>, b: Slick.Group<TItem>) => number;
        aggregators?: any[];
        aggregateCollapsed?: boolean;
        lazyTotalsCalculation?: boolean;
    }
}
declare namespace Slick {
    interface FormatterContext {
        row?: number;
        cell?: number;
        value?: any;
        column?: any;
        item?: any;
    }
    interface Formatter {
        format(ctx: FormatterContext): string;
    }
    type Format = (ctx: Slick.FormatterContext) => string;
    type AsyncPostRender = (cellNode: any, row: number, item: any, column: Slick.Column, clean?: boolean) => void;
    type ColumnFormatter = (row: number, cell: number, value: any, column: Slick.Column, item: any) => string;
}
declare namespace Slick {
    interface Column {
        asyncPostRender?: Slick.AsyncPostRender;
        behavior?: any;
        cannotTriggerInsert?: boolean;
        cssClass?: string;
        defaultSortAsc?: boolean;
        editor?: Function;
        field: string;
        focusable?: boolean;
        formatter?: Slick.ColumnFormatter;
        headerCssClass?: string;
        id?: string;
        maxWidth?: any;
        minWidth?: number;
        name?: string;
        rerenderOnResize?: boolean;
        resizable?: boolean;
        selectable?: boolean;
        sortable?: boolean;
        toolTip?: string;
        width?: number;
        format?: (ctx: Slick.FormatterContext) => string;
        referencedFields?: string[];
        sourceItem?: Serenity.PropertyItem;
        sortOrder?: number;
        groupTotalsFormatter?: (p1?: GroupTotals<any>, p2?: Column) => string;
        visible?: boolean;
    }
}
declare namespace Slick {
    interface GridOptions {
        asyncEditorLoading?: boolean;
        asyncEditorLoadDelay?: number;
        asyncPostRenderDelay?: number;
        asyncPostRenderCleanupDelay?: number;
        autoEdit?: boolean;
        autoHeight?: boolean;
        cellFlashingCssClass?: string;
        cellHighlightCssClass?: string;
        dataItemColumnValueExtractor?: () => void;
        groupingPanel?: boolean;
        groupingPanelHeight?: number;
        setGroupingPanelVisibility?: (value: boolean) => void;
        defaultColumnWidth?: number;
        defaultFormatter?: () => void;
        editable?: boolean;
        editCommandHandler?: () => void;
        editorFactory?: () => void;
        editorLock?: any;
        enableAddRow?: boolean;
        enableAsyncPostRender?: boolean;
        enableAsyncPostRenderCleanup?: boolean;
        enableCellRangeSelection?: boolean;
        enableCellNavigation?: boolean;
        enableColumnReorder?: boolean;
        enableRowReordering?: boolean;
        enableTextSelectionOnCells?: boolean;
        explicitInitialization?: boolean;
        forceFitColumns?: boolean;
        forceSyncScrolling?: boolean;
        formatterFactory?: () => void;
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
        showHeaderRow?: boolean;
        showFooterRow?: boolean;
        syncColumnCellResize?: boolean;
        topPanelHeight?: number;
    }
}
declare namespace Slick {
    class Grid {
        constructor(container: JQuery, data: any, columns: Column[], options: GridOptions);
    }
    interface ColumnSort {
        columnId?: string;
        sortAsc?: boolean;
    }
    interface RowCell {
        row: number;
        cell: number;
    }
    interface PositionInfo {
        bottom: number;
        height: number;
        left: number;
        right: number;
        top: number;
        visible: boolean;
        width: number;
    }
    interface RangeInfo {
        top?: number;
        bottom?: number;
        leftPx?: number;
        rightPx?: number;
    }
    class RowSelectionModel {
    }
    interface Grid {
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
        onScroll?: Event;
        onSort?: Event;
        onHeaderContextMenu?: Event;
        onHeaderClick?: Event;
        onMouseEnter?: Event;
        onMouseLeave?: Event;
        onClick?: Event;
        onDblClick?: Event;
        onContextMenu?: Event;
        onKeyDown?: Event;
        onAddNewRow?: Event;
        onValidationError?: Event;
        onViewportChanged?: Event;
        onColumnsReordered?: Event;
        onColumnsResized?: Event;
        onCellChange?: Event;
        onBeforeEditCell?: Event;
        onBeforeCellEditorDestroy?: Event;
        onHeaderCellRendered?: Event;
        onBeforeHeaderCellDestroy?: Event;
        onBeforeDestroy?: Event;
        onActiveCellChanged?: Event;
        onActiveCellPositionChanged?: Event;
        onDragInit?: Event;
        onDragStart?: Event;
        onDrag?: Event;
        onDragEnd?: Event;
        onSelectedRowsChanged?: Event;
        onCellCssStylesChanged?: Event;
    }
}
declare namespace Slick.Aggregators {
    function Avg(field: string): void;
    function WeightedAvg(field: string, weightedField: string): void;
    function Min(field: string): void;
    function Max(field: string): void;
    function Sum(field: string): void;
}
declare namespace Slick {
    interface RemoteViewOptions {
        autoLoad?: boolean;
        idField?: string;
        contentType?: string;
        dataType?: string;
        filter?: any;
        params?: any;
        onSubmit?: CancellableViewCallback<any>;
        url?: string;
        localSort?: boolean;
        sortBy?: any;
        rowsPerPage?: number;
        seekToPage?: number;
        onProcessData?: RemoteViewProcessCallback<any>;
        method?: string;
        inlineFilters?: boolean;
        groupItemMetadataProvider?: Slick.GroupItemMetadataProvider;
        onAjaxCall?: Slick.RemoteViewAjaxCallback<any>;
        getItemMetadata?: (p1?: any, p2?: number) => any;
        errorMsg?: string;
    }
    class RemoteView<TEntity> {
        constructor(options: RemoteViewOptions);
    }
}
declare namespace Slick {
    type CancellableViewCallback<TEntity> = (view: Slick.RemoteView<TEntity>) => boolean | void;
    type RemoteViewAjaxCallback<TEntity> = (view: Slick.RemoteView<TEntity>, options: JQueryAjaxSettings) => boolean | void;
    type RemoteViewFilter<TEntity> = (item: TEntity, view: Slick.RemoteView<TEntity>) => boolean;
    type RemoteViewProcessCallback<TEntity> = (data: Serenity.ListResponse<TEntity>, view: Slick.RemoteView<TEntity>) => Serenity.ListResponse<TEntity>;
    interface SummaryOptions {
        aggregators: any[];
    }
    interface PagingOptions {
        rowsPerPage?: number;
        page?: number;
    }
    interface PagingInfo {
        rowsPerPage: number;
        page: number;
        totalCount: number;
        loading: boolean;
        error: string;
        dataView: RemoteView<any>;
    }
    interface RemoteView<TEntity> {
        constructor(options: RemoteViewOptions): void;
        onSubmit: Slick.CancellableViewCallback<TEntity>;
        onDataChanged: Slick.Event;
        onDataLoaded: Slick.Event;
        onPagingInfoChanged: Slick.Event;
        getPagingInfo(): PagingInfo;
        onGroupExpanded: Slick.Event;
        onGroupCollapsed: Slick.Event;
        onAjaxCall: Slick.RemoteViewAjaxCallback<TEntity>;
        onProcessData: Slick.RemoteViewProcessCallback<TEntity>;
        addData(data: Serenity.ListResponse<TEntity>): void;
        beginUpdate(): void;
        endUpdate(): void;
        deleteItem(id: any): void;
        getItems(): TEntity[];
        setFilter(filter: RemoteViewFilter<TEntity>): void;
        setItems(items: any[], newIdProperty?: boolean | string): void;
        getIdPropertyName(): string;
        getItemById(id: any): TEntity;
        getRowById(id: any): number;
        updateItem(id: any, item: TEntity): void;
        addItem(item: TEntity): void;
        getIdxById(id: any): any;
        getItemByIdx(index: number): any;
        setGrouping(groupInfo: Slick.GroupInfo<TEntity>[]): void;
        collapseAllGroups(level: number): void;
        expandAllGroups(level: number): void;
        expandGroup(keys: any[]): void;
        collapseGroup(keys: any[]): void;
        setSummaryOptions(options: Slick.SummaryOptions): void;
        setPagingOptions(options: PagingOptions): void;
        refresh(): void;
        populate(): void;
        populateLock(): void;
        populateUnlock(): void;
        getItem(row: number): any;
        getLength(): number;
        rowsPerPage: number;
        errormsg: string;
        params: any;
        getLocalSort(): boolean;
        setLocalSort(value: boolean): boolean;
        sort(comparer?: (a: any, b: any) => number, ascending?: boolean): void;
        reSort(): void;
        sortBy: string[];
        url: string;
        method: string;
        idField: string;
        seekToPage?: number;
        loading: boolean;
    }
}
declare namespace Slick {
    class AutoTooltips {
        constructor(options: Slick.AutoTooltipsOptions);
    }
    interface AutoTooltipsOptions {
        enableForHeaderCells?: boolean;
        enableForCells?: boolean;
        maxToolTipLength?: number;
    }
}
declare namespace Slick {
    class RowMoveManager {
        constructor(options: Slick.RowMoveManagerOptions);
        onBeforeMoveRows: Slick.Event;
        onMoveRows: Slick.Event;
    }
    interface RowMoveManagerOptions {
        cancelEditOnDrag: boolean;
    }
}
