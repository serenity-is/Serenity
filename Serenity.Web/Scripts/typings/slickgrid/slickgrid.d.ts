declare namespace Slick.Data {
    class GroupItemMetadataProvider {
        constructor();
    }
}
declare namespace Slick.Controls {
    class ColumnPicker {
        constructor(columns: Slick.Column[], grid: Slick.Grid, options?);
        init(): void;
        destroy(): void;
        handleHeaderContextMenu(e, args): void;
        updateColumnOrder(): void;
        updateColumn(e): void;
        getAllColumns(): Column[];
    }
    class Pager {
        constructor(dataView, grid, $container);
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
    class Event {
        subscribe(handler: (p1: any, p2?: any) => void): void;
        subscribe(handler: (p1: any, p2?: any) => any): void;
        unsubscribe(handler: (p1: any, p2?: any) => void): void;
        notify(p1?: any, p2?: any, p3?: any): void;
        clear(): void;
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
    interface SlickRangeInfo {
        top: number;
        bottom: number;
        leftPx: number;
        rightPx: number;
    }
    class EventData {
        constructor();
    }
    type AsyncPostRender = (cellNode: any, row: number, item: any, column: Slick.Column, clean?: boolean) => void;
    type CancellableViewCallback<TEntity> = (view: Slick.RemoteView<TEntity>) => boolean;
    type ColumnFormatter = (row: number, cell: number, value: any, column: Slick.Column, item: any) => string;
    type RemoteViewAjaxCallback<TEntity> = (view: Slick.RemoteView<TEntity>, options: JQueryAjaxSettings) => boolean;
    type RemoteViewFilter<TEntity> = (item: TEntity, view: Slick.RemoteView<TEntity>) => boolean;
    type RemoteViewProcessCallback<TEntity> = (data: Serenity.ListResponse<TEntity>, view: Slick.RemoteView<TEntity>) => Serenity.ListResponse<TEntity>;
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
    class RowMoveManager {
        constructor(options: Slick.RowMoveManagerOptions);
        get_onBeforeMoveRows(): Slick.Event;
        get_onMoveRows(): Slick.Event;
    }
    class RowSelectionModel {
    }
    class AutoTooltips {
        constructor(options: Slick.AutoTooltipsOptions);
    }
    interface AutoTooltipsOptions {
        enableForHeaderCells: boolean;
        enableForCells: boolean;
        maxToolTipLength: number;
    }
    interface GroupInfo<TItem> {
        getter?: any;
        formatter?: (p1: Slick.Group<TItem>) => string;
        comparer?: (a: Slick.Group<TItem>, b: Slick.Group<TItem>) => number;
        aggregators?: any[];
        aggregateCollapsed?: boolean;
        lazyTotalsCalculation?: boolean;
    }
    interface RowCell {
        row: number;
        cell: number;
    }
    interface RowMoveManagerOptions {
        cancelEditOnDrag: boolean;
    }
    interface SummaryOptions {
        aggregators: any[];
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
        groups: Group<TEntity>;
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
        multiColumnSort?: boolean;
        multiSelect?: boolean;
        rowHeight?: number;
        selectedCellCssClass?: string;
        showHeaderRow?: boolean;
        showFooterRow?: boolean;
        syncColumnCellResize?: boolean;
        topPanelHeight?: number;
    }
    interface RemoteView<TEntity> {
        constructor(options: RemoteViewOptions): void;
        onSubmit: Slick.CancellableViewCallback<TEntity>;
        onAjaxCall: Slick.RemoteViewAjaxCallback<TEntity>;
        onProcessData: Slick.RemoteViewProcessCallback<TEntity>;
        addData(data: Serenity.ListResponse<TEntity>): void;
        deleteItem(id: any): void;
        getItems(): TEntity[];
        setFilter(filter: RemoteViewFilter<TEntity>): void;
        setItems(items: any[], fullReset: boolean): void;
        getItemById(id: any): TEntity;
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
        refresh(): void;
        getItem(row: number): any;
        params: any;
        sortBy: string[];
        url: string;
    }
    interface RemoteViewOptions {
        autoLoad?: boolean;
        idField?: string;
        contentType?: string;
        dataType?: string;
        filter?: any;
        params?: any;
        onSubmit?: Slick.Event;
        url?: string;
        sortBy?: any;
        rowsPerPage?: number;
        seekToPage?: number;
        onProcessData?: Slick.Event;
        method?: string;
        getItemMetadata?: (p1?: any, p2?: number) => any;
        errorMsg?: string;
    }
    interface ColumnSort {
        columnId?: string;
        sortAsc?: boolean;
    }
    interface RangeInfo {
        top?: number;
        bottom?: number;
        leftPx?: number;
        rightPx?: number;
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
        setColumns(columns: Column[]): void;
        setSortColumn(columnId: string, ascending: boolean): void;
        setSortColumns(cols: Slick.ColumnSort[]): void;
        updateColumnHeader(columnId: string, title: string, toolTip: string): void;
        addCellCssStyles(key: string, hash: any): void;
        canCellBeActive(row: number, col: number): boolean;
        canCellBeSelected(row: number, col: number): boolean;
        editActiveCell(editor: Function): void;
        flashCell(row: number, cell: number, speed: number): void;
        getActiveCell(): Slick.RowCell;
        getActiveCellNode(): any;
        getActiveCellPosition(): Slick.PositionInfo;
        getCellCssStyles(key: string): any;
        getCellEditor(): any;
        getCellFromEvent(e: any): Slick.RowCell;
        getCellFromPoint(x: number, y: number): Slick.RowCell;
        getCellNode(row: number, cell: number): any;
        getCellNodeBox(row: number, cell: number): Slick.PositionInfo;
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
        getGridPosition(): Slick.PositionInfo;
        getRenderedRange(viewportTop: number, viewportLeft: number): Slick.RangeInfo;
        getViewport(viewportTop: number, viewportLeft: number): Slick.RangeInfo;
        getViewport(): Slick.RangeInfo;
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
        getHeaderRow(): any;
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
}
declare namespace Slick.Data {
}
declare namespace Slick {
    class RemoteView<TEntity> {
        constructor(options: any);
    }
}
declare namespace Slick.Aggregators {
    function Avg(field: string): void;
    function WeightedAvg(field: string, weightedField: string): void;
    function Min(field: string): void;
    function Max(field: string): void;
    function Sum(field: string): void;
}
