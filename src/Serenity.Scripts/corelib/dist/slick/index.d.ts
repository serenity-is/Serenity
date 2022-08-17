/// <reference types="jquery" />
import { GroupTotals, Column, Grid, Group, SelectionModel, Range, Event } from '@serenity-is/sleekgrid';

declare namespace Aggregators {
    function Avg(field: string): void;
    function WeightedAvg(field: string, weightedField: string): void;
    function Min(field: string): void;
    function Max(field: string): void;
    function Sum(field: string): void;
}
declare namespace AggregateFormatting {
    function formatMarkup<TItem = any>(totals: GroupTotals, column: Column<TItem>, aggType: string): string;
    function formatValue(column: Column, value: number): string;
    function groupTotalsFormatter<TItem = any>(totals: GroupTotals, column: Column<TItem>): string;
}

interface ServiceError {
    Code?: string;
    Arguments?: string;
    Message?: string;
    Details?: string;
    ErrorId?: string;
}
interface ServiceResponse {
    Error?: ServiceError;
}
interface ListResponse<TEntity> extends ServiceResponse {
    Entities?: TEntity[];
    Values?: any[];
    TotalCount?: number;
    Skip?: number;
    Take?: number;
}

declare global {
    namespace Q {
        interface Lookup<TItem> {
            items: TItem[];
            itemById: {
                [key: string]: TItem;
            };
            idField: string;
            parentIdField: string;
            textField: string;
            textFormatter: (item: TItem) => string;
        }
    }
}

interface PropertyItem {
    name?: string;
    title?: string;
    hint?: string;
    placeholder?: string;
    editorType?: string;
    editorParams?: any;
    category?: string;
    collapsible?: boolean;
    collapsed?: boolean;
    tab?: string;
    cssClass?: string;
    headerCssClass?: string;
    formCssClass?: string;
    maxLength?: number;
    required?: boolean;
    insertable?: boolean;
    insertPermission?: string;
    hideOnInsert?: boolean;
    updatable?: boolean;
    updatePermission?: string;
    hideOnUpdate?: boolean;
    readOnly?: boolean;
    readPermission?: string;
    oneWay?: boolean;
    defaultValue?: any;
    localizable?: boolean;
    visible?: boolean;
    allowHide?: boolean;
    formatterType?: string;
    formatterParams?: any;
    displayFormat?: string;
    alignment?: string;
    width?: number;
    widthSet?: boolean;
    minWidth?: number;
    maxWidth?: number;
    labelWidth?: string;
    resizable?: boolean;
    sortable?: boolean;
    sortOrder?: number;
    groupOrder?: number;
    summaryType?: SummaryType;
    editLink?: boolean;
    editLinkItemType?: string;
    editLinkIdField?: string;
    editLinkCssClass?: string;
    filteringType?: string;
    filteringParams?: any;
    filteringIdField?: string;
    notFilterable?: boolean;
    filterOnly?: boolean;
    quickFilter?: boolean;
    quickFilterParams?: any;
    quickFilterSeparator?: boolean;
    quickFilterCssClass?: string;
}
declare enum SummaryType {
    Disabled = -1,
    None = 0,
    Sum = 1,
    Avg = 2,
    Min = 3,
    Max = 4
}

declare type Format<TItem = any> = (ctx: FormatterContext<TItem>) => string;
declare module "@serenity-is/sleekgrid" {
    interface Column<TItem = any> {
        referencedFields?: string[];
        format?: Format<TItem>;
        sourceItem?: PropertyItem;
    }
}
interface FormatterContext<TItem = any> {
    addAttrs?: {
        [key: string]: string;
    };
    addClass?: string;
    cell?: number;
    column?: Column<TItem>;
    grid?: Grid<TItem>;
    item?: TItem;
    row?: number;
    toolTip?: string;
    value?: any;
}
interface Formatter {
    format(ctx: FormatterContext): string;
}
interface GroupInfo<TItem> {
    getter?: any;
    formatter?: (p1: Group<TItem>) => string;
    comparer?: (a: Group<TItem>, b: Group<TItem>) => number;
    aggregators?: any[];
    aggregateCollapsed?: boolean;
    lazyTotalsCalculation?: boolean;
}
interface PagerOptions {
    view?: any;
    showRowsPerPage?: boolean;
    rowsPerPage?: number;
    rowsPerPageOptions?: number[];
    onChangePage?: (newPage: number) => void;
    onRowsPerPageChange?: (n: number) => void;
}
declare class RowSelectionModel implements SelectionModel {
    init(grid: Grid): void;
    destroy?: () => void;
    setSelectedRanges(ranges: Range[]): void;
    onSelectedRangesChanged: Event<Range[]>;
    refreshSelections?(): void;
}
interface SummaryOptions {
    aggregators: any[];
}
interface PagingOptions {
    rowsPerPage?: number;
    page?: number;
}

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
    groupItemMetadataProvider?: Slick.Data.GroupItemMetadataProvider;
    onAjaxCall?: RemoteViewAjaxCallback<any>;
    getItemMetadata?: (p1?: any, p2?: number) => any;
    errorMsg?: string;
}
interface PagingInfo {
    rowsPerPage: number;
    page: number;
    totalCount: number;
    loading: boolean;
    error: string;
    dataView: RemoteView<any>;
}
declare type CancellableViewCallback<TEntity> = (view: RemoteView<TEntity>) => boolean | void;
declare type RemoteViewAjaxCallback<TEntity> = (view: RemoteView<TEntity>, options: JQueryAjaxSettings) => boolean | void;
declare type RemoteViewFilter<TEntity> = (item: TEntity, view: RemoteView<TEntity>) => boolean;
declare type RemoteViewProcessCallback<TEntity> = (data: ListResponse<TEntity>, view: RemoteView<TEntity>) => ListResponse<TEntity>;
interface RemoteView<TEntity> {
    onSubmit: CancellableViewCallback<TEntity>;
    onDataChanged: Event;
    onDataLoading: Event;
    onDataLoaded: Event;
    onPagingInfoChanged: Event;
    onRowCountChanged: Event;
    onRowsChanged: Event;
    onRowsOrCountChanged: Event;
    getPagingInfo(): PagingInfo;
    onGroupExpanded: Event;
    onGroupCollapsed: Event;
    onAjaxCall: RemoteViewAjaxCallback<TEntity>;
    onProcessData: RemoteViewProcessCallback<TEntity>;
    addData(data: ListResponse<TEntity>): void;
    beginUpdate(): void;
    endUpdate(): void;
    deleteItem(id: any): void;
    getItems(): TEntity[];
    setFilter(filter: RemoteViewFilter<TEntity>): void;
    getFilter(): RemoteViewFilter<TEntity>;
    getFilteredItems(): any;
    fastSort: any;
    setItems(items: any[], newIdProperty?: boolean | string): void;
    getIdPropertyName(): string;
    getItemById(id: any): TEntity;
    getGrandTotals(): any;
    getGrouping(): GroupInfo<TEntity>[];
    getGroups(): any[];
    getRowById(id: any): number;
    getRowByItem(item: any): number;
    getRows(): any[];
    mapItemsToRows(itemArray: any[]): any[];
    mapRowsToIds(rowArray: number[]): any[];
    mapIdsToRows(idAray: any[]): number[];
    setFilterArgs(args: any): void;
    setRefreshHints(hints: any[]): void;
    insertItem(insertBefore: number, item: any): void;
    sortedAddItem(item: any): void;
    sortedUpdateItem(id: any, item: any): void;
    syncGridSelection(grid: any, preserveHidden?: boolean, preserveHiddenOnSelectionChange?: boolean): void;
    syncGridCellCssStyles(grid: any, key: string): void;
    getItemMetadata(i: number): any;
    updateItem(id: any, item: TEntity): void;
    addItem(item: TEntity): void;
    getIdxById(id: any): any;
    getItemByIdx(index: number): any;
    setGrouping(groupInfo: GroupInfo<TEntity>[]): void;
    collapseAllGroups(level: number): void;
    expandAllGroups(level: number): void;
    expandGroup(keys: any[]): void;
    collapseGroup(keys: any[]): void;
    setSummaryOptions(options: SummaryOptions): void;
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
    setLocalSort(value: boolean): void;
    sort(comparer?: (a: any, b: any) => number, ascending?: boolean): void;
    reSort(): void;
    sortBy: string[];
    url: string;
    method: string;
    idField: string;
    seekToPage?: number;
}
declare class RemoteView<TEntity> {
    constructor(options: RemoteViewOptions);
}

export { AggregateFormatting, Aggregators, CancellableViewCallback, Format, Formatter, FormatterContext, GroupInfo, PagerOptions, PagingInfo, PagingOptions, RemoteView, RemoteViewAjaxCallback, RemoteViewFilter, RemoteViewOptions, RemoteViewProcessCallback, RowSelectionModel, SummaryOptions };
