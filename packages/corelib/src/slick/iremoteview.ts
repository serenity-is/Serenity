import { EventEmitter, Grid, Group, GroupItemMetadataProvider, GroupTotals, IDataView, IEventData } from "@serenity-is/sleekgrid";
import { ListResponse, ServiceOptions } from "../base";
import { GroupInfo, PagingOptions, SummaryOptions } from "./slicktypes";

/**
 * Interface for an extension of IDataView that support remote data loading
 */
export interface IRemoteView<TItem = any> extends IDataView<TItem> {
    /**
     * Adds data received from the server to the view.
     * @param data The response data from the server
     */
    addData?(data: any): boolean;
    /**
     * Adds an item to the end of the items array.
     * @param item The item to add
     */
    addItem?(item: any): void;
    /**
     * Begins a batch update operation. Multiple changes can be made without triggering refreshes.
     * Call endUpdate() to complete the batch and refresh the view.
     */
    beginUpdate(): void;
    /**
     * Collapses all groups at the specified level, or all levels if not specified.
     * @param level Optional level to collapse. If not specified, applies to all levels.
     */
    collapseAllGroups?(level?: number): void;
    /**
     * Collapses a specific group.
     * @param varArgs Either a Slick.Group's "groupingKey" property, or a
     * variable argument list of grouping values denoting a unique path to the row.
     * For example, calling collapseGroup('high', '10%') will collapse the '10%' subgroup of the 'high' group.
     */
    collapseGroup?(varArgs: any[]): void;
    /**
     * Deletes an item by its ID.
     * @param id The ID of the item to delete
     */
    deleteItem?(id: any): void;
    /**
     * Ends a batch update operation. If this is the outermost endUpdate call,
     * refreshes the view to reflect all changes made during the batch.
     */
    endUpdate(): void;
    /**
     * Expands all groups at the specified level, or all levels if not specified.
     * @param level Optional level to expand. If not specified, applies to all levels.
     */
    expandAllGroups?(level?: number): void;
    /**
     * Expands a specific group.
     * @param varArgs Either a Slick.Group's "groupingKey" property, or a
     * variable argument list of grouping values denoting a unique path to the row.
     * For example, calling expandGroup('high', '10%') will expand the '10%' subgroup of the 'high' group.
     */
    expandGroup?(varArgs: any[]): void;
    /**
     * Gets the current filter function.
     * @returns The current filter function
     */
    getFilter?(): RemoteViewFilter<TItem>;
    /**
     * Gets the current grouping configuration.
     * @returns Array of grouping information
     */
    getGrouping?(): GroupInfo<TItem>[];
    /**
     * Gets the group item metadata provider.
     * @returns The metadata provider
     */
    getGroupItemMetadataProvider?(): GroupItemMetadataProvider;
    /**
     * Gets the current groups.
     * @returns Array of groups
     */
    getGroups?(): Group<TItem>[];
    /**
     * Gets the name of the property used as the unique identifier for items.
     * @returns The ID property name
     */
    getIdPropertyName(): string;
    /**
     * Gets the index of an item by its ID.
     * @param id The ID of the item
     * @returns The index of the item, or undefined if not found
     */
    getIdxById(id: any): number;
    /**
     * Gets an item by its ID.
     * @param id The ID of the item
     * @returns The item with the specified ID
     */
    getItemById(id: any): TItem;
    /**
     * Gets an item by its index in the items array.
     * @param i The index of the item
     * @returns The item at the specified index
     */
    getItemByIdx(i: number): any;
    /**
     * Gets all items in the view.
     * @returns Array of all items
     */
    getItems(): TItem[];
    /**
     * Gets whether local sorting is enabled.
     * @returns true if local sorting is enabled
     */
    getLocalSort?(): boolean;
    /**
     * Gets the current paging information.
     * @returns Object containing paging state information
     */
    getPagingInfo(): PagingInfo;
    /**
     * Gets the row index for an item by its ID.
     * @param id The ID of the item
     * @returns The row index of the item
     */
    getRowById?(id: any): number;
    /**
     * Gets the row index for an item.
     * @param item The item to find
     * @returns The row index of the item
     */
    getRowByItem?(item: any): number;
    /**
     * Gets all rows in the view (including group rows and totals rows).
     * @returns Array of all rows
     */
    getRows(): (TItem | Group<any> | GroupTotals<any>)[];
    /**
     * Inserts an item at the specified position.
     * @param insertBefore The index to insert before
     * @param item The item to insert
     */
    insertItem?(insertBefore: number, item: any): void;
    /** Callback invoked before making AJAX calls */
    onAjaxCall: RemoteViewAjaxCallback<TItem>;
    /** Event fired when data loading completes */
    readonly onDataLoaded: EventEmitter<any, IEventData>;
    /** Event fired when data loading begins */
    readonly onDataLoading: EventEmitter<any, IEventData>;
    /** Event fired when a group is collapsed */
    readonly onGroupCollapsed?: EventEmitter<any, IEventData>;
    /** Event fired when a group is expanded */
    readonly onGroupExpanded?: EventEmitter<any, IEventData>;
    /** Event fired when paging information changes */
    readonly onPagingInfoChanged: EventEmitter<any, IEventData>;
    /** Callback invoked to process data received from the server */
    onProcessData: RemoteViewProcessCallback<TItem>;
    /** Event fired when rows or count change */
    readonly onRowsOrCountChanged?: EventEmitter<any, IEventData>;
    /** Callback invoked before submitting a request, can cancel the operation */
    onSubmit: CancellableViewCallback<TItem>;
    /** Additional parameters to send with service requests */
    params: Record<string, any>;
    /**
     * Loads data from the server using the configured URL and parameters.
     * @returns false if the operation was cancelled or no URL is configured
     */
    populate(): boolean;
    /**
     * Locks population to prevent automatic data loading.
     * Use this when you want to make multiple changes without triggering loads.
     */
    populateLock(): void;
    /**
     * Unlocks population. If there were pending populate calls while locked, executes them.
     */
    populateUnlock(): void;
    /**
     * Refresh the view by recalculating the rows and notifying changes.
     * Note that this does not re-fetch the data from the server, use populate
     * method for that purpose.
     */
    refresh(): void;
    /**
     * Re-sorts the items using the current sort settings.
     */
    reSort(): void;
    /** The page number to seek to when loading data */
    seekToPage: number;
    /**
     * Sets the filter function to apply to items.
     * @param filterFn The filter function to apply
     */
    setFilter(filterFn: RemoteViewFilter<TItem>): void;
    /**
     * Sets the grouping configuration for the view.
     * @param groupingInfo Grouping information or array of grouping information
     */
    setGrouping?(groupingInfo: GroupInfo<TItem> | GroupInfo<TItem>[]): void;
    /**
     * Sets the group item metadata provider.
     * @param value The metadata provider to set
     */
    setGroupItemMetadataProvider?(value: GroupItemMetadataProvider): void;
    /**
     * Sets the items in the view and optionally changes the ID property.
     * @param data Array of items to set
     * @param newIdProperty Optional new ID property name, or boolean to reset
     */
    setItems(data: any[], newIdProperty?: string | boolean): void;
    /**
     * Sets whether to use local sorting. When enabled, sorting is done client-side.
     * @param value Whether to enable local sorting
     */
    setLocalSort?(value: boolean): void;
    /**
     * Sets paging options and triggers a data reload if options changed.
     * @param args The paging options to set
     */
    setPagingOptions(args: PagingOptions): void;
    /**
     * Sets summary/aggregation options for the view.
     * @param summary Object containing aggregators and other summary options
     */
    setSummaryOptions?(summary: SummaryOptions): void;
    /**
     * Sorts the items using the specified comparer function.
     * @param comparer Optional custom comparer function
     * @param ascending Whether to sort in ascending order (default true)
     */
    sort?(comparer?: (a: any, b: any) => number, ascending?: boolean): void;
    /**
     * Adds an item in sorted order.
     * @param item The item to add
     */
    sortedAddItem?(item: any): void;
    /**
     * Updates an item while maintaining sorted order.
     * @param id The ID of the item to update
     * @param item The new item data
     */
    sortedUpdateItem?(id: any, item: any): void;
    /** Sort expressions for the data */
    sortBy: string[];
    /**
     * Syncs cell CSS styles between the grid and the data view.
     */
    syncGridCellCssStyles?(grid: Grid, key: string): void;
    /***
     * Wires the grid and the DataView together to keep row selection tied to item ids.
     */
    syncGridSelection?(grid: Grid, preserveHidden: boolean, preserveHiddenOnSelectionChange: boolean): EventEmitter<any, IEventData>;
    /**
     * Updates an existing item in the view.
     * @param id The ID of the item to update
     * @param item The new item data
     */
    updateItem(id: any, item: any): void;
    /** The URL to fetch data from */
    url: string;
}

/**
 * Information about the current paging state of the view
 */
export interface PagingInfo {
    /** Reference to the RemoteView instance */
    dataView: IRemoteView<any>;
    /** Current error message, if any */
    error: string;
    /** Whether data is currently being loaded */
    loading: boolean;
    /** Current page number (1-based) */
    page: number;
    /** Number of rows displayed per page */
    rowsPerPage: number;
    /** Total number of items available */
    totalCount: number;
}

/**
 * Callback function that can cancel a view operation
 * @param view The RemoteView instance
 * @returns true to continue, false to cancel
 */
export type CancellableViewCallback<TItem> = (view: IRemoteView<TItem>) => boolean | void;

/**
 * Callback function for AJAX calls made by the view
 * @param view The RemoteView instance
 * @param options The service options for the AJAX call
 * @returns true to continue, false to cancel
 */
export type RemoteViewAjaxCallback<TItem> = (view: IRemoteView<TItem>, options: ServiceOptions<ListResponse<TItem>>) => boolean | void;

/**
 * Filter function for items in the view
 * @param item The item to test
 * @param view The RemoteView instance
 * @returns true if the item should be included
 */
export type RemoteViewFilter<TItem> = (item: TItem, view: IRemoteView<TItem>) => boolean;

/**
 * Callback function for processing data received from the server
 * @param data The raw data response
 * @param view The RemoteView instance
 * @returns The processed data
 */
export type RemoteViewProcessCallback<TItem> = (data: ListResponse<TItem>, view: IRemoteView<TItem>) => ListResponse<TItem>;