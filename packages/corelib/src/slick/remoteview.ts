import { deepClone, extend, htmlEncode, ListRequest, ListResponse, notifyError, ServiceResponse, localText } from "../q";
import { EventEmitter, EventData, Grid, gridDefaults, Group, GroupItemMetadataProvider, GroupTotals } from "@serenity-is/sleekgrid";
import { AggregateFormatting } from "./aggregators";
import { GroupInfo, PagingOptions, SummaryOptions } from "./slicktypes";

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
    groupItemMetadataProvider?: GroupItemMetadataProvider;
    onAjaxCall?: RemoteViewAjaxCallback<any>;
    getItemMetadata?: (p1?: any, p2?: number) => any;
    errorMsg?: string;
}
    
export interface PagingInfo {
    rowsPerPage: number;
    page: number,
    totalCount: number;
    loading: boolean,
    error: string;
    dataView: RemoteView<any>
}

export type CancellableViewCallback<TEntity> = (view: RemoteView<TEntity>) => boolean | void;
export type RemoteViewAjaxCallback<TEntity> = (view: RemoteView<TEntity>, options: JQueryAjaxSettings) => boolean | void;
export type RemoteViewFilter<TEntity> = (item: TEntity, view: RemoteView<TEntity>) => boolean;
export type RemoteViewProcessCallback<TEntity> = (data: ListResponse<TEntity>, view: RemoteView<TEntity>) => ListResponse<TEntity>;

export interface RemoteView<TEntity> {
    onSubmit: CancellableViewCallback<TEntity>;
    onDataChanged: EventEmitter;
    onDataLoading: EventEmitter;
    onDataLoaded: EventEmitter;
    onPagingInfoChanged: EventEmitter;
    onRowCountChanged: EventEmitter;
    onRowsChanged: EventEmitter;
    onRowsOrCountChanged: EventEmitter;
    getPagingInfo(): PagingInfo;
    onGroupExpanded: EventEmitter,
    onGroupCollapsed: EventEmitter,
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

export class RemoteView<TEntity> {
    constructor(options: RemoteViewOptions) {
        var self = this;

        if (gridDefaults != null && gridDefaults.groupTotalsFormatter === void 0)
            gridDefaults.groupTotalsFormatter = AggregateFormatting.groupTotalsFormatter;
        
        var idProperty: string;
        var items: any[] = [];
        var rows: any[] = [];
        var idxById: Record<any, number> = {};
        var rowsById: any = null;
        var filter: any = null;
        var updated: any = null;
        var suspend = 0;

        var sortAsc = true;
        var fastSortField: string;
        var sortComparer: any;
        var refreshHints: any = {};
        var prevRefreshHints: any = {};
        var filterArgs: any;
        var filteredItems: any = [];
        var compiledFilter: any;
        var compiledFilterWithCaching: any;
        var filterCache: any[] = [];

        var groupingInfoDefaults = {
            getter: <any>null,
            formatter: <any>null,
            comparer: function (a: any, b: any) {
                return (a.value === b.value ? 0 :
                    (a.value > b.value ? 1 : -1)
                );
            },
            predefinedValues: <any[]>[],
            aggregateEmpty: false,
            aggregateCollapsed: false,
            aggregateChildGroups: false,
            collapsed: false,
            displayTotalsRow: true,
            lazyTotalsCalculation: false
        };
        var summaryOptions: any = {};
        var groupingInfos: any[] = [];
        var groups: any[] = [];
        var toggledGroupsByLevel: any[] = [];
        var groupingDelimiter = ':|:';

        var page = 1;
        var totalRows = 0;

        var onDataChanged: EventEmitter = new EventEmitter();
        var onDataLoading: EventEmitter = new EventEmitter();
        var onDataLoaded: EventEmitter = new EventEmitter();
        var onGroupExpanded: EventEmitter = new EventEmitter();
        var onGroupCollapsed: EventEmitter = new EventEmitter();
        var onPagingInfoChanged: EventEmitter = new EventEmitter();
        var onRowCountChanged: EventEmitter = new EventEmitter();
        var onRowsChanged: EventEmitter = new EventEmitter();
        var onRowsOrCountChanged: EventEmitter = new EventEmitter();

        var loading: any = false;
        var errorMessage: string = null;
        var populateLocks = 0;
        var populateCalls = 0;
        var contentType: string;
        var dataType: string;
        var totalCount: number = null;
        var groupItemMetadataProvider = options.groupItemMetadataProvider;
        var localSort: boolean = options.localSort ?? false;

        var intf: RemoteView<TEntity>;

        function beginUpdate() {
            suspend++;
        }

        function endUpdate() {
            suspend--;
            if (suspend <= 0)
                refresh();
        }

        function setRefreshHints(hints: any) {
            refreshHints = hints;
        }

        function setFilterArgs(args: any) {
            filterArgs = args;
        }

        function updateIdxById(startingIndex?: number) {
            startingIndex = startingIndex || 0;
            var id: any;
            for (var i = startingIndex, l = items.length; i < l; i++) {
                id = items[i][idProperty];
                if (id === undefined) {
                    var msg = "Each data element must implement a unique '" +
                        idProperty + "' property. Object at index '" + i + "' " +
                        "has no identity value: ";

                    msg += (<any>$).toJSON(items[i]);
                    throw msg;
                }
                idxById[id] = i;
            }
        }

        function ensureIdUniqueness() {
            var id: any;
            for (var i = 0, l = items.length; i < l; i++) {
                id = items[i][idProperty];
                if (id === undefined || idxById[id] !== i) {
                    var msg = "Each data element must implement a unique '" +
                        idProperty + "' property. Object at index '" + i + "' ";

                    if (id == undefined)
                        msg += "has no identity value: ";
                    else
                        msg += "has repeated identity value '" + id + "': ";

                    msg += (<any>$).toJSON(items[i]);
                    throw msg;
                }
            }
        }

        function getItems() {
            return items;
        }

        function getIdPropertyName() {
            return idProperty;
        }

        function setItems(data: any[], newIdProperty?: string | boolean) {
            if (newIdProperty != null && typeof newIdProperty == "string")
                idProperty = newIdProperty;

            items = filteredItems = data;
            if (localSort) {
                items.sort(getSortComparer());
            }

            idxById = {};
            rowsById = null;
            summaryOptions.totals = {};
            updateIdxById();
            ensureIdUniqueness();

            if (suspend) {
                recalc(items);
            }
            else {
                refresh();
            }

            onDataChanged.notify({ dataView: self }, null, self);
        }

        function setPagingOptions(args: any) {
            var anyChange = false;

            if (args.rowsPerPage != undefined &&
                intf.rowsPerPage != args.rowsPerPage) {
                intf.rowsPerPage = args.rowsPerPage;
                anyChange = true;
            }

            if (args.page != undefined) {
                var newPage: number;
                if (!intf.rowsPerPage)
                    newPage = 1;
                else if (totalCount == null)
                    newPage = args.page;
                else
                    newPage = Math.min(args.page, Math.ceil(totalCount / intf.rowsPerPage) + 1);

                if (newPage < 1)
                    newPage = 1;

                if (newPage != page) {
                    intf.seekToPage = newPage;
                    anyChange = true;
                }
            }

            if (anyChange)
                populate();
        }

        function getPagingInfo(): PagingInfo {
            return {
                rowsPerPage: intf.rowsPerPage,
                page: page,
                totalCount: totalCount,
                loading: loading,
                error: errorMessage,
                dataView: intf
            };
        }

        function getSortComparer() {
            if (sortComparer != null)
                return sortComparer;

            var cols: string[] = [];
            var asc: boolean[] = [];
            var sorts = intf.sortBy || [];
            for (var s of sorts) {
                if (s == null)
                    continue;
                if (s.length > 5 && s.toLowerCase().substr(s.length - 5).toLowerCase() == ' desc') {
                    asc.push(false);
                    cols.push(s.substr(0, s.length - 5));
                }
                else {
                    asc.push(true);
                    cols.push(s);
                }
            }
            return function (a: any, b: any) {
                for (var i = 0, l = cols.length; i < l; i++) {
                    var field = cols[i];
                    var sign = asc[i] ? 1 : -1;
                    var value1 = a[field], value2 = b[field];
                    var result = (value1 == value2 ? 0 : (value1 > value2 ? 1 : -1)) * sign;
                    if (result != 0) {
                        return result;
                    }
                }
                return 0;
            }
        }

        function sort(comparer?: (a: any, b: any) => number, ascending?: boolean) {
            sortAsc = ascending;
            fastSortField = null;
            if (ascending === false) {
                items.reverse();
            }

            sortComparer = comparer;
            items.sort(getSortComparer());

            if (ascending === false) {
                items.reverse();
            }
            idxById = {};
            updateIdxById();
            refresh();
        }

        function getLocalSort(): boolean {
            return localSort;
        }

        function setLocalSort(value: boolean) {
            if (localSort != value) {
                localSort = value;
                sort();
            }
        }

        /***
         * Provides a workaround for the extremely slow sorting in IE.
         * Does a [lexicographic] sort on a give column by temporarily overriding Object.prototype.toString
         * to return the value of that field and then doing a native Array.sort().
         */
        function fastSort(field: any, ascending: boolean) {
            sortAsc = ascending;
            fastSortField = field;
            sortComparer = null;
            var oldToString = Object.prototype.toString;
            Object.prototype.toString = (typeof field === "function") ? field : function () {
                return this[field]
            };
            // an extra reversal for descending sort keeps the sort stable
            // (assuming a stable native sort implementation, which isn't true in some cases)
            if (ascending === false) {
                items.reverse();
            }
            items.sort();
            Object.prototype.toString = oldToString;
            if (ascending === false) {
                items.reverse();
            }
            idxById = {};
            updateIdxById();
            refresh();
        }

        function reSort() {
            if (fastSortField)
                fastSort(fastSortField, sortAsc);
            else
                sort(sortComparer, sortAsc);
        }

        function getFilteredItems() {
            return filteredItems;
        }

        function getFilter() {
            return filter;
        }

        function setFilter(filterFn: any) {
            filter = filterFn;
            if (options.inlineFilters) {
                compiledFilter = compileFilter();
                compiledFilterWithCaching = compileFilterWithCaching();
            }
            refresh();
        }

        function getGrouping() {
            return groupingInfos;
        }

        function setSummaryOptions(summary: any) {
            summary = summary || {};
            summaryOptions.aggregators = summary.aggregators || [];
            summaryOptions.compiledAccumulators = [];
            summaryOptions.totals = {};
            var idx = summaryOptions.aggregators.length;
            while (idx--) {
                summaryOptions.compiledAccumulators[idx] = compileAccumulatorLoop(summaryOptions.aggregators[idx]);
            }
            setGrouping(groupingInfos || []);
        }

        function getGrandTotals() {
            summaryOptions.totals = summaryOptions.totals || {};

            if (!summaryOptions.totals.initialized) {
                summaryOptions.aggregators = summaryOptions.aggregators || [];
                summaryOptions.compiledAccumulators = summaryOptions.compiledAccumulators || [];
                var agg: any, idx = summaryOptions.aggregators.length;

                while (idx--) {
                    agg = summaryOptions.aggregators[idx];
                    agg.init();
                    summaryOptions.compiledAccumulators[idx].call(agg, items);
                    agg.storeResult(summaryOptions.totals);
                }
                summaryOptions.totals.initialized = true;
            }

            return summaryOptions.totals;
        }

        function setGrouping(groupingInfo: any) {
            if (!groupItemMetadataProvider) {
                groupItemMetadataProvider = new GroupItemMetadataProvider();
            }

            groups = [];
            toggledGroupsByLevel = [];
            groupingInfo = groupingInfo || [];
            groupingInfos = (groupingInfo instanceof Array) ? groupingInfo : [groupingInfo];

            for (var i = 0; i < groupingInfos.length; i++) {
                var gi = groupingInfos[i] = extend(extend<any>({}, groupingInfoDefaults), deepClone(groupingInfos[i]));
                gi.aggregators = gi.aggregators || summaryOptions.aggregators || [];
                gi.getterIsAFn = typeof gi.getter === "function";

                // pre-compile accumulator loops
                gi.compiledAccumulators = [];
                var idx = gi.aggregators.length;
                while (idx--) {
                    gi.compiledAccumulators[idx] = compileAccumulatorLoop(gi.aggregators[idx]);
                }

                toggledGroupsByLevel[i] = {};
            }

            refresh();
        }

        function getItemByIdx(i: number) {
            return items[i];
        }

        function getIdxById(id: any) {
            return idxById[id];
        }

        function ensureRowsByIdCache() {
            if (!rowsById) {
                rowsById = {};
                for (var i = 0, l = rows.length; i < l; i++) {
                    rowsById[rows[i][idProperty]] = i;
                }
            }
        }

        function getRowByItem(item: any) {
            ensureRowsByIdCache();
            return rowsById[item[idProperty]];
        }

        function getRowById(id: any) {
            ensureRowsByIdCache();
            return rowsById[id];
        }

        function getItemById(id: any) {
            return items[idxById[id]];
        }

        function mapItemsToRows(itemArray: any[]) {
            var rows = [];
            ensureRowsByIdCache();
            for (var i = 0, l = itemArray.length; i < l; i++) {
                var row = rowsById[itemArray[i][idProperty]];
                if (row != null) {
                    rows[rows.length] = row;
                }
            }
            return rows;
        }

        function mapIdsToRows(idArray: any[]) {
            var rows: any[] = [];
            ensureRowsByIdCache();
            for (var i = 0, l = idArray.length; i < l; i++) {
                var row = rowsById[idArray[i]];
                if (row != null) {
                    rows[rows.length] = row;
                }
            }
            return rows;
        }

        function mapRowsToIds(rowArray: any[]) {
            var ids: any[] = [];
            for (var i = 0, l = rowArray.length; i < l; i++) {
                if (rowArray[i] < rows.length) {
                    ids[ids.length] = rows[rowArray[i]][idProperty];
                }
            }
            return ids;
        }

        function updateItem(id: any, item: any) {
            if (idxById[id] === undefined) {
                throw new Error("Invalid id");
            }

            if (id !== item[idProperty]) {
                // make sure the new id is unique:
                var newId = item[idProperty];
                if (newId == null) {
                    throw new Error("Cannot update item to associate with a null id");
                }
                if (idxById[newId] !== undefined) {
                    throw new Error("Cannot update item to associate with a non-unique id");
                }
                idxById[newId] = idxById[id];
                delete idxById[id];

                if (updated && updated[id]) {
                    delete updated[id];
                }

                id = newId;
            }
            items[idxById[id]] = item;

            if (!updated) {
                updated = {};
            }
            updated[id] = true;
            refresh();
        }

        function insertItem(insertBefore: number, item: any) {
            items.splice(insertBefore, 0, item);
            updateIdxById(insertBefore);
            refresh();
        }

        function addItem(item: any) {
            items.push(item);
            updateIdxById(items.length - 1);
            refresh();
        }

        function deleteItem(id: any) {
            var idx = idxById[id];
            if (idx === undefined) {
                throw "Invalid id";
            }
            delete idxById[id];
            items.splice(idx, 1);
            updateIdxById(idx);
            refresh();
        }

        function sortedAddItem(item: any) {
            insertItem(sortedIndex(item), item);
        }

        function sortedUpdateItem(id: any, item: any) {
            if (idxById[id] === undefined || id !== item[idProperty]) {
                throw new Error("Invalid or non-matching id " + idxById[id]);
            }
            var comparer = getSortComparer();
            var oldItem = getItemById(id);
            if (comparer(oldItem, item) !== 0) {
                // item affects sorting -> must use sorted add
                deleteItem(id);
                sortedAddItem(item);
            }
            else { // update does not affect sorting -> regular update works fine
                updateItem(id, item);
            }
        }

        function sortedIndex(searchItem: any) {
            var low = 0, high = items.length;
            var comparer = getSortComparer();
            while (low < high) {
                var mid = low + high >>> 1;
                if (comparer(items[mid], searchItem) === -1) {
                    low = mid + 1;
                }
                else {
                    high = mid;
                }
            }
            return low;
        }

        function getRows() {
            return rows;
        }

        function getLength() {
            return rows.length;
        }

        function getItem(i: number) {
            var item = rows[i];

            // if this is a group row, make sure totals are calculated and update the title
            if (item && item.__group && item.totals && !item.totals.initialized) {
                var gi = groupingInfos[item.level];
                if (!gi.displayTotalsRow) {
                    calculateTotals(item.totals);
                    item.title = gi.formatter ? gi.formatter(item) : htmlEncode(item.value);
                }
            }
            // if this is a totals row, make sure it's calculated
            else if (item && item.__groupTotals && !item.initialized) {
                calculateTotals(item);
            }

            return item;
        }

        function getItemMetadata(i: number) {
            var item = rows[i];
            if (item === undefined) {
                return null;
            }

            // overrides for grouping rows
            if (item.__group) {
                return groupItemMetadataProvider.getGroupRowMetadata(item);
            }

            // overrides for totals rows
            if (item.__groupTotals) {
                return groupItemMetadataProvider.getTotalsRowMetadata(item);
            }

            return (options.getItemMetadata && options.getItemMetadata(item, i)) || null;
        }

        function expandCollapseAllGroups(level: number, collapse: boolean) {
            if (level == null) {
                for (var i = 0; i < groupingInfos.length; i++) {
                    toggledGroupsByLevel[i] = {};
                    groupingInfos[i].collapsed = collapse;

                    if (collapse === true) {
                        onGroupCollapsed.notify({ level: i, groupingKey: null });
                    } else {
                        onGroupExpanded.notify({ level: i, groupingKey: null });
                    }
                }
            } else {
                toggledGroupsByLevel[level] = {};
                groupingInfos[level].collapsed = collapse;

                if (collapse === true) {
                    onGroupCollapsed.notify({ level: level, groupingKey: null });
                } else {
                    onGroupExpanded.notify({ level: level, groupingKey: null });
                }
            }
            refresh();
        }

        /**
         * @param level {Number} Optional level to collapse.  If not specified, applies to all levels.
         */
        function collapseAllGroups(level: number) {
            expandCollapseAllGroups(level, true);
        }

        /**
         * @param level {Number} Optional level to expand.  If not specified, applies to all levels.
         */
        function expandAllGroups(level: number) {
            expandCollapseAllGroups(level, false);
        }

        function resolveLevelAndGroupingKey(args: any) {
            var arg0 = args[0];
            if (args.length === 1 && arg0.indexOf(groupingDelimiter) !== -1) {
                return { level: arg0.split(groupingDelimiter).length - 1, groupingKey: arg0 };
            } else {
                return { level: args.length - 1, groupingKey: args.join(groupingDelimiter) };
            }
        }

        function expandCollapseGroup(args: any, collapse: any) {
            var opts = resolveLevelAndGroupingKey(args);
            toggledGroupsByLevel[opts.level][opts.groupingKey] = groupingInfos[opts.level].collapsed ^ collapse;
            if (collapse)
                onGroupCollapsed.notify({ level: opts.level, groupingKey: opts.groupingKey });
            else
                onGroupExpanded.notify({ level: opts.level, groupingKey: opts.groupingKey });

            refresh();
        }

        /**
         * @param varArgs Either a Slick.Group's "groupingKey" property, or a
         *     variable argument list of grouping values denoting a unique path to the row.  For
         *     example, calling collapseGroup('high', '10%') will collapse the '10%' subgroup of
         *     the 'high' group.
         */
        function collapseGroup(varArgs: any[]) {
            var args = Array.prototype.slice.call(arguments);
            expandCollapseGroup(args, true);
        }

        /**
         * @param varArgs Either a Slick.Group's "groupingKey" property, or a
         *     variable argument list of grouping values denoting a unique path to the row.  For
         *     example, calling expandGroup('high', '10%') will expand the '10%' subgroup of
         *     the 'high' group.
         */
        function expandGroup(varArgs: any[]) {
            var args = Array.prototype.slice.call(arguments);
            expandCollapseGroup(args, false);
        }

        function getGroups() {
            return groups;
        }

        function getOrCreateGroup(groupsByVal: any, val: any, level: number, parentGroup: any, groups: any[]) {
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

        function extractGroups(rows: any[], parentGroup?: any) {
            var group: any;
            var val: any;
            var groups: any[] = [];
            var groupsByVal = {};
            var r: any;
            var level = parentGroup ? parentGroup.level + 1 : 0;
            var gi = groupingInfos[level];

            for (var i = 0, l: number = gi.predefinedValues.length; i < l; i++) {
                val = gi.predefinedValues[i];
                group = getOrCreateGroup(groupsByVal, val, level, parentGroup, groups);
            }

            for (var i = 0, l = rows.length; i < l; i++) {
                r = rows[i];
                val = gi.getterIsAFn ? gi.getter(r) : r[gi.getter];
                group = getOrCreateGroup(groupsByVal, val, level, parentGroup, groups);

                group.rows[group.count++] = r;
            }

            if (level < groupingInfos.length - 1) {
                for (var i = 0; i < groups.length; i++) {
                    group = groups[i];
                    group.groups = extractGroups(group.rows, group);
                }
            }

            if (groups.length) {
                addTotals(groups, level);
            }

            groups.sort(groupingInfos[level].comparer);

            return groups;
        }

        function calculateTotals(totals: any) {
            var group = totals.group;
            var gi = groupingInfos[group.level];
            var isLeafLevel = (group.level == groupingInfos.length);
            var agg: any, idx = gi.aggregators.length;

            if (!isLeafLevel && gi.aggregateChildGroups) {
                // make sure all the subgroups are calculated
                var i = group.groups.length;
                while (i--) {
                    if (!group.groups[i].totals.initialized) {
                        calculateTotals(group.groups[i].totals);
                    }
                }
            }

            while (idx--) {
                agg = gi.aggregators[idx];
                agg.init();
                if (!isLeafLevel && gi.aggregateChildGroups) {
                    gi.compiledAccumulators[idx].call(agg, group.groups);
                } else {
                    gi.compiledAccumulators[idx].call(agg, group.rows);
                }
                agg.storeResult(totals);
            }
            totals.initialized = true;
        }

        function addGroupTotals(group: any) {
            var gi = groupingInfos[group.level];
            var totals = new GroupTotals<TEntity>();
            totals.group = group;
            group.totals = totals;
            if (!gi.lazyTotalsCalculation) {
                calculateTotals(totals);
            }
        }

        function addTotals(groups: any[], level?: number) {
            level = level || 0;
            var gi = groupingInfos[level];
            var groupCollapsed = gi.collapsed;
            var toggledGroups = toggledGroupsByLevel[level];
            var idx = groups.length, g: any;
            while (idx--) {
                g = groups[idx];

                if (g.collapsed && !gi.aggregateCollapsed) {
                    continue;
                }

                // Do a depth-first aggregation so that parent group aggregators can access subgroup totals.
                if (g.groups) {
                    addTotals(g.groups, level + 1);
                }

                if (gi.aggregators.length && (
                    gi.aggregateEmpty || g.rows.length || (g.groups && g.groups.length))) {
                    addGroupTotals(g);
                }

                g.collapsed = groupCollapsed ^ toggledGroups[g.groupingKey];
                g.title = gi.formatter ? gi.formatter(g) : htmlEncode(g.value);
            }
        }

        function flattenGroupedRows(groups: any[], level?: number) {
            level = level || 0;
            var gi = groupingInfos[level];
            var groupedRows: any[] = [], rows: any[], gl = 0, g: any;
            for (var i = 0, l = groups.length; i < l; i++) {
                g = groups[i];
                groupedRows[gl++] = g;

                if (!g.collapsed) {
                    rows = g.groups ? flattenGroupedRows(g.groups, level + 1) : g.rows;
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

        function getFunctionInfo(fn: any) {
            var fnRegex = /^function[^(]*\(([^)]*)\)\s*{([\s\S]*)}$/;
            var matches = fn.toString().match(fnRegex);
            return {
                params: matches[1].split(","),
                body: matches[2]
            };
        }

        function compileAccumulatorLoop(aggregator: any) {
            var accumulatorInfo = getFunctionInfo(aggregator.accumulate);
            var fn: any = new Function(
                "_items",
                "for (var " + accumulatorInfo.params[0] + ", _i=0, _il=_items.length; _i<_il; _i++) {" +
                accumulatorInfo.params[0] + " = _items[_i]; " +
                accumulatorInfo.body +
                "}"
            );
            return fn;
        }

        function compileFilter() {
            var filterInfo = getFunctionInfo(filter);

            var filterBody = filterInfo.body
                .replace(/return false\s*([;}]|$)/gi, "{ continue _coreloop; }$1")
                .replace(/return true\s*([;}]|$)/gi, "{ _retval[_idx++] = $item$; continue _coreloop; }$1")
                .replace(/return ([^;}]+?)\s*([;}]|$)/gi,
                "{ if ($1) { _retval[_idx++] = $item$; }; continue _coreloop; }$2");

            // This preserves the function template code after JS compression,
            // so that replace() commands still work as expected.
            var tpl = [
                //"function(_items, _args) { ",
                "var _retval = [], _idx = 0; ",
                "var $item$, $args$ = _args; ",
                "_coreloop: ",
                "for (var _i = 0, _il = _items.length; _i < _il; _i++) { ",
                "$item$ = _items[_i]; ",
                "$filter$; ",
                "} ",
                "return _retval; "
                //"}"
            ].join("");
            tpl = tpl.replace(/\$filter\$/gi, filterBody);
            tpl = tpl.replace(/\$item\$/gi, filterInfo.params[0]);
            tpl = tpl.replace(/\$args\$/gi, filterInfo.params[1]);

            var fn: any = new Function("_items,_args", tpl);
            fn.displayName = fn.name = "compiledFilter";
            return fn;
        }

        function compileFilterWithCaching() {
            var filterInfo = getFunctionInfo(filter);

            var filterBody = filterInfo.body
                .replace(/return false\s*([;}]|$)/gi, "{ continue _coreloop; }$1")
                .replace(/return true\s*([;}]|$)/gi, "{ _cache[_i] = true;_retval[_idx++] = $item$; continue _coreloop; }$1")
                .replace(/return ([^;}]+?)\s*([;}]|$)/gi,
                "{ if ((_cache[_i] = $1)) { _retval[_idx++] = $item$; }; continue _coreloop; }$2");

            // This preserves the function template code after JS compression,
            // so that replace() commands still work as expected.
            var tpl = [
                //"function(_items, _args, _cache) { ",
                "var _retval = [], _idx = 0; ",
                "var $item$, $args$ = _args; ",
                "_coreloop: ",
                "for (var _i = 0, _il = _items.length; _i < _il; _i++) { ",
                "$item$ = _items[_i]; ",
                "if (_cache[_i]) { ",
                "_retval[_idx++] = $item$; ",
                "continue _coreloop; ",
                "} ",
                "$filter$; ",
                "} ",
                "return _retval; "
                //"}"
            ].join("");
            tpl = tpl.replace(/\$filter\$/gi, filterBody);
            tpl = tpl.replace(/\$item\$/gi, filterInfo.params[0]);
            tpl = tpl.replace(/\$args\$/gi, filterInfo.params[1]);

            var fn: any = new Function("_items,_args,_cache", tpl);
            var fnName = "compiledFilterWithCaching";
            fn.displayName = fnName;
            fn.name = setFunctionName(fn, fnName);
            return fn;
        }

        /**
         * In ES5 we could set the function name on the fly but in ES6 this is forbidden and we need to set it through differently
         * We can use Object.defineProperty and set it the property to writable, see MDN for reference
         * https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/defineProperty
         * @param {string} fn
         * @param {string} fnName
         */
        function setFunctionName(fn: Function, fnName: string) {
            try {
                Object.defineProperty(fn, 'name', {
                    writable: true,
                    value: fnName
                });
            } catch (err) {
                (fn as any).name = fnName;
            }
        }

        function uncompiledFilter(items: any[], args: any) {
            var retval: any[] = [], idx = 0;

            for (var i = 0, ii = items.length; i < ii; i++) {
                if (filter(items[i], args)) {
                    retval[idx++] = items[i];
                }
            }

            return retval;
        }

        function uncompiledFilterWithCaching(items: any[], args: any, cache: any) {
            var retval: any[] = [], idx = 0, item: any;

            for (var i = 0, ii = items.length; i < ii; i++) {
                item = items[i];
                if (cache[i]) {
                    retval[idx++] = item;
                } else if (filter(item, args)) {
                    retval[idx++] = item;
                    cache[i] = true;
                }
            }

            return retval;
        }

        function getFilteredAndPagedItems(items: any[]) {
            if (filter) {
                var batchFilter = options.inlineFilters ? compiledFilter : uncompiledFilter;
                var batchFilterWithCaching = options.inlineFilters ? compiledFilterWithCaching : uncompiledFilterWithCaching;

                if (refreshHints.isFilterNarrowing) {
                    filteredItems = batchFilter(filteredItems, filterArgs);
                } else if (refreshHints.isFilterExpanding) {
                    filteredItems = batchFilterWithCaching(items, filterArgs, filterCache);
                } else if (!refreshHints.isFilterUnchanged) {
                    filteredItems = batchFilter(items, filterArgs);
                }
            } else {
                // special case:  if not filtering and not paging, the resulting
                // rows collection needs to be a copy so that changes due to sort
                // can be caught
                filteredItems = items.concat();
            }

            // get the current page
            return { totalRows: filteredItems.length, rows: filteredItems };
        }

        function getRowDiffs(rows: any[], newRows: any[]) {
            var item: any, r: any, eitherIsNonData: boolean, diff: any[] = [];
            var from = 0, to = newRows.length;

            if (refreshHints && refreshHints.ignoreDiffsBefore) {
                from = Math.max(0,
                    Math.min(newRows.length, refreshHints.ignoreDiffsBefore));
            }

            if (refreshHints && refreshHints.ignoreDiffsAfter) {
                to = Math.min(newRows.length,
                    Math.max(0, refreshHints.ignoreDiffsAfter));
            }

            for (var i = from, rl = rows.length; i < to; i++) {
                if (i >= rl) {
                    diff[diff.length] = i;
                } else {
                    item = newRows[i];
                    r = rows[i];

                    if ((groupingInfos.length && (eitherIsNonData = (item.__nonDataRow) || (r.__nonDataRow)) &&
                        item.__group !== r.__group ||
                        item.__group && !item.equals(r))
                        || (eitherIsNonData &&
                            // no good way to compare totals since they are arbitrary DTOs
                            // deep object comparison is pretty expensive
                            // always considering them 'dirty' seems easier for the time being
                            (item.__groupTotals || r.__groupTotals))
                        || item[idProperty] != r[idProperty]
                        || (updated && updated[item[idProperty]])
                    ) {
                        diff[diff.length] = i;
                    }
                }
            }
            return diff;
        }

        function recalc(_items: any[]) {
            rowsById = null;

            if (refreshHints.isFilterNarrowing != prevRefreshHints.isFilterNarrowing ||
                refreshHints.isFilterExpanding != prevRefreshHints.isFilterExpanding) {
                filterCache = [];
            }

            var filteredItems = getFilteredAndPagedItems(_items);
            totalRows = filteredItems.totalRows;
            var newRows = filteredItems.rows;

            summaryOptions.totals = {};

            groups = [];
            if (groupingInfos.length) {
                groups = extractGroups(newRows);
                if (groups.length) {
                    newRows = flattenGroupedRows(groups);
                }
            }

            var diff = getRowDiffs(rows, newRows);

            rows = newRows;

            return diff;
        }

        function refresh() {
            if (suspend) {
                return;
            }

            var countBefore = rows.length;
            var totalRowsBefore = totalRows;

            var diff = recalc(items); // pass as direct refs to avoid closure perf hit

            updated = null;
            prevRefreshHints = refreshHints;
            refreshHints = {};

            if (totalRowsBefore !== totalRows) {
                onPagingInfoChanged.notify(getPagingInfo(), null, self);
            }
            if (countBefore !== rows.length) {
                onRowCountChanged.notify({ previous: countBefore, current: rows.length, dataView: self }, null, self);
            }
            if (diff.length > 0) {
                onRowsChanged.notify({ rows: diff, dataView: self }, null, self);
            }
            if (countBefore !== rows.length || diff.length > 0) {
                onRowsOrCountChanged.notify({
                    rowsDiff: diff, previousRowCount: countBefore, currentRowCount: rows.length,
                    rowCountChanged: countBefore !== rows.length, rowsChanged: diff.length > 0, dataView: self
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
         * @param grid {Slick.Grid} The grid to sync selection with.
         * @param preserveHidden {Boolean} Whether to keep selected items that go out of the
         *     view due to them getting filtered out.
         * @param preserveHiddenOnSelectionChange {Boolean} Whether to keep selected items
         *     that are currently out of the view (see preserveHidden) as selected when selection
         *     changes.
         * @return {EventEmitter} An event that notifies when an internal list of selected row ids
         *     changes.  This is useful since, in combination with the above two options, it allows
         *     access to the full list selected row ids, and not just the ones visible to the grid.
         * @method syncGridSelection
         */
        function syncGridSelection(grid: any, preserveHidden: boolean, preserveHiddenOnSelectionChange: boolean) {
            var self = this;
            var inHandler: any;
            var selectedRowIds = self.mapRowsToIds(grid.getSelectedRows());
            var onSelectedRowIdsChanged = new EventEmitter();

            function setSelectedRowIds(rowIds: any[]) {
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

            function update() {
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

            grid.onSelectedRowsChanged.subscribe(function (e: any, args: any) {
                if (inHandler) { return; }
                var newSelectedRowIds = self.mapRowsToIds(grid.getSelectedRows());
                if (!preserveHiddenOnSelectionChange || !grid.getOptions().multiSelect) {
                    setSelectedRowIds(newSelectedRowIds);
                } else {
                    // keep the ones that are hidden
                    var existing = $.grep(selectedRowIds, function (id) { return self.getRowById(id) === undefined; });
                    // add the newly selected ones
                    setSelectedRowIds(existing.concat(newSelectedRowIds));
                }
            });

            this.onRowsChanged.subscribe(update);
            this.onRowCountChanged.subscribe(update);

            return onSelectedRowIdsChanged;
        }

        function syncGridCellCssStyles(grid: Grid, key: string) {
            var hashById: any;
            var inHandler: any;

            // since this method can be called after the cell styles have been set,
            // get the existing ones right away
            storeCellCssStyles(grid.getCellCssStyles(key));

            function storeCellCssStyles(hash: any) {
                hashById = {};
                for (var row in hash) {
                    var id: any = (rows as any)[row][idProperty];
                    hashById[id] = hash[row];
                }   
            }

            function update() {
                if (hashById) {
                    inHandler = true;
                    ensureRowsByIdCache();
                    var newHash: Record<number, any> = {};
                    for (var id in hashById) {
                        var row = rowsById[id];
                        if (row != undefined) {
                            newHash[row] = hashById[id];
                        }
                    }
                    grid.setCellCssStyles(key, newHash);
                    inHandler = false;
                }
            }

            var subFunc = function (e: any, args: any) {
                if (inHandler) { return; }
                if (key != args.key) { return; }
                if (args.hash) {
                    storeCellCssStyles(args.hash);
                }
                else {
                    grid.onCellCssStylesChanged.unsubscribe(subFunc);
                    onRowsOrCountChanged.unsubscribe(update);
                }
            };

            grid.onCellCssStylesChanged.subscribe(subFunc);
            onRowsOrCountChanged.subscribe(update);
        }

        function addData(data: any) {

            if (intf.onProcessData && data)
                data = intf.onProcessData(data, intf) || data;

            errorMessage = null;
            loading && loading.abort();
            loading = false;

            if (!data) {
                errorMessage = intf.errormsg;
                onPagingInfoChanged.notify(getPagingInfo());
                return false;
            }

            data.TotalCount = data.TotalCount || 0;
            data.Entities = data.Entities || [];

            if (!data.Skip || (!intf.rowsPerPage && !data.Take))
                data.Page = 1;
            else
                data.Page = Math.ceil(data.Skip / (data.Take || intf.rowsPerPage)) + 1;

            page = data.Page;
            totalCount = data.TotalCount;

            setItems(data.Entities);

            onPagingInfoChanged.notify(getPagingInfo());
        }

        function populate() {
            if (populateLocks > 0) {
                populateCalls++;
                return;
            }

            populateCalls = 0;

            loading && loading.abort();

            if (intf.onSubmit) {
                var gh = intf.onSubmit(intf);
                if (gh === false)
                    return false;
            }

            onDataLoading.notify(this);

            if (!intf.url)
                return false;

            // set loading event
        
            if (!intf.seekToPage)
                intf.seekToPage = 1;

            var request: ListRequest = {};

            var skip = (intf.seekToPage - 1) * intf.rowsPerPage;
            if (skip)
                request.Skip = skip;
            if (intf.rowsPerPage)
                request.Take = intf.rowsPerPage;

            if (intf.sortBy && intf.sortBy.length) {
                if (typeof intf.sortBy !== "string")
                    request.Sort = intf.sortBy;
                else {
                    request.Sort = [intf.sortBy];
                }
            }

            if (intf.params) {
                request = extend(request, intf.params);
            }

            var dt = dataType;

            var ajaxOptions = {
                cache: false,
                type: intf.method,
                contentType: contentType,
                url: intf.url,
                data: request,
                dataType: dt,
                success: function (response: ServiceResponse) {
                    loading = false;
                    if (response.Error)
                        notifyError(response.Error.Message || response.Error.Code);
                    else
                        addData(response);
                    onDataLoaded.notify(this);
                },
                error: function (xhr: any, status: any, ev: any) {
                    loading = false;

                    if ((xhr.getResponseHeader("content-type") || '').toLowerCase().indexOf("application/json") >= 0) {
                        var json = $.parseJSON(xhr.responseText);
                        if (json != null && json.Error != null) {
                            notifyError(json.Error.Message || json.Error.Code);
                            onPagingInfoChanged.notify(getPagingInfo());
                            onDataLoaded.notify(this);
                            return;
                        }
                    }

                    errorMessage = xhr.errormsg;
                    onPagingInfoChanged.notify(getPagingInfo());
                    onDataLoaded.notify(this);
                },
                complete: function () {
                    loading = false;
                }
            }

            if (intf.onAjaxCall) {
                var ah = intf.onAjaxCall(this, ajaxOptions);
                if (ah === false) {
                    loading = false;
                    onPagingInfoChanged.notify(getPagingInfo());
                    return false;
                }
            }

            ajaxOptions.data = (<any>$).toJSON(ajaxOptions.data);

            onPagingInfoChanged.notify(getPagingInfo());
            loading = $.ajax(ajaxOptions);
        }

        function populateLock() {
            if (populateLocks == 0)
                populateCalls = 0;
            populateLocks++;
        }

        function populateUnlock() {
            if (populateLocks > 0) {
                populateLocks--;
                if (populateLocks == 0 && populateCalls > 0)
                    populate();
            }
        }

        function getGroupItemMetadataProvider() {
            return groupItemMetadataProvider;
        }

        function setGroupItemMetadataProvider(value: GroupItemMetadataProvider) {
            groupItemMetadataProvider = value;
        }

        idProperty = options.idField || 'id';
        contentType = options.contentType || "application/json";
        dataType = options.dataType || 'json';
        filter = options.filter || null;

        intf = {
            // methods
            beginUpdate: beginUpdate,
            endUpdate: endUpdate,
            setPagingOptions: setPagingOptions,
            getPagingInfo: getPagingInfo,
            getIdPropertyName: getIdPropertyName,
            getRows: getRows,
            getItems: getItems,
            setItems: setItems,
            getFilter: getFilter,
            getFilteredItems: getFilteredItems,
            setFilter: setFilter,
            sort: sort,
            fastSort: fastSort,
            reSort: reSort,
            getLocalSort: getLocalSort,
            setLocalSort: setLocalSort,
            setSummaryOptions: setSummaryOptions,
            getGrandTotals: getGrandTotals,
            setGrouping: setGrouping,
            getGrouping: getGrouping,
            collapseAllGroups: collapseAllGroups,
            expandAllGroups: expandAllGroups,
            collapseGroup: collapseGroup,
            expandGroup: expandGroup,
            getGroups: getGroups,
            getIdxById: getIdxById,
            getRowByItem: getRowByItem,
            getRowById: getRowById,
            getItemById: getItemById,
            getItemByIdx: getItemByIdx,
            mapItemsToRows: mapItemsToRows,
            mapRowsToIds: mapRowsToIds,
            mapIdsToRows: mapIdsToRows,
            setRefreshHints: setRefreshHints,
            setFilterArgs: setFilterArgs,
            refresh: refresh,
            updateItem: updateItem,
            insertItem: insertItem,
            addItem: addItem,
            deleteItem: deleteItem,
            sortedAddItem: sortedAddItem,
            sortedUpdateItem: sortedUpdateItem,
            syncGridSelection: syncGridSelection,
            syncGridCellCssStyles: syncGridCellCssStyles,

            getLength: getLength,
            getItem: getItem,
            getItemMetadata: getItemMetadata,
            getGroupItemMetadataProvider: getGroupItemMetadataProvider,
            setGroupItemMetadataProvider: setGroupItemMetadataProvider,

            onRowCountChanged: onRowCountChanged,
            onRowsChanged: onRowsChanged,
            onRowsOrCountChanged: onRowsOrCountChanged,
            onPagingInfoChanged: onPagingInfoChanged,
            onGroupExpanded: onGroupExpanded,
            onGroupCollapsed: onGroupCollapsed,

            addData: addData,
            populate: populate,
            populateLock: populateLock,
            populateUnlock: populateUnlock,
            onDataChanged: onDataChanged,
            onDataLoaded: onDataLoaded,
            onDataLoading: onDataLoading,
            params: options.params || {},
            onSubmit: options.onSubmit || null,
            url: options.url || null,
            rowsPerPage: options.rowsPerPage || 0,
            seekToPage: options.seekToPage || 1,
            onAjaxCall: options.onAjaxCall || null,
            onProcessData: options.onProcessData || null,
            method: options.method || "POST",
            errormsg: localText("Controls.Pager.DefaultLoadError"),
            sortBy: typeof options.sortBy == "string" ? [options.sortBy] : (options.sortBy || []),
            idField: idProperty
        };

        if (options.url && options.autoLoad) {
            populate();
        }

        return intf;
    }
}
