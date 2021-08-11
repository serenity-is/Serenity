declare namespace Serenity {
        
    export type Constructor<T> = new (...args: any[]) => T;

    export interface PropertyItem {
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

    export interface Select2Item {
        id: string;
        text: string;
        source?: any;
        disabled?: boolean;
    }
        
    export interface ServiceError {
        Code?: string;
        Arguments?: string;
        Message?: string;
        Details?: string;
        ErrorId?: string;
    }               
          
    export interface ServiceResponse {
        Error?: Serenity.ServiceError;
    }
        
    export interface ServiceRequest {
    }
        
    export interface SaveRequest<TEntity> extends ServiceRequest {
        EntityId?: any;
        Entity?: TEntity;
        Localizations?: any;
    }
        
    export interface SaveRequestWithAttachment<TEntity> extends SaveRequest<TEntity> {
        Attachments?: any[];
    }
        
    export interface SaveResponse extends ServiceResponse {
        EntityId?: any;
    }
        
    export interface SaveWithLocalizationRequest<TEntity> extends SaveRequest<TEntity> {
        Localizations?: { [key: string]: TEntity };
    }
        
    export interface DeleteRequest extends ServiceRequest {
        EntityId?: any;
    }
        
    export interface DeleteResponse extends ServiceResponse {
    }
        
    export interface UndeleteRequest extends ServiceRequest {
        EntityId?: any;
    }
        
    export interface UndeleteResponse extends ServiceResponse {
    }
        
    export const enum ColumnSelection {
        List = 0,
        KeyOnly = 1,
        Details = 2
    }
        
    export const enum RetrieveColumnSelection {
        details = 0,
        keyOnly = 1,
        list = 2
    }
        
    export interface ListRequest extends ServiceRequest {
        Skip?: number;
        Take?: number;
        Sort?: string[];
        ContainsText?: string;
        ContainsField?: string;
        Criteria?: any[];
        EqualityFilter?: any;
        IncludeDeleted?: boolean;
        ExcludeTotalCount?: boolean;
        ColumnSelection?: ColumnSelection;
        IncludeColumns?: string[];
        ExcludeColumns?: string[];
        ExportColumns?: string[];
        DistinctFields?: string[];
    }
        
    export interface ListResponse<TEntity> extends ServiceResponse {
        Entities?: TEntity[];
        Values?: any[];
        TotalCount?: number;
        Skip?: number;
        Take?: number;
    }
        
    export interface RetrieveRequest extends ServiceRequest {
        EntityId?: any;
        ColumnSelection?: RetrieveColumnSelection;
        IncludeColumns?: string[];
        ExcludeColumns?: string[];
    }
        
    export interface RetrieveResponse<TEntity> extends ServiceResponse {
        Entity?: TEntity;
    }
        
    export interface RetrieveLocalizationRequest extends RetrieveRequest {
    }
        
    export interface RetrieveLocalizationResponse<TEntity> extends ServiceResponse {
        Entities?: { [key: string]: TEntity };
    }
        
    export interface ServiceOptions<TResponse extends ServiceResponse> extends JQueryAjaxSettings {
        request?: any;
        service?: string;
        blockUI?: boolean;
        onError?(response: TResponse): void;
        onSuccess?(response: TResponse): void;
        onCleanup?(): void;
    }
        
    export const enum SummaryType {
        Disabled = -1,
        None = 0,
        Sum = 1,
        Avg = 2,
        Min = 3,
        Max = 4
    }
}

declare namespace Slick {

    export type CancellableViewCallback<TEntity> = (view: RemoteView<TEntity>) => boolean | void;
    export type RemoteViewAjaxCallback<TEntity> = (view: RemoteView<TEntity>, options: JQueryAjaxSettings) => boolean | void;
    export type RemoteViewFilter<TEntity> = (item: TEntity, view: RemoteView<TEntity>) => boolean;
    export type RemoteViewProcessCallback<TEntity> = (data: Serenity.ListResponse<TEntity>, view: RemoteView<TEntity>) => Serenity.ListResponse<TEntity>;
        
    interface SummaryOptions {
        aggregators: any[];
    }
        
    interface PagingOptions {
        rowsPerPage?: number;
        page?: number;
    }
        
    interface PagingInfo {
        rowsPerPage: number;
        page: number,
        totalCount: number;
        loading: boolean,
        error: string;
        dataView: RemoteView<any>
    }

    export interface RemoteViewOptions {
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
        groupItemMetadataProvider?: Data.GroupItemMetadataProvider;
        onAjaxCall?: RemoteViewAjaxCallback<any>;
        getItemMetadata?: (p1?: any, p2?: number) => any;
        errorMsg?: string;
    }
        
    export interface RemoteView<TEntity> {
        onSubmit: CancellableViewCallback<TEntity>;
        onDataChanged: Slick.Event;
        onDataLoading: Slick.Event;
        onDataLoaded: Slick.Event;
        onPagingInfoChanged: Slick.Event;
        onRowCountChanged: Slick.Event;
        onRowsChanged: Slick.Event;
        onRowsOrCountChanged: Slick.Event;
        getPagingInfo(): PagingInfo;
        onGroupExpanded: Event,
        onGroupCollapsed: Event,
        onAjaxCall: RemoteViewAjaxCallback<TEntity>;
        onProcessData: RemoteViewProcessCallback<TEntity>;
        addData(data: Serenity.ListResponse<TEntity>): void;
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
        setGrouping(groupInfo: Slick.GroupInfo<TEntity>[]): void;
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
}
