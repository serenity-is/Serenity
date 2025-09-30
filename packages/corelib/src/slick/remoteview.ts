import { EventData, EventEmitter, Grid, Group, GroupItemMetadataProvider, GroupTotals, IGroupTotals, ItemMetadata, gridDefaults } from "@serenity-is/sleekgrid";
import { ListRequest, ListResponse, PagerTexts, ServiceOptions, ServiceResponse, htmlEncode, serviceCall } from "../base";
import { AggregateFormatting, IAggregator } from "./aggregators";
import { CancellableViewCallback, IRemoteView, PagingInfo, RemoteViewAjaxCallback, RemoteViewFilter, RemoteViewProcessCallback } from "./iremoteview";
import { GroupInfo, PagingOptions, SummaryOptions } from "./slicktypes";

/**
 * A data view that supports remote data loading, sorting, filtering, grouping, and paging.
 * Extends the functionality of SlickGrid's DataView with server-side data operations.
 * 
 * @typeparam TItem The type of entities in the view
 */
export class RemoteView<TItem = any> implements IRemoteView<TItem> {
    private contentType: string;
    private dataType: string;
    private errormsg: string;
    private errorMessage: string = null;
    private filter: RemoteViewFilter<TItem>;
    private filterCache: any[] = [];
    private filteredItems: any = [];
    private grandAggregators: IAggregator[];
    private grandTotals: IGroupTotals;
    private groupingInfos: GroupInfo<TItem>[] = [];
    private groupItemMetadataProvider: GroupItemMetadataProvider;
    private groups: Group<TItem>[] = [];
    private idProperty: string;
    private idxById: Record<any, number> = {};
    private itemMetadataCallback?: (p1?: TItem, p2?: number) => any;
    private items: TItem[] = [];
    private loading: AbortController | boolean = false;
    private localSort: boolean;
    private method: string;
    private page = 1;
    private populateCalls = 0;
    private populateLocks = 0;
    private prevRefreshHints: any = {};
    private refreshHints: any = {};
    private rows: (TItem | Group | GroupTotals)[] = [];
    private rowsById: Record<any, number> = null;
    private rowsPerPage: number;
    private sortAsc = true;
    private sortComparer: any;
    private suspend = 0;
    private toggledGroupsByLevel: any[] = [];
    private totalCount: number = null;
    private totalRows = 0;
    private updated: Record<string, boolean> = null;

    /** Additional parameters to send with service requests */
    public params: Record<string, any>;

    /** The page number to seek to when loading data */
    public seekToPage: number;

    /** Sort expressions for the data */
    public sortBy: string[];

    /** The URL to fetch data from */
    public url: string;

    /** Callback invoked before making AJAX calls */
    public onAjaxCall: RemoteViewAjaxCallback<TItem>;

    /** Callback invoked to process data received from the server */
    public onProcessData: RemoteViewProcessCallback<TItem>;

    /** Callback invoked before submitting a request, can cancel the operation */
    public onSubmit: CancellableViewCallback<TItem>;

    /** Event fired when the underlying data changes */
    public readonly onDataChanged = new EventEmitter();

    /** Event fired when data loading completes */
    public readonly onDataLoaded = new EventEmitter();

    /** Event fired when data loading begins */
    public readonly onDataLoading = new EventEmitter();

    /** Event fired when a group is collapsed */
    public readonly onGroupCollapsed = new EventEmitter();

    /** Event fired when a group is expanded */
    public readonly onGroupExpanded = new EventEmitter();

    /** Event fired when paging information changes */
    public readonly onPagingInfoChanged = new EventEmitter();

    /** Event fired when the row count changes */
    public readonly onRowCountChanged = new EventEmitter();

    /** Event fired when specific rows change */
    public readonly onRowsChanged = new EventEmitter();

    /** Event fired when rows or count change */
    public readonly onRowsOrCountChanged = new EventEmitter();

    constructor(options: RemoteViewOptions<TItem>) {
        options ??= {}
        if (gridDefaults != null && gridDefaults.groupTotalsFormat === void 0)
            gridDefaults.groupTotalsFormat = AggregateFormatting.groupTotalsFormat;

        this.groupItemMetadataProvider = options.groupItemMetadataProvider;
        this.localSort = options.localSort ?? false;

        this.idProperty = options.idField ?? 'id';
        this.contentType = options.contentType ?? "application/json";
        this.dataType = options.dataType ?? 'json';
        this.filter = options.filter ?? null;
        this.itemMetadataCallback = options.getItemMetadata;
        this.params = options.params ?? {};
        this.onSubmit = options.onSubmit ?? null;
        this.url = options.url || null;
        this.rowsPerPage = options.rowsPerPage || 0;
        this.seekToPage = options.seekToPage || 1;
        this.onAjaxCall = options.onAjaxCall || null;
        this.onProcessData = options.onProcessData || null;
        this.method = options.method || "POST";
        this.errormsg = options.errormsg;
        this.sortBy = typeof options.sortBy == "string" && options.sortBy !== "" ? [options.sortBy] : (options.sortBy || []);

        options.url && options.autoLoad && this.populate();
    }

    /** Default configuration for grouping information */
    static readonly groupingInfoDefaults: GroupInfo<any> = {
        comparer: function (this: void, a: any, b: any) {
            return (a.value === b.value ? 0 :
                (a.value > b.value ? 1 : -1)
            );
        },
        aggregateEmpty: false,
        aggregateCollapsed: false,
        aggregateChildGroups: false,
        collapsed: false,
        displayTotalsRow: true,
        lazyTotalsCalculation: false,
        predefinedValues: []
    }

    /**
     * Begins a batch update operation. Multiple changes can be made without triggering refreshes.
     * Call endUpdate() to complete the batch and refresh the view.
     */
    public beginUpdate() {
        this.suspend++;
    }

    /**
     * Ends a batch update operation. If this is the outermost endUpdate call,
     * refreshes the view to reflect all changes made during the batch.
     */
    public endUpdate() {
        this.suspend--;
        if (this.suspend <= 0)
            this.refresh();
    }

    /**
     * Sets hints for the next refresh operation to optimize performance.
     * @param hints Object containing refresh hints like isFilterNarrowing, isFilterExpanding, etc.
     */
    public setRefreshHints(hints: any) {
        this.refreshHints = hints;
    }

    private updateIdxById(startingIndex?: number) {
        startingIndex = startingIndex || 0;
        let id: any;
        for (let i = startingIndex, l = this.items.length; i < l; i++) {
            id = (this.items[i] as any)[this.idProperty];
            if (id === undefined) {
                throw (`Each data element must implement a unique '${this.idProperty}' property. ` +
                    `Object at index '${i}' has no identity value: ${JSON.stringify(this.items[i])}`);
            }
            this.idxById[id] = i;
        }
    }

    private ensureIdUniqueness(): void {
        let id: any;
        for (let i = 0, l = this.items.length; i < l; i++) {
            id = (this.items[i] as any)[this.idProperty];
            if (id === undefined || this.idxById[id] !== i) {
                throw (`Each data element must implement a unique '${this.idProperty}' property. Object at index '${i}' ` +
                    (id == undefined ? "has no identity value: " : "has repeated identity value '" + id + "': ") +
                    JSON.stringify(this.items[i]));
            }
        }
    }

    /**
     * Gets all items in the view.
     * @returns Array of all items
     */
    public getItems(): TItem[] {
        return this.items;
    }

    /**
     * Gets the name of the property used as the unique identifier for items.
     * @returns The ID property name
     */
    public getIdPropertyName(): string {
        return this.idProperty;
    }

    /**
     * Sets the items in the view and optionally changes the ID property.
     * @param data Array of items to set
     * @param newIdProperty Optional new ID property name, or boolean to reset
     */
    public setItems(data: any[], newIdProperty?: string | boolean): void {
        if (newIdProperty != null && typeof newIdProperty == "string")
            this.idProperty = newIdProperty;

        this.items = this.filteredItems = data;
        if (this.localSort) {
            this.items.sort(this.getSortComparer());
        }

        this.idxById = {};
        this.rowsById = null;
        this.grandTotals = {};
        this.updateIdxById();
        this.ensureIdUniqueness();

        if (this.suspend) {
            this.recalc(this.items);
        }
        else {
            this.refresh();
        }

        this.onDataChanged.notify({ dataView: self }, null, self);
    }

    /**
     * Sets paging options and triggers a data reload if options changed.
     * @param args The paging options to set
     */
    public setPagingOptions(args: PagingOptions): void {
        let anyChange = false;

        if (args.rowsPerPage != undefined &&
            this.rowsPerPage != args.rowsPerPage) {
            this.rowsPerPage = args.rowsPerPage;
            anyChange = true;
        }

        if (args.page != undefined) {
            let newPage: number;
            if (!this.rowsPerPage)
                newPage = 1;
            else if (this.totalCount == null)
                newPage = args.page;
            else
                newPage = Math.min(args.page, Math.ceil(this.totalCount / this.rowsPerPage) + 1);

            if (newPage < 1)
                newPage = 1;

            if (newPage != this.page) {
                this.seekToPage = newPage;
                anyChange = true;
            }
        }

        if (anyChange)
            this.populate();
    }

    /**
     * Gets the current paging information.
     * @returns Object containing paging state information
     */
    public getPagingInfo(): PagingInfo {
        return {
            rowsPerPage: this.rowsPerPage,
            page: this.page,
            totalCount: this.totalCount,
            loading: this.loading != null && this.loading != false,
            error: this.errorMessage,
            dataView: this
        };
    }

    private getSortComparer(): (a: any, b: any) => number {
        if (this.sortComparer != null)
            return this.sortComparer;

        const cols: string[] = [];
        const asc: boolean[] = [];
        const sorts = this.sortBy || [];
        for (const s of sorts) {
            if (s == null)
                continue;
            if (s.length > 5 && s.toLowerCase().substring(s.length - 5).toLowerCase() == ' desc') {
                asc.push(false);
                cols.push(s.substring(0, s.length - 5));
            }
            else {
                asc.push(true);
                cols.push(s);
            }
        }

        return function (this: void, a: any, b: any) {
            for (let i = 0, l = cols.length; i < l; i++) {
                const field = cols[i];
                const sign = asc[i] ? 1 : -1;
                const value1 = a[field], value2 = b[field];
                const result = (value1 == value2 ? 0 : (value1 > value2 ? 1 : -1)) * sign;
                if (result != 0) {
                    return result;
                }
            }
            return 0;
        }
    }

    /**
     * Sorts the items using the specified comparer function.
     * @param comparer Optional custom comparer function
     * @param ascending Whether to sort in ascending order (default true)
     */
    public sort(comparer?: (a: any, b: any) => number, ascending?: boolean): void {
        this.sortAsc = ascending;
        if (ascending === false) {
            this.items.reverse();
        }

        this.sortComparer = comparer;
        this.items.sort(this.getSortComparer());

        if (ascending === false) {
            this.items.reverse();
        }
        this.idxById = {};
        this.updateIdxById();
        this.refresh();
    }

    /**
     * Gets whether local sorting is enabled.
     * @returns true if local sorting is enabled
     */
    public getLocalSort(): boolean {
        return this.localSort;
    }

    /**
     * Sets whether to use local sorting. When enabled, sorting is done client-side.
     * @param value Whether to enable local sorting
     */
    public setLocalSort(value: boolean): void {
        if (this.localSort != value) {
            this.localSort = value;
            this.sort();
        }
    }

    /**
     * Re-sorts the items using the current sort settings.
     */
    public reSort() {
        this.sort(this.sortComparer, this.sortAsc);
    }

    /**
     * Gets the filtered items (after applying the current filter).
     * @returns Array of filtered items
     */
    public getFilteredItems() {
        return this.filteredItems;
    }

    /**
     * Gets the current filter function.
     * @returns The current filter function
     */
    public getFilter() {
        return this.filter;
    }

    /**
     * Sets the filter function to apply to items.
     * @param filterFn The filter function to apply
     */
    public setFilter(filterFn: RemoteViewFilter<TItem>): void {
        this.filter = filterFn;
        this.refresh();
    }

    /**
     * Gets the current grouping configuration.
     * @returns Array of grouping information
     */
    public getGrouping() {
        return this.groupingInfos;
    }

    /**
     * Sets summary/aggregation options for the view.
     * @param summary Object containing aggregators and other summary options
     */
    public setSummaryOptions(summary: SummaryOptions): void {
        this.grandAggregators = summary?.aggregators || [];
        this.grandTotals = {};
        this.setGrouping(this.groupingInfos || []);
    }

    /**
     * Gets the grand totals for all aggregated data.
     * @returns Object containing grand totals
     */
    public getGrandTotals(): IGroupTotals {
        this.grandTotals ??= {};

        if (!this.grandTotals.initialized) {
            this.grandAggregators ??= [];
            let agg: IAggregator, idx = this.grandAggregators.length;

            while (idx--) {
                agg = this.grandAggregators[idx];
                agg.init();
                for (const item of this.items)
                    agg.accumulate(item);
                agg.storeResult(this.grandTotals);
            }
            this.grandTotals.initialized = true;
        }

        return this.grandTotals;
    }

    /**
     * Sets the grouping configuration for the view.
     * @param groupingInfo Grouping information or array of grouping information
     */
    public setGrouping(groupingInfo: GroupInfo<TItem> | GroupInfo<TItem>[]): void {
        if (!this.groupItemMetadataProvider) {
            this.groupItemMetadataProvider = new GroupItemMetadataProvider();
        }

        this.groups = [];
        this.toggledGroupsByLevel = [];
        groupingInfo = groupingInfo || [];
        this.groupingInfos = (groupingInfo instanceof Array) ? groupingInfo : [groupingInfo];

        for (let i = 0; i < this.groupingInfos.length; i++) {
            const gi = this.groupingInfos[i] = Object.assign({}, RemoteView.groupingInfoDefaults, this.groupingInfos[i]);
            gi.getterIsAFn ??= typeof gi.getter == "function";
            this.toggledGroupsByLevel[i] = {};
        }

        this.refresh();
    }

    /**
     * Gets an item by its index in the items array.
     * @param i The index of the item
     * @returns The item at the specified index
     */
    public getItemByIdx(i: number): any {
        return this.items[i];
    }

    /**
     * Gets the index of an item by its ID.
     * @param id The ID of the item
     * @returns The index of the item, or undefined if not found
     */
    public getIdxById(id: any): number {
        return this.idxById[id];
    }

    private ensureRowsByIdCache(): void {
        if (!this.rowsById) {
            this.rowsById = {};
            for (let i = 0, l = this.rows.length; i < l; i++) {
                this.rowsById[(this.rows[i] as any)[this.idProperty]] = i;
            }
        }
    }

    /**
     * Gets the row index for an item.
     * @param item The item to find
     * @returns The row index of the item
     */
    public getRowByItem(item: any): number {
        this.ensureRowsByIdCache();
        return this.rowsById[item[this.idProperty]];
    }

    /**
     * Gets the row index for an item by its ID.
     * @param id The ID of the item
     * @returns The row index of the item
     */
    public getRowById(id: any): number {
        this.ensureRowsByIdCache();
        return this.rowsById[id];
    }

    /**
     * Gets an item by its ID.
     * @param id The ID of the item
     * @returns The item with the specified ID
     */
    public getItemById(id: any): TItem {
        return this.items[this.idxById[id]];
    }

    /**
     * Maps an array of items to their corresponding row indices.
     * @param itemArray Array of items to map
     * @returns Array of row indices
     */
    public mapItemsToRows(itemArray: any[]) {
        const rows = [];
        this.ensureRowsByIdCache();
        for (let i = 0, l = itemArray.length; i < l; i++) {
            const row = this.rowsById[itemArray[i][this.idProperty]];
            if (row != null) {
                rows[rows.length] = row;
            }
        }
        return rows;
    }

    /**
     * Maps an array of IDs to their corresponding row indices.
     * @param idArray Array of item IDs to map
     * @returns Array of row indices
     */
    public mapIdsToRows(idArray: any[]) {
        const rows: any[] = [];
        this.ensureRowsByIdCache();
        for (let i = 0, l = idArray.length; i < l; i++) {
            const row = this.rowsById[idArray[i]];
            if (row != null) {
                rows[rows.length] = row;
            }
        }
        return rows;
    }

    /**
     * Maps an array of row indices to their corresponding item IDs.
     * @param rowArray Array of row indices to map
     * @returns Array of item IDs
     */
    public mapRowsToIds(rowArray: any[]) {
        const ids: any[] = [];
        for (let i = 0, l = rowArray.length; i < l; i++) {
            if (rowArray[i] < this.rows.length) {
                ids[ids.length] = (this.rows[rowArray[i]] as any)[this.idProperty];
            }
        }
        return ids;
    }

    /**
     * Updates an existing item in the view.
     * @param id The ID of the item to update
     * @param item The new item data
     */
    public updateItem(id: any, item: any) {
        if (this.idxById[id] === undefined) {
            throw new Error("Invalid id");
        }

        if (id !== item[this.idProperty]) {
            // make sure the new id is unique:
            const newId = item[this.idProperty];
            if (newId == null) {
                throw new Error("Cannot update item to associate with a null id");
            }
            if (this.idxById[newId] !== undefined) {
                throw new Error("Cannot update item to associate with a non-unique id");
            }
            this.idxById[newId] = this.idxById[id];
            delete this.idxById[id];

            if (this.updated && this.updated[id]) {
                delete this.updated[id];
            }

            id = newId;
        }
        this.items[this.idxById[id]] = item;

        if (!this.updated) {
            this.updated = {};
        }
        this.updated[id] = true;
        this.refresh();
    }

    /**
     * Inserts an item at the specified position.
     * @param insertBefore The index to insert before
     * @param item The item to insert
     */
    public insertItem(insertBefore: number, item: any) {
        this.items.splice(insertBefore, 0, item);
        this.updateIdxById(insertBefore);
        this.refresh();
    }

    /**
     * Adds an item to the end of the items array.
     * @param item The item to add
     */
    public addItem(item: any) {
        this.items.push(item);
        this.updateIdxById(this.items.length - 1);
        this.refresh();
    }

    /**
     * Deletes an item by its ID.
     * @param id The ID of the item to delete
     */
    public deleteItem(id: any) {
        const idx = this.idxById[id];
        if (idx === undefined) {
            throw "Invalid id";
        }
        delete this.idxById[id];
        this.items.splice(idx, 1);
        this.updateIdxById(idx);
        this.refresh();
    }

    /**
     * Adds an item in sorted order.
     * @param item The item to add
     */
    public sortedAddItem(item: any) {
        this.insertItem(this.sortedIndex(item), item);
    }

    /**
     * Updates an item while maintaining sorted order.
     * @param id The ID of the item to update
     * @param item The new item data
     */
    public sortedUpdateItem(id: any, item: any) {
        if (this.idxById[id] === undefined || id !== item[this.idProperty]) {
            throw new Error("Invalid or non-matching id " + this.idxById[id]);
        }
        const comparer = this.getSortComparer();
        const oldItem = this.getItemById(id);
        if (comparer(oldItem, item) !== 0) {
            // item affects sorting -> must use sorted add
            this.deleteItem(id);
            this.sortedAddItem(item);
        }
        else { // update does not affect sorting -> regular update works fine
            this.updateItem(id, item);
        }
    }

    private sortedIndex(searchItem: any) {
        let low = 0, high = this.items.length;
        const comparer = this.getSortComparer();
        while (low < high) {
            const mid = low + high >>> 1;
            if (comparer(this.items[mid], searchItem) === -1) {
                low = mid + 1;
            }
            else {
                high = mid;
            }
        }
        return low;
    }

    /**
     * Gets all rows in the view (including group rows and totals rows).
     * @returns Array of all rows
     */
    public getRows(): (TItem | Group<any> | GroupTotals<any>)[] {
        return this.rows;
    }

    /**
     * Gets the total number of rows in the view.
     * @returns The number of rows
     */
    public getLength() {
        return this.rows.length;
    }

    /**
     * Gets the item at the specified row index.
     * @param i The row index
     * @returns The item at the row index
     */
    public getItem(i: number): any {
        var item = this.rows[i];

        const group = item as Group;
        // if this is a group row, make sure the group is collapsed/expanded as needed
        // if this is a group row, make sure totals are calculated and update the title
        if (group && group.__group && group.totals && !group.totals.initialized) {
            var gi = this.groupingInfos[group.level];
            if (!gi.displayTotalsRow) {
                this.calculateTotals(group.totals);
                group.title = gi.formatter ? gi.formatter(group) : htmlEncode(group.value);
            }
        }
        // if this is a totals row, make sure it's calculated
        else if (item && (item as IGroupTotals).__groupTotals && !(item as IGroupTotals).initialized) {
            this.calculateTotals(item);
        }

        return item;
    }

    /**
     * Gets metadata for the item at the specified row index.
     * @param row The row index
     * @returns Metadata object or null
     */
    public getItemMetadata(row: number) {
        var item = this.rows[row];
        if (item === undefined) {
            return null;
        }

        // overrides for grouping rows
        if ((item as Group).__group) {
            return this.groupItemMetadataProvider.getGroupRowMetadata(item as Group);
        }

        // overrides for totals rows
        if ((item as IGroupTotals).__groupTotals) {
            return this.groupItemMetadataProvider.getTotalsRowMetadata(item);
        }

        return (this.itemMetadataCallback && this.itemMetadataCallback(item as TItem, row)) || null;
    }

    private expandCollapseAllGroups(level: number, collapse: boolean) {
        if (level == null) {
            for (var i = 0; i < this.groupingInfos.length; i++) {
                this.toggledGroupsByLevel[i] = {};
                this.groupingInfos[i].collapsed = collapse;

                if (collapse === true) {
                    this.onGroupCollapsed.notify({ level: i, groupingKey: null });
                } else {
                    this.onGroupExpanded.notify({ level: i, groupingKey: null });
                }
            }
        } else {
            this.toggledGroupsByLevel[level] = {};
            this.groupingInfos[level].collapsed = collapse;

            if (collapse === true) {
                this.onGroupCollapsed.notify({ level: level, groupingKey: null });
            } else {
                this.onGroupExpanded.notify({ level: level, groupingKey: null });
            }
        }
        this.refresh();
    }

    /**
     * Collapses all groups at the specified level, or all levels if not specified.
     * @param level Optional level to collapse. If not specified, applies to all levels.
     */
    public collapseAllGroups(level?: number) {
        this.expandCollapseAllGroups(level, true);
    }

    /**
     * Expands all groups at the specified level, or all levels if not specified.
     * @param level Optional level to expand. If not specified, applies to all levels.
     */
    public expandAllGroups(level?: number) {
        this.expandCollapseAllGroups(level, false);
    }

    private resolveLevelAndGroupingKey(args: any) {
        var arg0 = args[0];
        if (args.length === 1 && arg0.indexOf(groupingDelimiter) !== -1) {
            return { level: arg0.split(groupingDelimiter).length - 1, groupingKey: arg0 };
        } else {
            return { level: args.length - 1, groupingKey: args.join(groupingDelimiter) };
        }
    }

    private expandCollapseGroup(args: any, collapse: any) {
        var opts = this.resolveLevelAndGroupingKey(args);
        this.toggledGroupsByLevel[opts.level][opts.groupingKey] = !this.groupingInfos[opts.level].collapsed !== !collapse;
        if (collapse)
            this.onGroupCollapsed.notify({ level: opts.level, groupingKey: opts.groupingKey });
        else
            this.onGroupExpanded.notify({ level: opts.level, groupingKey: opts.groupingKey });

        this.refresh();
    }

    /**
     * Collapses a specific group.
     * @param varArgs Either a Slick.Group's "groupingKey" property, or a
     * variable argument list of grouping values denoting a unique path to the row.
     * For example, calling collapseGroup('high', '10%') will collapse the '10%' subgroup of the 'high' group.
     */
    public collapseGroup(varArgs: any[]) {
        var args = Array.prototype.slice.call(arguments);
        this.expandCollapseGroup(args, true);
    }

    /**
     * Expands a specific group.
     * @param varArgs Either a Slick.Group's "groupingKey" property, or a
     * variable argument list of grouping values denoting a unique path to the row.
     * For example, calling expandGroup('high', '10%') will expand the '10%' subgroup of the 'high' group.
     */
    public expandGroup(varArgs: any[]) {
        var args = Array.prototype.slice.call(arguments);
        this.expandCollapseGroup(args, false);
    }

    /**
     * Gets the current groups.
     * @returns Array of groups
     */
    public getGroups() {
        return this.groups;
    }

    private getOrCreateGroup(groupsByVal: any, val: any, level: number, parentGroup: any, groups: any[]) {
        var group = groupsByVal[val];

        if (!group) {
            group = new Group<any>();
            group.value = val;
            group.level = level;
            group.groupingKey = (parentGroup ? parentGroup.groupingKey + groupingDelimiter : '') + val;
            groups[groups.length] = group;
            groupsByVal[val] = group;
        }

        return group;
    }

    private extractGroups(rows: any[], parentGroup?: any) {
        var group: any;
        var val: any;
        var groups: any[] = [];
        var groupsByVal = {};
        var r: any;
        var level = parentGroup ? parentGroup.level + 1 : 0;
        var gi = this.groupingInfos[level];

        for (var i = 0, l: number = gi.predefinedValues.length; i < l; i++) {
            val = gi.predefinedValues[i];
            group = this.getOrCreateGroup(groupsByVal, val, level, parentGroup, groups);
        }

        for (var i = 0, l = rows.length; i < l; i++) {
            r = rows[i];
            val = gi.getterIsAFn ? gi.getter(r) : r[gi.getter];
            group = this.getOrCreateGroup(groupsByVal, val, level, parentGroup, groups);

            group.rows[group.count++] = r;
        }

        if (level < this.groupingInfos.length - 1) {
            for (var i = 0; i < groups.length; i++) {
                group = groups[i];
                group.groups = this.extractGroups(group.rows, group);
            }
        }

        if (groups.length) {
            this.addTotals(groups, level);
        }

        groups.sort(this.groupingInfos[level].comparer);

        return groups;
    }

    private calculateTotals(totals: IGroupTotals<TItem>) {
        var group = totals.group;
        var gi = this.groupingInfos[group.level];
        var isLeafLevel = (group.level == this.groupingInfos.length);
        const aggregators = (gi.aggregators ?? this.grandAggregators ?? []);
        var agg: IAggregator, idx = aggregators.length;

        if (!isLeafLevel && gi.aggregateChildGroups) {
            // make sure all the subgroups are calculated
            var i = group.groups.length;
            while (i--) {
                if (!group.groups[i].totals.initialized) {
                    this.calculateTotals(group.groups[i].totals);
                }
            }
        }

        while (idx--) {
            agg = aggregators[idx];
            agg.init();
            const items = !isLeafLevel && gi.aggregateChildGroups ? group.groups : group.rows;
            for (const item of items) {
                agg.accumulate(item);
            }
            agg.storeResult(totals);
        }
        totals.initialized = true;
    }

    private addGroupTotals(group: Group<TItem>) {
        var gi = this.groupingInfos[group.level];
        var totals = new GroupTotals<TItem>();
        totals.group = group;
        group.totals = totals;
        if (!gi.lazyTotalsCalculation) {
            this.calculateTotals(totals);
        }
    }

    private addTotals(groups: Group<TItem>[], level?: number) {
        level = level || 0;
        const gi = this.groupingInfos[level];
        const aggregators = (gi?.aggregators ?? this.grandAggregators ?? []);
        var groupCollapsed = !!gi?.collapsed;
        var toggledGroups = this.toggledGroupsByLevel[level];
        var idx = groups.length, g: Group<TItem>;
        while (idx--) {
            g = groups[idx];

            if (g.collapsed && !gi?.aggregateCollapsed) {
                continue;
            }

            // Do a depth-first aggregation so that parent group aggregators can access subgroup totals.
            if (g.groups) {
                this.addTotals(g.groups, level + 1);
            }

            if (aggregators.length && (
                gi.aggregateEmpty || g.rows.length || (g.groups && g.groups.length))) {
                this.addGroupTotals(g);
            }

            g.collapsed = groupCollapsed !== !!toggledGroups[g.groupingKey];
            g.title = gi.formatter ? gi.formatter(g) : htmlEncode(g.value);
        }
    }

    private flattenGroupedRows(groups: Group<TItem>[], level?: number) {
        level = level || 0;
        var gi = this.groupingInfos[level];
        var groupedRows: any[] = [], rows: any[], gl = 0, g: Group<TItem>;
        for (var i = 0, l = groups.length; i < l; i++) {
            g = groups[i];
            groupedRows[gl++] = g;

            if (!g.collapsed) {
                rows = g.groups ? this.flattenGroupedRows(g.groups, level + 1) : g.rows;
                for (var j = 0, jj = rows.length; j < jj; j++) {
                    groupedRows[gl++] = rows[j];
                }
            }

            if (g.totals && gi.displayTotalsRow && (!g.collapsed || gi.aggregateCollapsed)) {
                groupedRows[gl++] = g.totals;
            }
        }
        return groupedRows;
    }

    private batchFilter(items: any[]) {
        var retval: any[] = [], idx = 0;

        for (var i = 0, ii = items.length; i < ii; i++) {
            if (this.filter(items[i], this)) {
                retval[idx++] = items[i];
            }
        }

        return retval;
    }

    private batchFilterWithCaching(items: any[], cache: any) {
        var retval: any[] = [], idx = 0, item: any;

        for (var i = 0, ii = items.length; i < ii; i++) {
            item = items[i];
            if (cache[i]) {
                retval[idx++] = item;
            } else if (this.filter(item, this)) {
                retval[idx++] = item;
                cache[i] = true;
            }
        }

        return retval;
    }

    private getFilteredAndPagedItems(items: any[]) {
        if (this.filter) {
            if (this.refreshHints?.isFilterNarrowing) {
                this.filteredItems = this.batchFilter(this.filteredItems);
            } else if (this.refreshHints?.isFilterExpanding) {
                this.filteredItems = this.batchFilterWithCaching(items, this.filterCache);
            } else if (!this.refreshHints?.isFilterUnchanged) {
                this.filteredItems = this.batchFilter(items);
            }
        } else {
            // special case:  if not filtering and not paging, the resulting
            // rows collection needs to be a copy so that changes due to sort
            // can be caught
            this.filteredItems = items.concat();
        }

        // get the current page
        return { totalRows: this.filteredItems.length, rows: this.filteredItems };
    }

    private getRowDiffs(rows: any[], newRows: any[]) {
        var item: any, r: any, eitherIsNonData: boolean, diff: any[] = [];
        var from = 0, to = newRows.length;

        if (this.refreshHints?.ignoreDiffsBefore) {
            from = Math.max(0,
                Math.min(newRows.length, this.refreshHints.ignoreDiffsBefore));
        }

        if (this.refreshHints?.ignoreDiffsAfter) {
            to = Math.min(newRows.length,
                Math.max(0, this.refreshHints.ignoreDiffsAfter));
        }

        for (var i = from, rl = rows.length; i < to; i++) {
            if (i >= rl) {
                diff[diff.length] = i;
            } else {
                item = newRows[i];
                r = rows[i];

                if ((this.groupingInfos.length && (eitherIsNonData = (item.__nonDataRow) || (r.__nonDataRow)) &&
                    item.__group !== r.__group ||
                    item.__group && !item.equals(r))
                    || (eitherIsNonData &&
                        // no good way to compare totals since they are arbitrary DTOs
                        // deep object comparison is pretty expensive
                        // always considering them 'dirty' seems easier for the time being
                        (item.__groupTotals || r.__groupTotals))
                    || item[this.idProperty] != r[this.idProperty]
                    || (this.updated && this.updated[item[this.idProperty]])
                ) {
                    diff[diff.length] = i;
                }
            }
        }
        return diff;
    }

    private recalc(_items: any[]) {
        this.rowsById = null;

        if (this.refreshHints?.isFilterNarrowing != this.prevRefreshHints?.isFilterNarrowing ||
            this.refreshHints?.isFilterExpanding != this.prevRefreshHints?.isFilterExpanding) {
            this.filterCache = [];
        }

        var filteredItems = this.getFilteredAndPagedItems(_items);
        this.totalRows = filteredItems.totalRows;
        var newRows = filteredItems.rows;

        this.grandTotals = {};

        this.groups = [];
        if (this.groupingInfos.length) {
            this.groups = this.extractGroups(newRows);
            if (this.groups.length) {
                newRows = this.flattenGroupedRows(this.groups);
            }
        }

        var diff = this.getRowDiffs(this.rows, newRows);

        this.rows = newRows;

        return diff;
    }

    /**
     * Refresh the view by recalculating the rows and notifying changes.
     * Note that this does not re-fetch the data from the server, use populate
     * method for that purpose.
     */
    public refresh() {
        if (this.suspend) {
            return;
        }

        var countBefore = this.rows.length;
        var totalRowsBefore = this.totalRows;

        var diff = this.recalc(this.items); // pass as direct refs to avoid closure perf hit

        this.updated = null;
        this.prevRefreshHints = this.refreshHints;
        this.refreshHints = {};

        if (totalRowsBefore !== this.totalRows) {
            this.onPagingInfoChanged.notify(this.getPagingInfo(), null, self);
        }
        if (countBefore !== this.rows.length) {
            this.onRowCountChanged.notify({ previous: countBefore, current: this.rows.length, dataView: self }, null, self);
        }
        if (diff.length > 0) {
            this.onRowsChanged.notify({ rows: diff, dataView: self }, null, self);
        }
        if (countBefore !== this.rows.length || diff.length > 0) {
            this.onRowsOrCountChanged.notify({
                rowsDiff: diff, previousRowCount: countBefore, currentRowCount: this.rows.length,
                rowCountChanged: countBefore !== this.rows.length, rowsChanged: diff.length > 0, dataView: self
            }, null, self);
        }
    }

    /***
     * Wires the grid and the DataView together to keep row selection tied to item ids.
     * This is useful since, without it, the grid only knows about rows, so if the items
     * move around, the same rows stay selected instead of the selection moving along
     * with the items.
     *
     * NOTE:  This doesn't work with cell selection model.
     *
     * @param grid The grid to sync selection with.
     * @param preserveHidden Whether to keep selected items that go out of the
     *     view due to them getting filtered out.
     * @param preserveHiddenOnSelectionChange Whether to keep selected items
     *     that are currently out of the view (see preserveHidden) as selected when selection
     *     changes.
     * @return An event that notifies when an internal list of selected row ids
     *     changes.  This is useful since, in combination with the above two options, it allows
     *     access to the full list selected row ids, and not just the ones visible to the grid.
     */
    public syncGridSelection(grid: Grid, preserveHidden?: boolean, preserveHiddenOnSelectionChange?: boolean) {
        var self = this;
        var inHandler: any;
        var selectedRowIds = self.mapRowsToIds(grid.getSelectedRows());
        var onSelectedRowIdsChanged = new EventEmitter();

        function setSelectedRowIds(this: void, rowIds: any[]) {
            if (selectedRowIds.join(",") == rowIds.join(",")) {
                return;
            }

            selectedRowIds = rowIds;

            onSelectedRowIdsChanged.notify({
                "grid": grid,
                "ids": selectedRowIds,
                "dataView": self
            }, new EventData(), self);
        }

        function update(this: void) {
            if (selectedRowIds.length > 0) {
                inHandler = true;
                var selectedRows = self.mapIdsToRows(selectedRowIds);
                if (!preserveHidden) {
                    setSelectedRowIds(self.mapRowsToIds(selectedRows));
                }
                grid.setSelectedRows(selectedRows);
                inHandler = false;
            }
        }

        grid.onSelectedRowsChanged.subscribe(function (this: void, e: any, args: any) {
            if (inHandler) { return; }
            var newSelectedRowIds = self.mapRowsToIds(grid.getSelectedRows());
            if (!preserveHiddenOnSelectionChange || !grid.getOptions().multiSelect) {
                setSelectedRowIds(newSelectedRowIds);
            } else {
                // keep the ones that are hidden
                var existing = selectedRowIds.filter((id: any) => self.getRowById(id) === undefined);
                // add the newly selected ones
                setSelectedRowIds(existing.concat(newSelectedRowIds));
            }
        });

        this.onRowsChanged.subscribe(update);
        this.onRowCountChanged.subscribe(update);

        return onSelectedRowIdsChanged;
    }

    /**
     * Syncs cell CSS styles between the grid and the data view.
     * @param grid The grid to sync styles with
     * @param key The style key to sync
     */
    public syncGridCellCssStyles(grid: Grid, key: string) {
        var hashById: any;
        var inHandler: any;

        // since this method can be called after the cell styles have been set,
        // get the existing ones right away
        storeCellCssStyles(grid.getCellCssStyles(key));

        const self = this;

        function storeCellCssStyles(this: void, hash: any) {
            hashById = {};
            for (var row in hash) {
                var id: any = (self.rows[row as any] as any)[self.idProperty];
                hashById[id] = hash[row];
            }
        }

        function update(this: void) {
            if (hashById) {
                inHandler = true;
                self.ensureRowsByIdCache();
                var newHash: Record<number, any> = {};
                for (var id in hashById) {
                    var row = self.rowsById[id];
                    if (row != undefined) {
                        newHash[row] = hashById[id];
                    }
                }
                grid.setCellCssStyles(key, newHash);
                inHandler = false;
            }
        }

        var subFunc = function (this: void, e: any, args: any) {
            if (inHandler) { return; }
            if (key != args.key) { return; }
            if (args.hash) {
                storeCellCssStyles(args.hash);
            }
            else {
                grid.onCellCssStylesChanged.unsubscribe(subFunc);
                self.onRowsOrCountChanged.unsubscribe(update);
            }
        };

        grid.onCellCssStylesChanged.subscribe(subFunc);
        this.onRowsOrCountChanged.subscribe(update);
    }

    /**
     * Adds data received from the server to the view.
     * @param data The response data from the server
     */
    public addData(data: any) {

        if (this.onProcessData && data)
            data = this.onProcessData(data, this) || data;

        this.errorMessage = null;
        this.loading && typeof this.loading !== "boolean" && this.loading.abort();
        this.loading = false;

        if (!data) {
            this.errorMessage = this.errormsg ?? PagerTexts.DefaultLoadError;
            this.onPagingInfoChanged.notify(this.getPagingInfo());
            return false;
        }

        data.TotalCount = data.TotalCount || 0;
        data.Entities = data.Entities || [];

        if (!data.Skip || (!this.rowsPerPage && !data.Take))
            data.Page = 1;
        else
            data.Page = Math.ceil(data.Skip / (data.Take || this.rowsPerPage)) + 1;

        this.page = data.Page;
        this.totalCount = data.TotalCount;

        this.setItems(data.Entities);

        this.onPagingInfoChanged.notify(this.getPagingInfo());
    }

    /**
     * Loads data from the server using the configured URL and parameters.
     * @returns false if the operation was cancelled or no URL is configured
     */
    public populate() {
        if (this.populateLocks > 0) {
            this.populateCalls++;
            return;
        }

        this.populateCalls = 0;

        this.loading && typeof this.loading !== "boolean" && this.loading.abort();

        if (this.onSubmit) {
            var gh = this.onSubmit(this);
            if (gh === false)
                return false;
        }

        this.onDataLoading.notify(this);

        if (!this.url)
            return false;

        // set loading event

        if (!this.seekToPage)
            this.seekToPage = 1;

        var request: ListRequest = {};

        var skip = (this.seekToPage - 1) * this.rowsPerPage;
        if (skip)
            request.Skip = skip;
        if (this.rowsPerPage)
            request.Take = this.rowsPerPage;

        if (this.sortBy && this.sortBy.length) {
            if (typeof this.sortBy !== "string")
                request.Sort = this.sortBy;
            else {
                request.Sort = [this.sortBy];
            }
        }

        if (this.params) {
            request = Object.assign(request, this.params);
        }

        const controller = new AbortController();

        const self = this;
        const serviceOptions: ServiceOptions<ListResponse<TItem>> = {
            allowRedirect: false,
            cache: "no-store",
            method: this.method,
            headers: {
                "Content-Type": this.contentType,
                "Accept": (this.dataType == "json" || this.dataType == "application/json") ? "application/json" : this.dataType
            },
            request,
            url: this.url,
            signal: controller.signal,
            errorMode: 'notification',
            onSuccess: function (this: void, response: ServiceResponse) {
                self.addData(response);
            },
            onCleanup: function (this: void) {
                self.loading = false;
                self.onPagingInfoChanged.notify(self.getPagingInfo());
                self.onDataLoaded.notify(self);
            }
        }

        if (this.onAjaxCall) {
            var ah = this.onAjaxCall(this, serviceOptions);
            if (ah === false) {
                this.loading = false;
                this.onPagingInfoChanged.notify(this.getPagingInfo());
                return false;
            }
        }

        serviceCall(serviceOptions);
        this.onPagingInfoChanged.notify(this.getPagingInfo());
        this.loading = controller;
    }

    /**
     * Locks population to prevent automatic data loading.
     * Use this when you want to make multiple changes without triggering loads.
     */
    public populateLock() {
        if (this.populateLocks == 0)
            this.populateCalls = 0;
        this.populateLocks++;
    }

    /**
     * Unlocks population. If there were pending populate calls while locked, executes them.
     */
    public populateUnlock() {
        if (this.populateLocks > 0) {
            this.populateLocks--;
            if (this.populateLocks == 0 && this.populateCalls > 0)
                this.populate();
        }
    }

    /**
     * Gets the group item metadata provider.
     * @returns The metadata provider
     */
    public getGroupItemMetadataProvider() {
        return this.groupItemMetadataProvider;
    }

    /**
     * Sets the group item metadata provider.
     * @param value The metadata provider to set
     */
    public setGroupItemMetadataProvider(value: GroupItemMetadataProvider) {
        this.groupItemMetadataProvider = value;
    }

    /** @deprecated Gets the ID property name, for compatibility */
    get idField(): string {
        return this.idProperty;
    }
}

/**
 * Options for configuring a RemoteView instance
 */
export interface RemoteViewOptions<TItem = any> {
    /** Automatically load data (call populate) on initialization */
    autoLoad?: boolean;
    /** HTTP content type for service requests */
    contentType?: string;
    /** Expected data type of the service response */
    dataType?: string;
    /** Error message to display when requests fail */
    errormsg?: string;
    /** Filter criteria or function to apply to the data */
    filter?: RemoteViewFilter<TItem>;
    /** Callback function to get metadata for individual items */
    getItemMetadata?: (item: TItem, row: number) => ItemMetadata<TItem>;
    /** Provider for group item metadata in grouped views */
    groupItemMetadataProvider?: GroupItemMetadataProvider;
    /** Name of the field containing unique item identifiers */
    idField?: string;
    /** Whether to perform sorting locally instead of server-side */
    localSort?: boolean;
    /** HTTP method to use for service requests */
    method?: string;
    /** Callback function invoked before AJAX calls are made */
    onAjaxCall?: RemoteViewAjaxCallback<TItem>;
    /** Callback function to process data received from the server */
    onProcessData?: RemoteViewProcessCallback<TItem>;
    /** Callback function invoked before submitting service requests */
    onSubmit?: CancellableViewCallback<TItem>;
    /** Additional parameters to include in service requests */
    params?: Record<string, object>;
    /** Number of rows to display per page (0 for no paging) */
    rowsPerPage?: number;
    /** Initial page number to seek to on first load */
    seekToPage?: number;
    /** Initial sort criteria for the data */
    sortBy?: string | string[];
    /** URL of the service endpoint for data requests */
    url?: string;
}

const groupingDelimiter = ':|:';