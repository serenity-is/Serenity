// PAGER -----
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
            throw "SlickPager için view opsiyonu belirtilmelidir!";

        this.element.addClass('s-SlickPager slick-pg')
            .html(
                '<div class="slick-pg-in">' + 
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

        $('.slick-pg-reload', this.element).click(function () { v.populate() });
        $('.slick-pg-first', this.element).click(function () { self._changePage('first') });
        $('.slick-pg-prev', this.element).click(function () { self._changePage('prev') });
        $('.slick-pg-next', this.element).click(function () { self._changePage('next') });
        $('.slick-pg-last', this.element).click(function () { self._changePage('last') });
        $('.slick-pg-current', this.element).keydown(function (e) { if (e.keyCode == 13) self._changePage('input') });

        if (self.options.showRowsPerPage) {
            var opt = "";
            for (var nx = 0; nx < o.rowsPerPageOptions.length; nx++) {
                if (v.rowsPerPage == o.rowsPerPageOptions[nx]) 
                    sel = 'selected="selected"'; else sel = '';
                opt += "<option value='" + o.rowsPerPageOptions[nx] + "' " + sel + " >" + o.rowsPerPageOptions[nx] + "&nbsp;&nbsp;</option>";
            };
            $('.slick-pg-in', this.element).prepend('<div class="slick-pg-grp"><select class="slick-pg-size" name="rp">' + opt + '</select></div><div class="slick-pg-sep"></div>');
            $('select.slick-pg-size', this.element).change(
                function () {
                    if (o.onRowsPerPageChange)
                        o.onRowsPerPageChange(+this.value);
                    else {
                        v.newp = 1;
                        v.setPagingOptions({
                            page: 1,
                            rowsPerPage: +this.value 
                        });
                    }
                }
            );
        }

        v.onPagingInfoChanged.subscribe(function() {
            self._updatePager();
        });
    },
    
    destroy: function () {
        $.Widget.prototype.destroy.apply(this, arguments);
    },
    
    _changePage: function (ctype) { //change page

        var view = this.options.view;

        if (!view || view.loading)
            return true;

        var info = view.getPagingInfo();
        var pages = (!info.rowsPerPage || !info.totalRows) ? 1 : Math.ceil(info.totalRows / info.rowsPerPage);

        var newp;

        switch (ctype) {
            case 'first': newp = 1; break;
            case 'prev': if (info.page > 1) newp = parseInt(info.page) - 1; break;
            case 'next': if (info.page < pages) newp = parseInt(info.page) + 1; break;
            case 'last': newp = pages; break;
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
        var pages = (!info.rowsPerPage || !info.totalRows) ? 1 : Math.ceil(info.totalRows / info.rowsPerPage);

        $('.slick-pg-current', this.element).val(info.page);
        $('.slick-pg-total', this.element).html(pages);

        var r1 = (info.page - 1) * info.rowsPerPage + 1;
        var r2 = r1 + info.rowsPerPage - 1;

        if (info.totalRows < r2) 
            r2 = info.totalRows;

        var stat;

        if (info.loading) {
            stat = Q.text("Controls.Pager.LoadingStatus");
        }
        else if (info.error) {
            stat = info.error;
        }
        else if (info.totalRows > 0) {
            stat = Q.text("Controls.Pager.PageStatus");
            stat = stat.replace(/{from}/, r1);
            stat = stat.replace(/{to}/, r2);
            stat = stat.replace(/{total}/, info.totalRows);
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

// EVENT HELPER -----

function EventHelper() {
    this.handlers = [];

    this.subscribe = function(fn) {
        this.handlers.push(fn);
    };
    
    this.unsubscribe = function (fn) {
      for (var i = this.handlers.length - 1; i >= 0; i--) {
        if (this.handlers[i] === fn) {
          this.handlers.splice(i, 1);
        }
      }
    };    
    
    this.clear = function(fn) {
        this.handlers = [];
    };

    this.notify = function(args) {
        for (var i = 0; i < this.handlers.length; i++) {
            this.handlers[i].call(this, args);
        }
    };

    return this;
}

// REMOTE VIEW -----

(function($) {
    function RemoteView(options) {
        var self = this;
        var idField;
        var contentType;
        var dataType;
        var items = [];			// data by index
        var rows = [];			// data by row
        var idxById = {};		// indexes by id
        var rowsById = null;	// rows by id; lazy-calculated
        var filter = null;		// filter function
        var updated = null; 	// updated item ids
        var suspend = 0;	// suspends the recalculation
        var loading = false;
        var errorMessage = null;
        var populateLocks = 0;
        var populateCalls = 0;

        var page = 1;
        var totalRows = null;
        var totalRowsInPage = 0;
        
        var intf;

        // events
        var onRowCountChanged = new EventHelper();
        var onRowsChanged = new EventHelper();
        var onPagingInfoChanged = new EventHelper();
        var onDataLoading = new EventHelper();
        var onDataLoaded = new EventHelper();
        var onClearData = new EventHelper();

        function beginUpdate() {
            suspend++;
        }

        function endUpdate() {
            suspend--;
            if (suspend <= 0)
                refresh();
        }

        function refreshIdxById() {
            idxById = {};
            for (var i = 0,l = items.length; i < l; i++) {
                var id = items[i][idField];
                if (id == undefined || idxById[id] != undefined)
                {
                    var msg = "Each data element must implement a unique '" +
                        idField + "' property. Object at index '" + i + "' ";

                    if (id == undefined)
                        msg += "has no identity value: ";
                    else
                        msg += "has repeated identity value '" +
                            id + "': ";

                    msg += $.toJSON(items[i]);
                    throw msg;
                }
                idxById[id] = i;
            }
        }

        function getItems() {
            return items;
        }

        function setItems(data, fullReset) {
            if (fullReset) {
                var idxById = {};
                var rowsById = null;
            }
            items = data;
            refreshIdxById();
            refresh();
            if (fullReset)
                onRowsChanged.notify();
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
                else if (totalRows == null)
                    newPage = args.page;
                else
                    newPage = Math.min(args.page, Math.ceil(totalRows / intf.rowsPerPage) + 1);
                    
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
            totalRows = data.TotalCount;
            setItems(data.Entities, true);

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

            //request._ = (new Date).getTime();

            var dt = dataType;
            //if (dt === 'json' || dt === 'JSON')
            //    dt = "myJSON";

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

                    if ((xhr.getResponseHeader("content-type") || '').toLowerCase().indexOf("application/json") >= 0)
                    {
                        var json = $.parseJSON(xhr.responseText);
                        if (json != null && json.Error != null)
                        {
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
                complete: function() {
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

            ajaxOptions.data = $.toJSON(ajaxOptions.data);

            onPagingInfoChanged.notify(getPagingInfo());
            loading = $.ajax(ajaxOptions);
        }

        function getPagingInfo() {
            return { rowsPerPage: intf.rowsPerPage, page: page, totalRows: totalRows, loading: loading, error: errorMessage };
        }

        function setFilter(filterFn) {
            filter = filterFn;
            refresh();
        }

        function getItemByIdx(i) {
            return items[i];
        }

        function getIdxById(id) {
            return idxById[id];
        }

        // calculate the lookup table on first call
        function getRowById(id) {
            if (!rowsById) {
                rowsById = {};
                for (var i = 0, l = rows.length; i < l; ++i) {
                    rowsById[rows[i][idField]] = i;
                }
            }

            return rowsById[id];
        }

        function getItemById(id) {
            return items[idxById[id]];
        }

        function updateItem(id, item) {
            if (idxById[id] === undefined || id !== item[idField])
                throw "Invalid or non-matching id";
            items[idxById[id]] = item;
            if (!updated) updated = {};
            updated[id] = true;
            refresh();
        }

        function insertItem(insertBefore, item) {
            items.splice(insertBefore, 0, item);
            refreshIdxById();  // TODO:  optimize
            refresh();
        }

        function addItem(item) {
            items.push(item);
            refreshIdxById();  // TODO:  optimize
            refresh();
        }

        function deleteItem(id) {
            if (idxById[id] === undefined)
                throw "Invalid id";
            items.splice(idxById[id], 1);
            refreshIdxById();  // TODO:  optimize
            refresh();
        }

        function recalc(_items, _rows, _filter, _updated) {
            var diff = [];
            var items = _items, rows = _rows, filter = _filter, updated = _updated; // cache as local vars

            rowsById = null;

            // go over all items remapping them to rows on the fly
            // while keeping track of the differences and updating indexes
            var rl = rows.length;
            var currentRowIndex = 0;
            var currentPageIndex = 0;
            var item,id;

            for (var i = 0, il = items.length; i < il; ++i) {
                item = items[i];
                id = item[idField];

                if (!filter || filter(item, intf)) {
                    if (currentPageIndex >= rl || id != rows[currentPageIndex][idField] || (updated && updated[id]))
                        diff[diff.length] = currentPageIndex;

                    rows[currentPageIndex] = item;
                    currentPageIndex++;
                    currentRowIndex++;
                }
            }

            if (rl > currentPageIndex)
                rows.splice(currentPageIndex, rl - currentPageIndex);

            totalRowsInPage = currentRowIndex;

            return diff;
        }

        function refresh() {
            if (suspend) 
                return;

            var countBefore = rows.length;
            var totalRowsBefore = totalRowsInPage;

            var diff = recalc(items, rows, filter, updated); // pass as direct refs to avoid closure perf hit

            updated = null;

            if (totalRowsBefore != totalRowsInPage) 
                onPagingInfoChanged.notify(getPagingInfo());
            if (countBefore != rows.length) 
                onRowCountChanged.notify({previous:countBefore, current: rows.length});
            if (diff.length > 0) 
                onRowsChanged.notify(diff);
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
            // properties
            "rows": rows,  // note: neither the array or the data in it should be modified directly
            // methods
            "beginUpdate": beginUpdate,
            "endUpdate": endUpdate,
            "setPagingOptions": setPagingOptions,
            "getPagingInfo": getPagingInfo,
            "getItems": getItems,
            "setItems": setItems,
            "addData": addData,
            "setFilter": setFilter,
            "getIdxById": getIdxById,
            "getRowById": getRowById,
            "getItemById": getItemById,
            "getItemByIdx": getItemByIdx,
            "refresh": refresh,
            "updateItem": updateItem,
            "insertItem": insertItem,
            "addItem": addItem,
            "deleteItem": deleteItem,
            "populate": populate,  
            "populateLock": populateLock,
            "populateUnlock": populateUnlock,
            // events
            "onRowCountChanged": onRowCountChanged,
            "onRowsChanged": onRowsChanged,
            "onPagingInfoChanged": onPagingInfoChanged,
            "onDataLoaded": onDataLoaded,
            "onDataLoading": onDataLoading
        };
        
        idField = options.idField || 'id';
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

        if (options.url && options.autoLoad) {
            populate();
        }
        
        return intf;
    }

    $.extend(true, window, { Slick: { Data: { RemoteView: RemoteView }}});
})(jQuery);