(function ($) {
    $.widget("ui.slickPager", {
        options: {
            view: null,
            showRowsPerPage: true,
            rowsPerPageOptions: [20, 100, 500, 2000],
            onRpChange: null
        },
        _create: function () {
            var self = this;
            var o = self.options;
            var v = o.view;
            if (!v)
                throw "SlickPager requires view option to be set!";
            this.element.addClass('s-SlickPager slick-pg')
                .html('<div class="slick-pg-in">' +
                '<div class="slick-pg-grp">' +
                '<div class="slick-pg-first slick-pg-btn"><span class="slick-pg-btn-span"></span></div>' +
                '<div class="slick-pg-prev slick-pg-btn"><span class="slick-pg-btn-span"></span></div>' +
                '</div><div class="slick-pg-sep"></div><div class="slick-pg-grp">' +
                '<span class="slick-pg-control">&nbsp;' + Q.text("Controls.Pager.Page") +
                '&nbsp;<input class="slick-pg-current" type="text" size="4" value="1" /> / ' +
                '<span class="slick-pg-total"> 1 </span></span>' +
                '</div><div class="slick-pg-sep"></div><div class="slick-pg-grp">' +
                '<div class="slick-pg-next slick-pg-btn"><span class="slick-pg-btn-span"></span></div>' +
                '<div class="slick-pg-last slick-pg-btn"><span class="slick-pg-btn-span"></span></div>' +
                '</div><div class="slick-pg-sep"></div><div class="slick-pg-grp">' +
                '<div class="slick-pg-reload slick-pg-btn"><span class="slick-pg-btn-span"></span></div>' +
                '</div><div class="slick-pg-sep"></div>' +
                '<div class="slick-pg-grp"><span class="slick-pg-stat"></span></div></div>');
            $('.slick-pg-reload', this.element).click(function () { v.populate(); });
            $('.slick-pg-first', this.element).click(function () { self._changePage('first'); });
            $('.slick-pg-prev', this.element).click(function () { self._changePage('prev'); });
            $('.slick-pg-next', this.element).click(function () { self._changePage('next'); });
            $('.slick-pg-last', this.element).click(function () { self._changePage('last'); });
            $('.slick-pg-current', this.element).keydown(function (e) { if (e.keyCode == 13)
                self._changePage('input'); });
            if (self.options.showRowsPerPage) {
                var opt, sel = "";
                for (var nx = 0; nx < o.rowsPerPageOptions.length; nx++) {
                    if (v.rowsPerPage == o.rowsPerPageOptions[nx])
                        sel = 'selected="selected"';
                    else
                        sel = '';
                    opt += "<option value='" + o.rowsPerPageOptions[nx] + "' " + sel + " >" + o.rowsPerPageOptions[nx] + "&nbsp;&nbsp;</option>";
                }
                ;
                $('.slick-pg-in', this.element).prepend('<div class="slick-pg-grp"><select class="slick-pg-size" name="rp">' + opt + '</select></div><div class="slick-pg-sep"></div>');
                $('select.slick-pg-size', this.element).change(function () {
                    if (o.onRowsPerPageChange)
                        o.onRowsPerPageChange(+this.value);
                    else {
                        v.newp = 1;
                        v.setPagingOptions({
                            page: 1,
                            rowsPerPage: +this.value
                        });
                    }
                });
            }
            v.onPagingInfoChanged.subscribe(function () {
                self._updatePager();
            });
        },
        destroy: function () {
            $.Widget.prototype.destroy.apply(this, arguments);
        },
        _changePage: function (ctype) {
            var view = this.options.view;
            if (!view || view.loading)
                return true;
            var info = view.getPagingInfo();
            var pages = (!info.rowsPerPage || !info.totalCount) ? 1 : Math.ceil(info.totalCount / info.rowsPerPage);
            var newp;
            switch (ctype) {
                case 'first':
                    newp = 1;
                    break;
                case 'prev':
                    if (info.page > 1)
                        newp = parseInt(info.page) - 1;
                    break;
                case 'next':
                    if (info.page < pages)
                        newp = parseInt(info.page) + 1;
                    break;
                case 'last':
                    newp = pages;
                    break;
                case 'input':
                    var nv = parseInt($('input.slick-pg-current', this.element).val());
                    if (isNaN(nv))
                        nv = 1;
                    else if (nv < 1)
                        nv = 1;
                    else if (nv > pages)
                        nv = pages;
                    $('.slick-pg-current', this.element).val(nv);
                    newp = nv;
                    break;
            }
            if (newp == info.page)
                return false;
            if (this.options.onChangePage)
                this.options.onChangePage(newp);
            else {
                view.setPagingOptions({ page: newp });
            }
        },
        _updatePager: function () {
            var view = this.options.view;
            var info = view.getPagingInfo();
            var pages = (!info.rowsPerPage || !info.totalCount) ? 1 : Math.ceil(info.totalCount / info.rowsPerPage);
            $('.slick-pg-current', this.element).val(info.page);
            $('.slick-pg-total', this.element).html(pages);
            var r1 = (info.page - 1) * info.rowsPerPage + 1;
            var r2 = r1 + info.rowsPerPage - 1;
            if (info.totalCount < r2)
                r2 = info.totalCount;
            var stat;
            if (info.loading) {
                stat = Q.text("Controls.Pager.LoadingStatus");
            }
            else if (info.error) {
                stat = info.error;
            }
            else if (info.totalCount > 0) {
                stat = Q.text("Controls.Pager.PageStatus");
                stat = stat.replace(/{from}/, r1);
                stat = stat.replace(/{to}/, r2);
                stat = stat.replace(/{total}/, info.totalCount);
            }
            else
                stat = Q.text("Controls.Pager.NoRowStatus");
            $('.slick-pg-stat', this.element).html(stat);
            $('.slick-pg-size', this.element).val((info.rowsPerPage || 0).toString());
        },
        _setOption: function (key, value) {
            $.Widget.prototype._setOption.apply(this, arguments);
        }
    });
})(jQuery);
var Slick;
(function (Slick) {
    var Data;
    (function (Data) {
        var GroupItemMetadataProvider;
        var RemoteView = (function () {
            function RemoteView(options) {
                var self = this;
                var defaults = {
                    groupItemMetadataProvider: null,
                    inlineFilters: false
                };
                var idProperty;
                var items = [];
                var rows = [];
                var idxById = {};
                var rowsById = null;
                var filter = null;
                var updated = null;
                var suspend = 0;
                var sortAsc = true;
                var fastSortField;
                var sortComparer;
                var refreshHints = {};
                var prevRefreshHints = {};
                var filterArgs;
                var filteredItems = [];
                var compiledFilter;
                var compiledFilterWithCaching;
                var filterCache = [];
                var groupingInfoDefaults = {
                    getter: null,
                    formatter: null,
                    comparer: function (a, b) {
                        return (a.value === b.value ? 0 :
                            (a.value > b.value ? 1 : -1));
                    },
                    predefinedValues: [],
                    aggregateEmpty: false,
                    aggregateCollapsed: false,
                    aggregateChildGroups: false,
                    collapsed: false,
                    displayTotalsRow: true,
                    lazyTotalsCalculation: false
                };
                var summaryOptions = {};
                var groupingInfos = [];
                var groups = [];
                var toggledGroupsByLevel = [];
                var groupingDelimiter = ':|:';
                var page = 1;
                var totalRows = 0;
                var onRowCountChanged = new Slick.Event();
                var onRowsChanged = new Slick.Event();
                var onPagingInfoChanged = new Slick.Event();
                var loading = false;
                var errorMessage = null;
                var populateLocks = 0;
                var populateCalls = 0;
                var contentType;
                var dataType;
                var totalCount = null;
                var onDataChanged = new Slick.Event();
                var onDataLoading = new Slick.Event();
                var onDataLoaded = new Slick.Event();
                var onClearData = new Slick.Event();
                var intf;
                function beginUpdate() {
                    suspend++;
                }
                function endUpdate() {
                    suspend--;
                    if (suspend <= 0)
                        refresh();
                }
                function setRefreshHints(hints) {
                    refreshHints = hints;
                }
                function setFilterArgs(args) {
                    filterArgs = args;
                }
                function updateIdxById(startingIndex) {
                    startingIndex = startingIndex || 0;
                    var id;
                    for (var i = startingIndex, l = items.length; i < l; i++) {
                        id = items[i][idProperty];
                        if (id === undefined) {
                            var msg = "Each data element must implement a unique '" +
                                idProperty + "' property. Object at index '" + i + "' " +
                                "has no identity value: ";
                            msg += $.toJSON(items[i]);
                            throw msg;
                        }
                        idxById[id] = i;
                    }
                }
                function ensureIdUniqueness() {
                    var id;
                    for (var i = 0, l = items.length; i < l; i++) {
                        id = items[i][idProperty];
                        if (id === undefined || idxById[id] !== i) {
                            var msg = "Each data element must implement a unique '" +
                                idProperty + "' property. Object at index '" + i + "' ";
                            if (id == undefined)
                                msg += "has no identity value: ";
                            else
                                msg += "has repeated identity value '" + id + "': ";
                            msg += $.toJSON(items[i]);
                            throw msg;
                        }
                    }
                }
                function getItems() {
                    return items;
                }
                function setItems(data) {
                    items = filteredItems = data;
                    idxById = {};
                    rowsById = null;
                    summaryOptions.totals = {};
                    updateIdxById();
                    ensureIdUniqueness();
                    refresh();
                    onDataChanged.notify({ dataView: self }, null, self);
                }
                function setPagingOptions(args) {
                    var anyChange = false;
                    if (args.rowsPerPage != undefined &&
                        intf.rowsPerPage != args.rowsPerPage) {
                        intf.rowsPerPage = args.rowsPerPage;
                        anyChange = true;
                    }
                    if (args.page != undefined) {
                        var newPage;
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
                function getPagingInfo() {
                    return {
                        rowsPerPage: intf.rowsPerPage,
                        page: page,
                        totalCount: totalCount,
                        loading: loading,
                        error: errorMessage,
                        dataView: self
                    };
                }
                function sort(comparer, ascending) {
                    sortAsc = ascending;
                    sortComparer = comparer;
                    fastSortField = null;
                    if (ascending === false) {
                        items.reverse();
                    }
                    items.sort(comparer);
                    if (ascending === false) {
                        items.reverse();
                    }
                    idxById = {};
                    updateIdxById();
                    refresh();
                }
                function fastSort(field, ascending) {
                    sortAsc = ascending;
                    fastSortField = field;
                    sortComparer = null;
                    var oldToString = Object.prototype.toString;
                    Object.prototype.toString = (typeof field === "function") ? field : function () {
                        return this[field];
                    };
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
                    if (sortComparer) {
                        sort(sortComparer, sortAsc);
                    }
                    else if (fastSortField) {
                        fastSort(fastSortField, sortAsc);
                    }
                }
                function setFilter(filterFn) {
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
                function setSummaryOptions(summary) {
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
                        var agg, idx = summaryOptions.aggregators.length;
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
                function setGrouping(groupingInfo) {
                    if (!options.groupItemMetadataProvider) {
                        options.groupItemMetadataProvider = new Slick.Data.GroupItemMetadataProvider();
                    }
                    groups = [];
                    toggledGroupsByLevel = [];
                    groupingInfo = groupingInfo || [];
                    groupingInfos = (groupingInfo instanceof Array) ? groupingInfo : [groupingInfo];
                    for (var i = 0; i < groupingInfos.length; i++) {
                        var gi = groupingInfos[i] = $.extend(true, {}, groupingInfoDefaults, groupingInfos[i]);
                        gi.aggregators = gi.aggregators || summaryOptions.aggregators || [];
                        gi.getterIsAFn = typeof gi.getter === "function";
                        gi.compiledAccumulators = [];
                        var idx = gi.aggregators.length;
                        while (idx--) {
                            gi.compiledAccumulators[idx] = compileAccumulatorLoop(gi.aggregators[idx]);
                        }
                        toggledGroupsByLevel[i] = {};
                    }
                    refresh();
                }
                function getItemByIdx(i) {
                    return items[i];
                }
                function getIdxById(id) {
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
                function getRowById(id) {
                    ensureRowsByIdCache();
                    return rowsById[id];
                }
                function getItemById(id) {
                    return items[idxById[id]];
                }
                function mapIdsToRows(idArray) {
                    var rows = [];
                    ensureRowsByIdCache();
                    for (var i = 0, l = idArray.length; i < l; i++) {
                        var row = rowsById[idArray[i]];
                        if (row != null) {
                            rows[rows.length] = row;
                        }
                    }
                    return rows;
                }
                function mapRowsToIds(rowArray) {
                    var ids = [];
                    for (var i = 0, l = rowArray.length; i < l; i++) {
                        if (rowArray[i] < rows.length) {
                            ids[ids.length] = rows[rowArray[i]][idProperty];
                        }
                    }
                    return ids;
                }
                function updateItem(id, item) {
                    if (idxById[id] === undefined || id !== item[idProperty]) {
                        throw "Invalid or non-matching id";
                    }
                    items[idxById[id]] = item;
                    if (!updated) {
                        updated = {};
                    }
                    updated[id] = true;
                    refresh();
                }
                function insertItem(insertBefore, item) {
                    items.splice(insertBefore, 0, item);
                    updateIdxById(insertBefore);
                    refresh();
                }
                function addItem(item) {
                    items.push(item);
                    updateIdxById(items.length - 1);
                    refresh();
                }
                function deleteItem(id) {
                    var idx = idxById[id];
                    if (idx === undefined) {
                        throw "Invalid id";
                    }
                    delete idxById[id];
                    items.splice(idx, 1);
                    updateIdxById(idx);
                    refresh();
                }
                function getRows() {
                    return rows;
                }
                function getLength() {
                    return rows.length;
                }
                function getItem(i) {
                    var item = rows[i];
                    if (item && item.__group && item.totals && !item.totals.initialized) {
                        var gi = groupingInfos[item.level];
                        if (!gi.displayTotalsRow) {
                            calculateTotals(item.totals);
                            item.title = gi.formatter ? gi.formatter(item) : item.value;
                        }
                    }
                    else if (item && item.__groupTotals && !item.initialized) {
                        calculateTotals(item);
                    }
                    return item;
                }
                function getItemMetadata(i) {
                    var item = rows[i];
                    if (item === undefined) {
                        return null;
                    }
                    if (item.__group) {
                        return options.groupItemMetadataProvider.getGroupRowMetadata(item);
                    }
                    if (item.__groupTotals) {
                        return options.groupItemMetadataProvider.getTotalsRowMetadata(item);
                    }
                    return (options.getItemMetadata && options.getItemMetadata(item, i)) || null;
                }
                function expandCollapseAllGroups(level, collapse) {
                    if (level == null) {
                        for (var i = 0; i < groupingInfos.length; i++) {
                            toggledGroupsByLevel[i] = {};
                            groupingInfos[i].collapsed = collapse;
                        }
                    }
                    else {
                        toggledGroupsByLevel[level] = {};
                        groupingInfos[level].collapsed = collapse;
                    }
                    refresh();
                }
                function collapseAllGroups(level) {
                    expandCollapseAllGroups(level, true);
                }
                function expandAllGroups(level) {
                    expandCollapseAllGroups(level, false);
                }
                function resolveLevelAndGroupingKey(args) {
                    var arg0 = args[0];
                    if (args.length === 1 && arg0.indexOf(groupingDelimiter) !== -1) {
                        return { level: arg0.split(groupingDelimiter).length - 1, groupingKey: arg0 };
                    }
                    else {
                        return { level: args.length - 1, groupingKey: args.join(groupingDelimiter) };
                    }
                }
                function expandCollapseGroup(args, collapse) {
                    var opts = resolveLevelAndGroupingKey(args);
                    toggledGroupsByLevel[opts.level][opts.groupingKey] = groupingInfos[opts.level].collapsed ^ collapse;
                    refresh();
                }
                function collapseGroup(varArgs) {
                    var args = Array.prototype.slice.call(arguments);
                    expandCollapseGroup(args, true);
                }
                function expandGroup(varArgs) {
                    var args = Array.prototype.slice.call(arguments);
                    expandCollapseGroup(args, false);
                }
                function getGroups() {
                    return groups;
                }
                function getOrCreateGroup(groupsByVal, val, level, parentGroup, groups) {
                    var group = groupsByVal[val];
                    if (!group) {
                        group = new Slick.Group();
                        group.value = val;
                        group.level = level;
                        group.groupingKey = (parentGroup ? parentGroup.groupingKey + groupingDelimiter : '') + val;
                        groups[groups.length] = group;
                        groupsByVal[val] = group;
                    }
                    return group;
                }
                function extractGroups(rows, parentGroup) {
                    var group;
                    var val;
                    var groups = [];
                    var groupsByVal = {};
                    var r;
                    var level = parentGroup ? parentGroup.level + 1 : 0;
                    var gi = groupingInfos[level];
                    for (var i = 0, l = gi.predefinedValues.length; i < l; i++) {
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
                    groups.sort(groupingInfos[level].comparer);
                    return groups;
                }
                function calculateTotals(totals) {
                    var group = totals.group;
                    var gi = groupingInfos[group.level];
                    var isLeafLevel = (group.level == groupingInfos.length);
                    var agg, idx = gi.aggregators.length;
                    if (!isLeafLevel && gi.aggregateChildGroups) {
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
                        }
                        else {
                            gi.compiledAccumulators[idx].call(agg, group.rows);
                        }
                        agg.storeResult(totals);
                    }
                    totals.initialized = true;
                }
                function addGroupTotals(group) {
                    var gi = groupingInfos[group.level];
                    var totals = new Slick.GroupTotals();
                    totals.group = group;
                    group.totals = totals;
                    if (!gi.lazyTotalsCalculation) {
                        calculateTotals(totals);
                    }
                }
                function addTotals(groups, level) {
                    level = level || 0;
                    var gi = groupingInfos[level];
                    var groupCollapsed = gi.collapsed;
                    var toggledGroups = toggledGroupsByLevel[level];
                    var idx = groups.length, g;
                    while (idx--) {
                        g = groups[idx];
                        if (g.collapsed && !gi.aggregateCollapsed) {
                            continue;
                        }
                        if (g.groups) {
                            addTotals(g.groups, level + 1);
                        }
                        if (gi.aggregators.length && (gi.aggregateEmpty || g.rows.length || (g.groups && g.groups.length))) {
                            addGroupTotals(g);
                        }
                        g.collapsed = groupCollapsed ^ toggledGroups[g.groupingKey];
                        g.title = gi.formatter ? gi.formatter(g) : g.value;
                    }
                }
                function flattenGroupedRows(groups, level) {
                    level = level || 0;
                    var gi = groupingInfos[level];
                    var groupedRows = [], rows, gl = 0, g;
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
                function getFunctionInfo(fn) {
                    var fnRegex = /^function[^(]*\(([^)]*)\)\s*{([\s\S]*)}$/;
                    var matches = fn.toString().match(fnRegex);
                    return {
                        params: matches[1].split(","),
                        body: matches[2]
                    };
                }
                function compileAccumulatorLoop(aggregator) {
                    var accumulatorInfo = getFunctionInfo(aggregator.accumulate);
                    var fn = new Function("_items", "for (var " + accumulatorInfo.params[0] + ", _i=0, _il=_items.length; _i<_il; _i++) {" +
                        accumulatorInfo.params[0] + " = _items[_i]; " +
                        accumulatorInfo.body +
                        "}");
                    fn.displayName = fn.name = "compiledAccumulatorLoop";
                    return fn;
                }
                function compileFilter() {
                    var filterInfo = getFunctionInfo(filter);
                    var filterBody = filterInfo.body
                        .replace(/return false\s*([;}]|$)/gi, "{ continue _coreloop; }$1")
                        .replace(/return true\s*([;}]|$)/gi, "{ _retval[_idx++] = $item$; continue _coreloop; }$1")
                        .replace(/return ([^;}]+?)\s*([;}]|$)/gi, "{ if ($1) { _retval[_idx++] = $item$; }; continue _coreloop; }$2");
                    var tpl = [
                        "var _retval = [], _idx = 0; ",
                        "var $item$, $args$ = _args; ",
                        "_coreloop: ",
                        "for (var _i = 0, _il = _items.length; _i < _il; _i++) { ",
                        "$item$ = _items[_i]; ",
                        "$filter$; ",
                        "} ",
                        "return _retval; "
                    ].join("");
                    tpl = tpl.replace(/\$filter\$/gi, filterBody);
                    tpl = tpl.replace(/\$item\$/gi, filterInfo.params[0]);
                    tpl = tpl.replace(/\$args\$/gi, filterInfo.params[1]);
                    var fn = new Function("_items,_args", tpl);
                    fn.displayName = fn.name = "compiledFilter";
                    return fn;
                }
                function compileFilterWithCaching() {
                    var filterInfo = getFunctionInfo(filter);
                    var filterBody = filterInfo.body
                        .replace(/return false\s*([;}]|$)/gi, "{ continue _coreloop; }$1")
                        .replace(/return true\s*([;}]|$)/gi, "{ _cache[_i] = true;_retval[_idx++] = $item$; continue _coreloop; }$1")
                        .replace(/return ([^;}]+?)\s*([;}]|$)/gi, "{ if ((_cache[_i] = $1)) { _retval[_idx++] = $item$; }; continue _coreloop; }$2");
                    var tpl = [
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
                    ].join("");
                    tpl = tpl.replace(/\$filter\$/gi, filterBody);
                    tpl = tpl.replace(/\$item\$/gi, filterInfo.params[0]);
                    tpl = tpl.replace(/\$args\$/gi, filterInfo.params[1]);
                    var fn = new Function("_items,_args,_cache", tpl);
                    fn.displayName = fn.name = "compiledFilterWithCaching";
                    return fn;
                }
                function uncompiledFilter(items, args) {
                    var retval = [], idx = 0;
                    for (var i = 0, ii = items.length; i < ii; i++) {
                        if (filter(items[i], args)) {
                            retval[idx++] = items[i];
                        }
                    }
                    return retval;
                }
                function uncompiledFilterWithCaching(items, args, cache) {
                    var retval = [], idx = 0, item;
                    for (var i = 0, ii = items.length; i < ii; i++) {
                        item = items[i];
                        if (cache[i]) {
                            retval[idx++] = item;
                        }
                        else if (filter(item, args)) {
                            retval[idx++] = item;
                            cache[i] = true;
                        }
                    }
                    return retval;
                }
                function getFilteredAndPagedItems(items) {
                    if (filter) {
                        var batchFilter = options.inlineFilters ? compiledFilter : uncompiledFilter;
                        var batchFilterWithCaching = options.inlineFilters ? compiledFilterWithCaching : uncompiledFilterWithCaching;
                        if (refreshHints.isFilterNarrowing) {
                            filteredItems = batchFilter(filteredItems, filterArgs);
                        }
                        else if (refreshHints.isFilterExpanding) {
                            filteredItems = batchFilterWithCaching(items, filterArgs, filterCache);
                        }
                        else if (!refreshHints.isFilterUnchanged) {
                            filteredItems = batchFilter(items, filterArgs);
                        }
                    }
                    else {
                        filteredItems = items.concat();
                    }
                    return { totalRows: filteredItems.length, rows: filteredItems };
                }
                function getRowDiffs(rows, newRows) {
                    var item, r, eitherIsNonData, diff = [];
                    var from = 0, to = newRows.length;
                    if (refreshHints && refreshHints.ignoreDiffsBefore) {
                        from = Math.max(0, Math.min(newRows.length, refreshHints.ignoreDiffsBefore));
                    }
                    if (refreshHints && refreshHints.ignoreDiffsAfter) {
                        to = Math.min(newRows.length, Math.max(0, refreshHints.ignoreDiffsAfter));
                    }
                    for (var i = from, rl = rows.length; i < to; i++) {
                        if (i >= rl) {
                            diff[diff.length] = i;
                        }
                        else {
                            item = newRows[i];
                            r = rows[i];
                            if ((groupingInfos.length && (eitherIsNonData = (item.__nonDataRow) || (r.__nonDataRow)) &&
                                item.__group !== r.__group ||
                                item.__group && !item.equals(r))
                                || (eitherIsNonData &&
                                    (item.__groupTotals || r.__groupTotals))
                                || item[idProperty] != r[idProperty]
                                || (updated && updated[item[idProperty]])) {
                                diff[diff.length] = i;
                            }
                        }
                    }
                    return diff;
                }
                function recalc(_items) {
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
                            addTotals(groups);
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
                    var diff = recalc(items);
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
                }
                function syncGridSelection(grid, preserveHidden, preserveHiddenOnSelectionChange) {
                    var self = this;
                    var inHandler;
                    var selectedRowIds = self.mapRowsToIds(grid.getSelectedRows());
                    var onSelectedRowIdsChanged = new Slick.Event();
                    function setSelectedRowIds(rowIds) {
                        if (selectedRowIds.join(",") == rowIds.join(",")) {
                            return;
                        }
                        selectedRowIds = rowIds;
                        onSelectedRowIdsChanged.notify({
                            "grid": grid,
                            "ids": selectedRowIds,
                            "dataView": self
                        }, new Slick.EventData(), self);
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
                    grid.onSelectedRowsChanged.subscribe(function (e, args) {
                        if (inHandler) {
                            return;
                        }
                        var newSelectedRowIds = self.mapRowsToIds(grid.getSelectedRows());
                        if (!preserveHiddenOnSelectionChange || !grid.getOptions().multiSelect) {
                            setSelectedRowIds(newSelectedRowIds);
                        }
                        else {
                            var existing = $.grep(selectedRowIds, function (id) { return self.getRowById(id) === undefined; });
                            setSelectedRowIds(existing.concat(newSelectedRowIds));
                        }
                    });
                    this.onRowsChanged.subscribe(update);
                    this.onRowCountChanged.subscribe(update);
                    return onSelectedRowIdsChanged;
                }
                function syncGridCellCssStyles(grid, key) {
                    var hashById;
                    var inHandler;
                    storeCellCssStyles(grid.getCellCssStyles(key));
                    function storeCellCssStyles(hash) {
                        hashById = {};
                        for (var row in hash) {
                            var id = rows[row][idProperty];
                            hashById[id] = hash[row];
                        }
                    }
                    function update() {
                        if (hashById) {
                            inHandler = true;
                            ensureRowsByIdCache();
                            var newHash = {};
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
                    grid.onCellCssStylesChanged.subscribe(function (e, args) {
                        if (inHandler) {
                            return;
                        }
                        if (key != args.key) {
                            return;
                        }
                        if (args.hash) {
                            storeCellCssStyles(args.hash);
                        }
                    });
                    this.onRowsChanged.subscribe(update);
                    this.onRowCountChanged.subscribe(update);
                }
                function addData(data) {
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
                    var theData = data;
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
                    if (!intf.seekToPage)
                        intf.seekToPage = 1;
                    var request = {};
                    var skip = (intf.seekToPage - 1) * intf.rowsPerPage;
                    if (skip)
                        request.Skip = skip;
                    if (intf.rowsPerPage)
                        request.Take = intf.rowsPerPage;
                    if (intf.sortBy && intf.sortBy.length) {
                        if ($.isArray(intf.sortBy))
                            request.Sort = intf.sortBy;
                        else {
                            request.Sort = [intf.sortBy];
                        }
                    }
                    if (intf.params) {
                        request = $.extend(request, intf.params);
                    }
                    var dt = dataType;
                    var self = this;
                    var ajaxOptions = {
                        cache: false,
                        type: intf.method,
                        contentType: contentType,
                        url: intf.url,
                        data: request,
                        dataType: dt,
                        success: function (response) {
                            loading = false;
                            if (response.Error)
                                Q.notifyError(response.Error.Message || response.Error.Code);
                            else
                                addData(response);
                            onDataLoaded.notify(this);
                        },
                        error: function (xhr, status, ev) {
                            loading = false;
                            if ((xhr.getResponseHeader("content-type") || '').toLowerCase().indexOf("application/json") >= 0) {
                                var json = $.parseJSON(xhr.responseText);
                                if (json != null && json.Error != null) {
                                    Q.notifyError(json.Error.Message || json.Error.Code);
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
                    };
                    if (intf.onAjaxCall) {
                        var ah = intf.onAjaxCall(this, ajaxOptions);
                        if (ah === false) {
                            loading = false;
                            onPagingInfoChanged.notify(getPagingInfo());
                            return false;
                        }
                    }
                    ajaxOptions.data = $.toJSON(ajaxOptions.data);
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
                intf = {
                    "beginUpdate": beginUpdate,
                    "endUpdate": endUpdate,
                    "setPagingOptions": setPagingOptions,
                    "getPagingInfo": getPagingInfo,
                    "getRows": getRows,
                    "getItems": getItems,
                    "setItems": setItems,
                    "setFilter": setFilter,
                    "sort": sort,
                    "fastSort": fastSort,
                    "reSort": reSort,
                    "setSummaryOptions": setSummaryOptions,
                    "getGrandTotals": getGrandTotals,
                    "setGrouping": setGrouping,
                    "getGrouping": getGrouping,
                    "collapseAllGroups": collapseAllGroups,
                    "expandAllGroups": expandAllGroups,
                    "collapseGroup": collapseGroup,
                    "expandGroup": expandGroup,
                    "getGroups": getGroups,
                    "getIdxById": getIdxById,
                    "getRowById": getRowById,
                    "getItemById": getItemById,
                    "getItemByIdx": getItemByIdx,
                    "mapRowsToIds": mapRowsToIds,
                    "mapIdsToRows": mapIdsToRows,
                    "setRefreshHints": setRefreshHints,
                    "setFilterArgs": setFilterArgs,
                    "refresh": refresh,
                    "updateItem": updateItem,
                    "insertItem": insertItem,
                    "addItem": addItem,
                    "deleteItem": deleteItem,
                    "syncGridSelection": syncGridSelection,
                    "syncGridCellCssStyles": syncGridCellCssStyles,
                    "getLength": getLength,
                    "getItem": getItem,
                    "getItemMetadata": getItemMetadata,
                    "onRowCountChanged": onRowCountChanged,
                    "onRowsChanged": onRowsChanged,
                    "onPagingInfoChanged": onPagingInfoChanged,
                    "addData": addData,
                    "populate": populate,
                    "populateLock": populateLock,
                    "populateUnlock": populateUnlock,
                    "onDataChanged": onDataChanged,
                    "onDataLoaded": onDataLoaded,
                    "onDataLoading": onDataLoading
                };
                idProperty = options.idField || 'id';
                contentType = options.contentType || "application/json";
                dataType = options.dataType || 'json';
                filter = options.filter || null;
                intf.params = options.params || {};
                intf.onSubmit = options.onSubmit || null;
                intf.url = options.url || null;
                intf.rowsPerPage = options.rowsPerPage || 0;
                intf.seekToPage = options.seekToPage || 1;
                intf.onAjaxCall = options.onAjaxCall || null;
                intf.onProcessData = options.onProcessData || null;
                intf.method = options.method || 'POST';
                intf.errormsg = intf.errormsg || Q.text("Controls.Pager.DefaultLoadError");
                intf.sortBy = options.sortBy || [];
                intf.idField = idProperty;
                if (options.url && options.autoLoad) {
                    populate();
                }
                return intf;
            }
            return RemoteView;
        })();
        Data.RemoteView = RemoteView;
    })(Data = Slick.Data || (Slick.Data = {}));
})(Slick || (Slick = {}));
var Slick;
(function (Slick) {
    var Data;
    (function (Data) {
        var Aggregators;
        (function (Aggregators) {
            function Avg(field) {
                this.field_ = field;
                this.init = function () {
                    this.count_ = 0;
                    this.nonNullCount_ = 0;
                    this.sum_ = 0;
                };
                this.accumulate = function (item) {
                    var val = item[this.field_];
                    this.count_++;
                    if (val != null && val !== "" && !isNaN(val)) {
                        this.nonNullCount_++;
                        this.sum_ += parseFloat(val);
                    }
                };
                this.storeResult = function (groupTotals) {
                    if (!groupTotals.avg) {
                        groupTotals.avg = {};
                    }
                    if (this.nonNullCount_ != 0) {
                        groupTotals.avg[this.field_] = this.sum_ / this.nonNullCount_;
                    }
                };
            }
            Aggregators.Avg = Avg;
            function WeightedAvg(field, weightedField) {
                this.field_ = field;
                this.weightedField_ = weightedField;
                this.init = function () {
                    this.sum_ = 0;
                    this.weightedSum_ = 0;
                };
                this.accumulate = function (item) {
                    var val = item[this.field_];
                    var valWeighted = item[this.weightedField_];
                    if (this.isValid(val) && this.isValid(valWeighted)) {
                        this.weightedSum_ += parseFloat(valWeighted);
                        this.sum_ += parseFloat(val) * parseFloat(valWeighted);
                    }
                };
                this.storeResult = function (groupTotals) {
                    if (!groupTotals.avg) {
                        groupTotals.avg = {};
                    }
                    if (this.sum_ && this.weightedSum_) {
                        groupTotals.avg[this.field_] = this.sum_ / this.weightedSum_;
                    }
                };
                this.isValid = function (val) {
                    return val !== null && val !== "" && !isNaN(val);
                };
            }
            Aggregators.WeightedAvg = WeightedAvg;
            function Min(field) {
                this.field_ = field;
                this.init = function () {
                    this.min_ = null;
                };
                this.accumulate = function (item) {
                    var val = item[this.field_];
                    if (val != null && val !== "" && !isNaN(val)) {
                        if (this.min_ == null || val < this.min_) {
                            this.min_ = val;
                        }
                    }
                };
                this.storeResult = function (groupTotals) {
                    if (!groupTotals.min) {
                        groupTotals.min = {};
                    }
                    groupTotals.min[this.field_] = this.min_;
                };
            }
            Aggregators.Min = Min;
            function Max(field) {
                this.field_ = field;
                this.init = function () {
                    this.max_ = null;
                };
                this.accumulate = function (item) {
                    var val = item[this.field_];
                    if (val != null && val !== "" && !isNaN(val)) {
                        if (this.max_ == null || val > this.max_) {
                            this.max_ = val;
                        }
                    }
                };
                this.storeResult = function (groupTotals) {
                    if (!groupTotals.max) {
                        groupTotals.max = {};
                    }
                    groupTotals.max[this.field_] = this.max_;
                };
            }
            Aggregators.Max = Max;
            function Sum(field) {
                this.field_ = field;
                this.init = function () {
                    this.sum_ = null;
                };
                this.accumulate = function (item) {
                    var val = item[this.field_];
                    if (val != null && val !== "" && !isNaN(val)) {
                        this.sum_ += parseFloat(val);
                    }
                };
                this.storeResult = function (groupTotals) {
                    if (!groupTotals.sum) {
                        groupTotals.sum = {};
                    }
                    groupTotals.sum[this.field_] = this.sum_;
                };
            }
            Aggregators.Sum = Sum;
        })(Aggregators = Data.Aggregators || (Data.Aggregators = {}));
    })(Data = Slick.Data || (Slick.Data = {}));
})(Slick || (Slick = {}));
//# sourceMappingURL=Serenity.Externals.Slick.js.map