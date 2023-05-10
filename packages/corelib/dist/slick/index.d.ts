/// <reference types="jquery" />
import { GroupTotals, Column, FormatterContext, Group, GroupItemMetadataProvider, EventEmitter } from '@serenity-is/sleekgrid';
import { PropertyItem, ListResponse } from '@serenity-is/corelib/q';

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

type Format<TItem = any> = (ctx: FormatterContext<TItem>) => string;
declare module "@serenity-is/sleekgrid" {
    interface Column<TItem = any> {
        referencedFields?: string[];
        sourceItem?: PropertyItem;
    }
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
    groupItemMetadataProvider?: GroupItemMetadataProvider;
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
type CancellableViewCallback<TEntity> = (view: RemoteView<TEntity>) => boolean | void;
type RemoteViewAjaxCallback<TEntity> = (view: RemoteView<TEntity>, options: JQueryAjaxSettings) => boolean | void;
type RemoteViewFilter<TEntity> = (item: TEntity, view: RemoteView<TEntity>) => boolean;
type RemoteViewProcessCallback<TEntity> = (data: ListResponse<TEntity>, view: RemoteView<TEntity>) => ListResponse<TEntity>;
interface RemoteView<TEntity> {
    onSubmit: CancellableViewCallback<TEntity>;
    onDataChanged: EventEmitter;
    onDataLoading: EventEmitter;
    onDataLoaded: EventEmitter;
    onPagingInfoChanged: EventEmitter;
    onRowCountChanged: EventEmitter;
    onRowsChanged: EventEmitter;
    onRowsOrCountChanged: EventEmitter;
    getPagingInfo(): PagingInfo;
    onGroupExpanded: EventEmitter;
    onGroupCollapsed: EventEmitter;
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
    getGroupItemMetadataProvider(): GroupItemMetadataProvider;
    setGroupItemMetadataProvider(value: GroupItemMetadataProvider): void;
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

export { AggregateFormatting, Aggregators, CancellableViewCallback, Format, Formatter, GroupInfo, PagerOptions, PagingInfo, PagingOptions, RemoteView, RemoteViewAjaxCallback, RemoteViewFilter, RemoteViewOptions, RemoteViewProcessCallback, SummaryOptions };
