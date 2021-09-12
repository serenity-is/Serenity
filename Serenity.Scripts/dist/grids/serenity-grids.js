var Serenity;
(function (Serenity) {
    var SlickPager = /** @class */ (function (_super) {
        __extends(SlickPager, _super);
        function SlickPager(div, o) {
            var _this = _super.call(this, div, Q.extend({
                showRowsPerPage: true,
                rowsPerPageOptions: [20, 100, 500, 2000]
            }, o)) || this;
            o = _this.options;
            var v = o.view;
            if (!v)
                throw "SlickPager requires view option to be set!";
            _this.element.addClass('s-SlickPager slick-pg')
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
            var self = _this;
            $('.slick-pg-reload', _this.element).click(function () { v.populate(); });
            $('.slick-pg-first', _this.element).click(function () { self._changePage('first'); });
            $('.slick-pg-prev', _this.element).click(function () { self._changePage('prev'); });
            $('.slick-pg-next', _this.element).click(function () { self._changePage('next'); });
            $('.slick-pg-last', _this.element).click(function () { self._changePage('last'); });
            $('.slick-pg-current', _this.element).keydown(function (e) { if (e.keyCode == 13)
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
                $('.slick-pg-in', _this.element).prepend('<div class="slick-pg-grp"><select class="slick-pg-size" name="rp">' + opt + '</select></div><div class="slick-pg-sep"></div>');
                $('select.slick-pg-size', _this.element).change(function () {
                    if (o.onRowsPerPageChange)
                        o.onRowsPerPageChange(+this.value);
                    else {
                        v["newp"] = 1;
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
            return _this;
        }
        SlickPager.prototype._changePage = function (ctype) {
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
        };
        SlickPager.prototype._updatePager = function () {
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
        };
        return SlickPager;
    }(Serenity.Widget));
    Serenity.SlickPager = SlickPager;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var GridRowSelectionMixin = /** @class */ (function () {
        function GridRowSelectionMixin(grid, options) {
            var _this = this;
            this.include = {};
            this.grid = grid;
            this.idField = grid.getView().idField;
            this.options = options || {};
            grid.getGrid().onClick.subscribe(function (e, p) {
                if ($(e.target).hasClass('select-item')) {
                    e.preventDefault();
                    var item = grid.getView().getItem(p.row);
                    var id = item[_this.idField].toString();
                    if (_this.include[id]) {
                        delete _this.include[id];
                    }
                    else {
                        _this.include[id] = true;
                    }
                    for (var i = 0; i < grid.getView().getLength(); i++) {
                        grid.getGrid().updateRow(i);
                    }
                    _this.updateSelectAll();
                }
            });
            grid.getGrid().onHeaderClick.subscribe(function (e1, u) {
                if (e1.isDefaultPrevented()) {
                    return;
                }
                if ($(e1.target).hasClass('select-all-items')) {
                    e1.preventDefault();
                    var view = grid.getView();
                    if (Object.keys(_this.include).length > 0) {
                        Q.clearKeys(_this.include);
                    }
                    else {
                        var items = grid.getView().getItems();
                        for (var _i = 0, _a = items.filter(_this.isSelectable.bind(_this)); _i < _a.length; _i++) {
                            var x = _a[_i];
                            var id1 = x[_this.idField];
                            _this.include[id1] = true;
                        }
                    }
                    _this.updateSelectAll();
                    grid.getView().setItems(grid.getView().getItems(), true);
                }
            });
            grid.getView().onRowsChanged.subscribe(function () {
                return _this.updateSelectAll();
            });
        }
        GridRowSelectionMixin.prototype.updateSelectAll = function () {
            var selectAllButton = this.grid.getElement()
                .find('.select-all-header .slick-column-name .select-all-items');
            if (selectAllButton) {
                var keys = Object.keys(this.include);
                selectAllButton.toggleClass('checked', keys.length > 0 &&
                    this.grid.getView().getItems().filter(this.isSelectable.bind(this)).length <= keys.length);
            }
        };
        GridRowSelectionMixin.prototype.clear = function () {
            Q.clearKeys(this.include);
            this.updateSelectAll();
        };
        GridRowSelectionMixin.prototype.resetCheckedAndRefresh = function () {
            this.include = {};
            this.updateSelectAll();
            this.grid.getView().populate();
        };
        GridRowSelectionMixin.prototype.selectKeys = function (keys) {
            for (var _i = 0, keys_1 = keys; _i < keys_1.length; _i++) {
                var k = keys_1[_i];
                this.include[k] = true;
            }
            this.updateSelectAll();
        };
        GridRowSelectionMixin.prototype.getSelectedKeys = function () {
            return Object.keys(this.include);
        };
        GridRowSelectionMixin.prototype.getSelectedAsInt32 = function () {
            return Object.keys(this.include).map(function (x) {
                return parseInt(x, 10);
            });
        };
        GridRowSelectionMixin.prototype.getSelectedAsInt64 = function () {
            return Object.keys(this.include).map(function (x) {
                return parseInt(x, 10);
            });
        };
        GridRowSelectionMixin.prototype.setSelectedKeys = function (keys) {
            this.clear();
            for (var _i = 0, keys_2 = keys; _i < keys_2.length; _i++) {
                var k = keys_2[_i];
                this.include[k] = true;
            }
            this.updateSelectAll();
        };
        GridRowSelectionMixin.prototype.isSelectable = function (item) {
            return item && (this.options.selectable == null ||
                this.options.selectable(item));
        };
        GridRowSelectionMixin.createSelectColumn = function (getMixin) {
            return {
                name: '<span class="select-all-items check-box no-float "></span>',
                toolTip: ' ',
                field: '__select__',
                width: 27,
                minWidth: 27,
                headerCssClass: 'select-all-header',
                sortable: false,
                format: function (ctx) {
                    var item = ctx.item;
                    var mixin = getMixin();
                    if (!mixin || !mixin.isSelectable(item)) {
                        return '';
                    }
                    var isChecked = mixin.include[ctx.item[mixin.idField]];
                    return '<span class="select-item check-box no-float ' + (isChecked ? ' checked' : '') + '"></span>';
                }
            };
        };
        GridRowSelectionMixin = __decorate([
            Serenity.Decorators.registerClass('Serenity.GridRowSelectionMixin')
        ], GridRowSelectionMixin);
        return GridRowSelectionMixin;
    }());
    Serenity.GridRowSelectionMixin = GridRowSelectionMixin;
    var GridRadioSelectionMixin = /** @class */ (function () {
        function GridRadioSelectionMixin(grid, options) {
            var _this = this;
            this.include = {};
            this.grid = grid;
            this.idField = grid.getView().idField;
            this.options = options || {};
            grid.getGrid().onClick.subscribe(function (e, p) {
                if ($(e.target).hasClass('rad-select-item')) {
                    e.preventDefault();
                    var item = grid.getView().getItem(p.row);
                    if (!_this.isSelectable(item)) {
                        return;
                    }
                    var id = item[_this.idField].toString();
                    if (_this.include[id] == true) {
                        Q.clearKeys(_this.include);
                    }
                    else {
                        Q.clearKeys(_this.include);
                        _this.include[id] = true;
                    }
                    for (var i = 0; i < grid.getView().getLength(); i++) {
                        grid.getGrid().updateRow(i);
                    }
                }
            });
        }
        GridRadioSelectionMixin.prototype.isSelectable = function (item) {
            return item && (this.options.selectable == null ||
                this.options.selectable(item));
        };
        GridRadioSelectionMixin.prototype.clear = function () {
            Q.clearKeys(this.include);
        };
        GridRadioSelectionMixin.prototype.resetCheckedAndRefresh = function () {
            this.include = {};
            this.grid.getView().populate();
        };
        GridRadioSelectionMixin.prototype.getSelectedKey = function () {
            var items = Object.keys(this.include);
            if (items != null && items.length > 0) {
                return items[0];
            }
            return null;
        };
        GridRadioSelectionMixin.prototype.getSelectedAsInt32 = function () {
            var items = Object.keys(this.include).map(function (x) {
                return parseInt(x, 10);
            });
            if (items != null && items.length > 0) {
                return items[0];
            }
            return null;
        };
        GridRadioSelectionMixin.prototype.getSelectedAsInt64 = function () {
            var items = Object.keys(this.include).map(function (x) {
                return parseInt(x, 10);
            });
            if (items != null && items.length > 0) {
                return items[0];
            }
            return null;
        };
        GridRadioSelectionMixin.prototype.setSelectedKey = function (key) {
            this.clear();
            this.include[key] = true;
        };
        GridRadioSelectionMixin.createSelectColumn = function (getMixin) {
            return {
                name: '',
                toolTip: ' ',
                field: '__select__',
                width: 27,
                minWidth: 27,
                headerCssClass: '',
                sortable: false,
                formatter: function (row, cell, value, column, item) {
                    var mixin = getMixin();
                    if (!mixin || !mixin.isSelectable(item)) {
                        return '';
                    }
                    var isChecked = mixin.include[item[mixin.idField]];
                    return '<input type="radio" name="radio-selection-group" class="rad-select-item no-float" style="cursor: pointer;width: 13px; height:13px;" ' + (isChecked ? ' checked' : '') + ' /> ';
                }
            };
        };
        GridRadioSelectionMixin = __decorate([
            Serenity.Decorators.registerClass('Serenity.GridRadioSelectionMixin')
        ], GridRadioSelectionMixin);
        return GridRadioSelectionMixin;
    }());
    Serenity.GridRadioSelectionMixin = GridRadioSelectionMixin;
    var GridSelectAllButtonHelper;
    (function (GridSelectAllButtonHelper) {
        function update(grid, getSelected) {
            var toolbar = grid.element.children('.s-Toolbar');
            if (toolbar.length === 0) {
                return;
            }
            var btn = toolbar.getWidget(Serenity.Toolbar).findButton('select-all-button');
            var items = grid.getView().getItems();
            btn.toggleClass('checked', items.length > 0 && !items.some(function (x) {
                return !getSelected(x);
            }));
        }
        GridSelectAllButtonHelper.update = update;
        function define(getGrid, getId, getSelected, setSelected, text, onClick) {
            if (text == null) {
                text = Q.coalesce(Q.tryGetText('Controls.CheckTreeEditor.SelectAll'), 'Select All');
            }
            return {
                title: text,
                cssClass: 'select-all-button',
                onClick: function () {
                    var grid = getGrid();
                    var view = grid.getView();
                    var btn = grid.element.children('.s-Toolbar')
                        .getWidget(Serenity.Toolbar).findButton('select-all-button');
                    var makeSelected = !btn.hasClass('checked');
                    view.beginUpdate();
                    try {
                        for (var _i = 0, _a = view.getItems(); _i < _a.length; _i++) {
                            var item = _a[_i];
                            setSelected(item, makeSelected);
                            view.updateItem(getId(item), item);
                        }
                        onClick && onClick();
                    }
                    finally {
                        view.endUpdate();
                    }
                    btn.toggleClass('checked', makeSelected);
                }
            };
        }
        GridSelectAllButtonHelper.define = define;
    })(GridSelectAllButtonHelper = Serenity.GridSelectAllButtonHelper || (Serenity.GridSelectAllButtonHelper = {}));
    var GridUtils;
    (function (GridUtils) {
        function addToggleButton(toolDiv, cssClass, callback, hint, initial) {
            var div = $('<div><a href="#"></a></div>')
                .addClass('s-ToggleButton').addClass(cssClass)
                .prependTo(toolDiv);
            div.children('a').click(function (e) {
                e.preventDefault();
                div.toggleClass('pressed');
                var pressed = div.hasClass('pressed');
                callback && callback(pressed);
            }).attr('title', Q.coalesce(hint, ''));
            if (initial) {
                div.addClass('pressed');
            }
        }
        GridUtils.addToggleButton = addToggleButton;
        function addIncludeDeletedToggle(toolDiv, view, hint, initial) {
            var includeDeleted = false;
            var oldSubmit = view.onSubmit;
            view.onSubmit = function (v) {
                v.params.IncludeDeleted = includeDeleted;
                if (oldSubmit != null) {
                    return oldSubmit(v);
                }
                return true;
            };
            if (hint == null)
                hint = Q.text('Controls.EntityGrid.IncludeDeletedToggle');
            addToggleButton(toolDiv, 's-IncludeDeletedToggle', function (pressed) {
                includeDeleted = pressed;
                view.seekToPage = 1;
                view.populate();
            }, hint, initial);
            toolDiv.bind('remove', function () {
                view.onSubmit = null;
                oldSubmit = null;
            });
        }
        GridUtils.addIncludeDeletedToggle = addIncludeDeletedToggle;
        function addQuickSearchInput(toolDiv, view, fields, onChange) {
            var oldSubmit = view.onSubmit;
            var input;
            view.onSubmit = function (v) {
                var _a;
                if (input) {
                    var searchText = input.get_value();
                    if (searchText && searchText.length > 0) {
                        v.params.ContainsText = searchText;
                    }
                    else {
                        delete v.params['ContainsText'];
                    }
                    var searchField = (_a = input.get_field()) === null || _a === void 0 ? void 0 : _a.name;
                    if (searchField != null && searchField.length > 0) {
                        v.params.ContainsField = searchField;
                    }
                    else {
                        delete v.params['ContainsField'];
                    }
                }
                if (oldSubmit != null)
                    return oldSubmit(v);
                return true;
            };
            var lastDoneEvent = null;
            input = addQuickSearchInputCustom(toolDiv, function (field, query, done) {
                onChange && onChange();
                view.seekToPage = 1;
                lastDoneEvent = done;
                view.populate();
            }, fields);
            view.onDataLoaded.subscribe(function (e, ui) {
                if (lastDoneEvent != null) {
                    lastDoneEvent(view.getLength() > 0);
                    lastDoneEvent = null;
                }
            });
        }
        GridUtils.addQuickSearchInput = addQuickSearchInput;
        function addQuickSearchInputCustom(container, onSearch, fields) {
            var div = $('<div><input type="text"/></div>')
                .addClass('s-QuickSearchBar').prependTo(container);
            if (fields != null && fields.length > 0) {
                div.addClass('has-quick-search-fields');
            }
            return new Serenity.QuickSearchInput(div.children(), {
                fields: fields,
                onSearch: onSearch
            });
        }
        GridUtils.addQuickSearchInputCustom = addQuickSearchInputCustom;
        function makeOrderable(grid, handleMove) {
            var moveRowsPlugin = new Slick.RowMoveManager({ cancelEditOnDrag: true });
            moveRowsPlugin.onBeforeMoveRows.subscribe(function (e, data) {
                for (var i = 0; !!(i < data.rows.length); i++) {
                    if (!!(data.rows[i] === data.insertBefore ||
                        data.rows[i] === data.insertBefore - 1)) {
                        e.stopPropagation();
                        return false;
                    }
                }
                return true;
            });
            moveRowsPlugin.onMoveRows.subscribe(function (e1, data1) {
                handleMove(data1.rows, data1.insertBefore);
                try {
                    grid.setSelectedRows([]);
                }
                catch ($t1) {
                }
            });
            grid.registerPlugin(moveRowsPlugin);
        }
        GridUtils.makeOrderable = makeOrderable;
        function makeOrderableWithUpdateRequest(grid, getId, getDisplayOrder, service, getUpdateRequest) {
            makeOrderable(grid.slickGrid, function (rows, insertBefore) {
                if (rows.length === 0) {
                    return;
                }
                var order;
                var index = insertBefore;
                if (index < 0) {
                    order = 1;
                }
                else if (insertBefore >= grid.rowCount()) {
                    order = Q.coalesce(getDisplayOrder(grid.itemAt(grid.rowCount() - 1)), 0);
                    if (order === 0) {
                        order = insertBefore + 1;
                    }
                    else {
                        order = order + 1;
                    }
                }
                else {
                    order = Q.coalesce(getDisplayOrder(grid.itemAt(insertBefore)), 0);
                    if (order === 0) {
                        order = insertBefore + 1;
                    }
                }
                var i = 0;
                var next = null;
                next = function () {
                    Q.serviceCall({
                        service: service,
                        request: getUpdateRequest(getId(grid.itemAt(rows[i])), order++),
                        onSuccess: function (response) {
                            i++;
                            if (i < rows.length) {
                                next();
                            }
                            else {
                                grid.view.populate();
                            }
                        }
                    });
                };
                next();
            });
        }
        GridUtils.makeOrderableWithUpdateRequest = makeOrderableWithUpdateRequest;
    })(GridUtils = Serenity.GridUtils || (Serenity.GridUtils = {}));
    var PropertyItemSlickConverter;
    (function (PropertyItemSlickConverter) {
        function toSlickColumns(items) {
            var result = [];
            if (items == null) {
                return result;
            }
            for (var i = 0; i < items.length; i++) {
                result.push(toSlickColumn(items[i]));
            }
            return result;
        }
        PropertyItemSlickConverter.toSlickColumns = toSlickColumns;
        function toSlickColumn(item) {
            var result = {
                field: item.name,
                sourceItem: item,
                cssClass: item.cssClass,
                headerCssClass: item.headerCssClass,
                sortable: item.sortable !== false,
                sortOrder: Q.coalesce(item.sortOrder, 0),
                width: item.width != null ? item.width : 80,
                minWidth: Q.coalesce(item.minWidth, 30),
                maxWidth: (item.maxWidth == null || item.maxWidth === 0) ? null : item.maxWidth,
                resizable: item.resizable == null || !!item.resizable
            };
            result.visible = item.visible !== false && item.filterOnly !== true &&
                (item.readPermission == null || Q.Authorization.hasPermission(item.readPermission));
            var name = Q.tryGetText(item.title);
            if (name == null)
                name = item.title;
            result.name = name;
            if (item.alignment != null && item.alignment.length > 0) {
                if (!Q.isEmptyOrNull(result.cssClass)) {
                    result.cssClass += ' align-' + item.alignment;
                }
                else {
                    result.cssClass = 'align-' + item.alignment;
                }
            }
            if (item.formatterType != null && item.formatterType.length > 0) {
                var formatterType = Serenity.FormatterTypeRegistry.get(item.formatterType);
                var formatter = new formatterType();
                if (item.formatterParams != null) {
                    Serenity.ReflectionOptionsSetter.set(formatter, item.formatterParams);
                }
                var initializer = Q.safeCast(formatter, Serenity.IInitializeColumn);
                if (initializer != null) {
                    initializer.initializeColumn(result);
                }
                result.format = function (ctx) { return formatter.format(ctx); };
            }
            return result;
        }
        PropertyItemSlickConverter.toSlickColumn = toSlickColumn;
    })(PropertyItemSlickConverter = Serenity.PropertyItemSlickConverter || (Serenity.PropertyItemSlickConverter = {}));
    var SlickFormatting;
    (function (SlickFormatting) {
        function getEnumText(enumKey, name) {
            return Serenity.EnumFormatter.getText(enumKey, name);
        }
        SlickFormatting.getEnumText = getEnumText;
        function treeToggle(getView, getId, formatter) {
            return function (ctx) {
                var text = formatter(ctx);
                var view = getView();
                var indent = Q.coalesce(ctx.item._indent, 0);
                var spacer = '<span class="s-TreeIndent" style="width:' + 15 * indent + 'px"></span>';
                var id = getId(ctx.item);
                var idx = view.getIdxById(id);
                var next = view.getItemByIdx(idx + 1);
                if (next != null) {
                    var nextIndent = Q.coalesce(next._indent, 0);
                    if (nextIndent > indent) {
                        if (!!!!ctx.item._collapsed) {
                            return spacer + '<span class="s-TreeToggle s-TreeExpand"></span>' + text;
                        }
                        else {
                            return spacer + '<span class="s-TreeToggle s-TreeCollapse"></span>' + text;
                        }
                    }
                }
                return spacer + '<span class="s-TreeToggle"></span>' + text;
            };
        }
        SlickFormatting.treeToggle = treeToggle;
        function date(format) {
            if (format == null) {
                format = Q.Culture.dateFormat;
            }
            return function (ctx) {
                return Q.htmlEncode(Serenity.DateFormatter.format(ctx.value, format));
            };
        }
        SlickFormatting.date = date;
        function dateTime(format) {
            if (format == null) {
                format = Q.Culture.dateTimeFormat;
            }
            return function (ctx) {
                return Q.htmlEncode(Serenity.DateFormatter.format(ctx.value, format));
            };
        }
        SlickFormatting.dateTime = dateTime;
        function checkBox() {
            return function (ctx) {
                return '<span class="check-box no-float ' + (!!ctx.value ? ' checked' : '') + '"></span>';
            };
        }
        SlickFormatting.checkBox = checkBox;
        function number(format) {
            return function (ctx) {
                return Serenity.NumberFormatter.format(ctx.value, format);
            };
        }
        SlickFormatting.number = number;
        function getItemType(link) {
            return link.data('item-type');
        }
        SlickFormatting.getItemType = getItemType;
        function getItemId(link) {
            var value = link.data('item-id');
            return value == null ? null : value.toString();
        }
        SlickFormatting.getItemId = getItemId;
        function itemLinkText(itemType, id, text, extraClass, encode) {
            return '<a' + (id != null ? (' href="#' + Q.replaceAll(itemType, '.', '-') +
                '/' + id + '"') : '') + ' data-item-type="' +
                Q.attrEncode(itemType) + '"' + ' data-item-id="' +
                Q.attrEncode(id) + '"' + ' class="s-EditLink s-' +
                Q.replaceAll(itemType, '.', '-') + 'Link' +
                (Q.isEmptyOrNull(extraClass) ? '' : (' ' + extraClass)) + '">' +
                (encode ? Q.htmlEncode(Q.coalesce(text, '')) : Q.coalesce(text, '')) + '</a>';
        }
        SlickFormatting.itemLinkText = itemLinkText;
        function itemLink(itemType, idField, getText, cssClass, encode) {
            return function (ctx) {
                return itemLinkText(itemType, ctx.item[idField], (getText == null ? ctx.value : getText(ctx)), (cssClass == null ? '' : cssClass(ctx)), encode);
            };
        }
        SlickFormatting.itemLink = itemLink;
    })(SlickFormatting = Serenity.SlickFormatting || (Serenity.SlickFormatting = {}));
    var SlickHelper;
    (function (SlickHelper) {
        function setDefaults(columns, localTextPrefix) {
            for (var _i = 0, columns_1 = columns; _i < columns_1.length; _i++) {
                var col = columns_1[_i];
                col.sortable = (col.sortable != null ? col.sortable : true);
                var id = col.id;
                if (id == null) {
                    id = col.field;
                }
                col.id = id;
                if (localTextPrefix != null && col.id != null &&
                    (col.name == null || Q.startsWith(col.name, '~'))) {
                    var key = (col.name != null ? col.name.substr(1) : col.id);
                    col.name = Q.text(localTextPrefix + key);
                }
                if (col.formatter == null && col.format != null) {
                    col.formatter = convertToFormatter(col.format);
                }
                else if (col.formatter == null) {
                    col.formatter = function (row, cell, value, column, item) {
                        return Q.htmlEncode(value);
                    };
                }
            }
            return columns;
        }
        SlickHelper.setDefaults = setDefaults;
        function convertToFormatter(format) {
            if (format == null) {
                return null;
            }
            else {
                return function (row, cell, value, column, item) {
                    return format({
                        row: row,
                        cell: cell,
                        value: value,
                        column: column,
                        item: item
                    });
                };
            }
        }
        SlickHelper.convertToFormatter = convertToFormatter;
    })(SlickHelper = Serenity.SlickHelper || (Serenity.SlickHelper = {}));
    var SlickTreeHelper;
    (function (SlickTreeHelper) {
        function filterCustom(item, getParent) {
            var parent = getParent(item);
            var loop = 0;
            while (parent != null) {
                if (!!parent._collapsed) {
                    return false;
                }
                parent = getParent(parent);
                if (loop++ > 1000) {
                    throw new Error('Possible infinite loop, check parents has no circular reference!');
                }
            }
            return true;
        }
        SlickTreeHelper.filterCustom = filterCustom;
        function filterById(item, view, getParentId) {
            return filterCustom(item, function (x) {
                var parentId = getParentId(x);
                if (parentId == null) {
                    return null;
                }
                return view.getItemById(parentId);
            });
        }
        SlickTreeHelper.filterById = filterById;
        function setCollapsed(items, collapsed) {
            if (items != null) {
                for (var _i = 0, items_1 = items; _i < items_1.length; _i++) {
                    var item = items_1[_i];
                    item._collapsed = collapsed;
                }
            }
        }
        SlickTreeHelper.setCollapsed = setCollapsed;
        function setCollapsedFlag(item, collapsed) {
            item._collapsed = collapsed;
        }
        SlickTreeHelper.setCollapsedFlag = setCollapsedFlag;
        function setIndents(items, getId, getParentId, setCollapsed) {
            var depth = 0;
            var depths = {};
            for (var line = 0; line < items.length; line++) {
                var item = items[line];
                if (line > 0) {
                    var parentId = getParentId(item);
                    if (parentId != null && parentId === getId(items[line - 1])) {
                        depth += 1;
                    }
                    else if (parentId == null) {
                        depth = 0;
                    }
                    else if (parentId !== getParentId(items[line - 1])) {
                        if (depths[parentId] != null) {
                            depth = depths[parentId] + 1;
                        }
                        else {
                            depth = 0;
                        }
                    }
                }
                depths[getId(item)] = depth;
                item._indent = depth;
                if (setCollapsed != null) {
                    item._collapsed = setCollapsed;
                }
            }
        }
        SlickTreeHelper.setIndents = setIndents;
        function toggleClick(e, row, cell, view, getId) {
            var target = $(e.target);
            if (!target.hasClass('s-TreeToggle')) {
                return;
            }
            if (target.hasClass('s-TreeCollapse') || target.hasClass('s-TreeExpand')) {
                var item = view.getItem(row);
                if (item != null) {
                    if (!!!item._collapsed) {
                        item._collapsed = true;
                    }
                    else {
                        item._collapsed = false;
                    }
                    view.updateItem(getId(item), item);
                }
                if (e.shiftKey) {
                    view.beginUpdate();
                    try {
                        setCollapsed(view.getItems(), !!item._collapsed);
                        view.setItems(view.getItems(), true);
                    }
                    finally {
                        view.endUpdate();
                    }
                }
            }
        }
        SlickTreeHelper.toggleClick = toggleClick;
    })(SlickTreeHelper = Serenity.SlickTreeHelper || (Serenity.SlickTreeHelper = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IInitializeColumn = /** @class */ (function () {
        function IInitializeColumn() {
        }
        IInitializeColumn = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IInitializeColumn')
        ], IInitializeColumn);
        return IInitializeColumn;
    }());
    Serenity.IInitializeColumn = IInitializeColumn;
    var Option = Serenity.Decorators.option;
    function Formatter(name, intf) {
        return Serenity.Decorators.registerFormatter('Serenity.' + name + 'Formatter', intf);
    }
    var BooleanFormatter = /** @class */ (function () {
        function BooleanFormatter() {
        }
        BooleanFormatter.prototype.format = function (ctx) {
            if (ctx.value == null) {
                return '';
            }
            var text;
            if (!!ctx.value) {
                text = Q.tryGetText(this.trueText);
                if (text == null) {
                    text = this.trueText;
                    if (text == null) {
                        text = Q.coalesce(Q.tryGetText('Dialogs.YesButton'), 'Yes');
                    }
                }
            }
            else {
                text = Q.tryGetText(this.falseText);
                if (text == null) {
                    text = this.falseText;
                    if (text == null) {
                        text = Q.coalesce(Q.tryGetText('Dialogs.NoButton'), 'No');
                    }
                }
            }
            return Q.htmlEncode(text);
        };
        __decorate([
            Option()
        ], BooleanFormatter.prototype, "falseText", void 0);
        __decorate([
            Option()
        ], BooleanFormatter.prototype, "trueText", void 0);
        BooleanFormatter = __decorate([
            Formatter('Boolean')
        ], BooleanFormatter);
        return BooleanFormatter;
    }());
    Serenity.BooleanFormatter = BooleanFormatter;
    var CheckboxFormatter = /** @class */ (function () {
        function CheckboxFormatter() {
        }
        CheckboxFormatter.prototype.format = function (ctx) {
            return '<span class="check-box no-float readonly ' + (!!ctx.value ? ' checked' : '') + '"></span>';
        };
        CheckboxFormatter = __decorate([
            Formatter('Checkbox')
        ], CheckboxFormatter);
        return CheckboxFormatter;
    }());
    Serenity.CheckboxFormatter = CheckboxFormatter;
    var DateFormatter = /** @class */ (function () {
        function DateFormatter() {
            this.displayFormat = Q.Culture.dateFormat;
        }
        DateFormatter_1 = DateFormatter;
        DateFormatter.format = function (value, format) {
            if (value == null) {
                return '';
            }
            var date;
            if (value instanceof Date) {
                date = value;
            }
            else if (typeof value === 'string') {
                date = Q.parseISODateTime(value);
                if (date == null) {
                    return Q.htmlEncode(value);
                }
            }
            else {
                return value.toString();
            }
            return Q.htmlEncode(Q.formatDate(date, format));
        };
        DateFormatter.prototype.format = function (ctx) {
            return DateFormatter_1.format(ctx.value, this.displayFormat);
        };
        var DateFormatter_1;
        __decorate([
            Option()
        ], DateFormatter.prototype, "displayFormat", void 0);
        DateFormatter = DateFormatter_1 = __decorate([
            Formatter('Date')
        ], DateFormatter);
        return DateFormatter;
    }());
    Serenity.DateFormatter = DateFormatter;
    var DateTimeFormatter = /** @class */ (function (_super) {
        __extends(DateTimeFormatter, _super);
        function DateTimeFormatter() {
            var _this = _super.call(this) || this;
            _this.displayFormat = Q.Culture.dateTimeFormat;
            return _this;
        }
        DateTimeFormatter = __decorate([
            Formatter('DateTime')
        ], DateTimeFormatter);
        return DateTimeFormatter;
    }(DateFormatter));
    Serenity.DateTimeFormatter = DateTimeFormatter;
    var EnumFormatter = /** @class */ (function () {
        function EnumFormatter() {
        }
        EnumFormatter_1 = EnumFormatter;
        EnumFormatter.prototype.format = function (ctx) {
            return EnumFormatter_1.format(Serenity.EnumTypeRegistry.get(this.enumKey), ctx.value);
        };
        EnumFormatter.format = function (enumType, value) {
            if (value == null) {
                return '';
            }
            var name = Q.Enum.toString(enumType, value);
            var enumKeyAttr = Q.getAttributes(enumType, Serenity.EnumKeyAttribute, false);
            var enumKey = ((enumKeyAttr.length > 0) ? enumKeyAttr[0].value : Q.getTypeFullName(enumType));
            return EnumFormatter_1.getText(enumKey, name);
        };
        EnumFormatter.getText = function (enumKey, name) {
            if (Q.isEmptyOrNull(name)) {
                return '';
            }
            return Q.htmlEncode(Q.coalesce(Q.tryGetText('Enums.' + enumKey + '.' + name), name));
        };
        EnumFormatter.getName = function (enumType, value) {
            if (value == null) {
                return '';
            }
            return Q.Enum.toString(enumType, value);
        };
        var EnumFormatter_1;
        __decorate([
            Option()
        ], EnumFormatter.prototype, "enumKey", void 0);
        EnumFormatter = EnumFormatter_1 = __decorate([
            Formatter('Enum')
        ], EnumFormatter);
        return EnumFormatter;
    }());
    Serenity.EnumFormatter = EnumFormatter;
    var FileDownloadFormatter = /** @class */ (function () {
        function FileDownloadFormatter() {
        }
        FileDownloadFormatter_1 = FileDownloadFormatter;
        FileDownloadFormatter.prototype.format = function (ctx) {
            var dbFile = Q.safeCast(ctx.value, String);
            if (Q.isEmptyOrNull(dbFile)) {
                return '';
            }
            var downloadUrl = FileDownloadFormatter_1.dbFileUrl(dbFile);
            var originalName = (!Q.isEmptyOrNull(this.originalNameProperty) ?
                Q.safeCast(ctx.item[this.originalNameProperty], String) : null);
            originalName = Q.coalesce(originalName, '');
            var text = Q.format(Q.coalesce(this.displayFormat, '{0}'), originalName, dbFile, downloadUrl);
            return "<a class='file-download-link' target='_blank' href='" +
                Q.attrEncode(downloadUrl) + "'>" + Q.htmlEncode(text) + '</a>';
        };
        FileDownloadFormatter.dbFileUrl = function (filename) {
            filename = Q.replaceAll(Q.coalesce(filename, ''), '\\', '/');
            return Q.resolveUrl('~/upload/') + filename;
        };
        FileDownloadFormatter.prototype.initializeColumn = function (column) {
            column.referencedFields = column.referencedFields || [];
            if (!Q.isEmptyOrNull(this.originalNameProperty)) {
                column.referencedFields.push(this.originalNameProperty);
                return;
            }
        };
        var FileDownloadFormatter_1;
        __decorate([
            Option()
        ], FileDownloadFormatter.prototype, "displayFormat", void 0);
        __decorate([
            Option()
        ], FileDownloadFormatter.prototype, "originalNameProperty", void 0);
        FileDownloadFormatter = FileDownloadFormatter_1 = __decorate([
            Formatter('FileDownload', [Serenity.ISlickFormatter, IInitializeColumn])
        ], FileDownloadFormatter);
        return FileDownloadFormatter;
    }());
    Serenity.FileDownloadFormatter = FileDownloadFormatter;
    var MinuteFormatter = /** @class */ (function () {
        function MinuteFormatter() {
        }
        MinuteFormatter_1 = MinuteFormatter;
        MinuteFormatter.prototype.format = function (ctx) {
            return MinuteFormatter_1.format(ctx.value);
        };
        MinuteFormatter.format = function (value) {
            var hour = Math.floor(value / 60);
            var minute = value - hour * 60;
            var hourStr, minuteStr;
            if (value == null || isNaN(value))
                return '';
            if (hour < 10)
                hourStr = '0' + hour;
            else
                hourStr = hour.toString();
            if (minute < 10)
                minuteStr = '0' + minute;
            else
                minuteStr = minute.toString();
            return Q.format('{0}:{1}', hourStr, minuteStr);
        };
        var MinuteFormatter_1;
        MinuteFormatter = MinuteFormatter_1 = __decorate([
            Formatter('Minute')
        ], MinuteFormatter);
        return MinuteFormatter;
    }());
    Serenity.MinuteFormatter = MinuteFormatter;
    var NumberFormatter = /** @class */ (function () {
        function NumberFormatter() {
        }
        NumberFormatter_1 = NumberFormatter;
        NumberFormatter.prototype.format = function (ctx) {
            return NumberFormatter_1.format(ctx.value, this.displayFormat);
        };
        NumberFormatter.format = function (value, format) {
            format = Q.coalesce(format, '0.##');
            if (value == null)
                return '';
            if (typeof (value) === 'number') {
                if (isNaN(value))
                    return '';
                return Q.htmlEncode(Q.formatNumber(value, format));
            }
            var dbl = Q.parseDecimal(value.toString());
            if (dbl == null)
                return '';
            return Q.htmlEncode(value.toString());
        };
        var NumberFormatter_1;
        __decorate([
            Option()
        ], NumberFormatter.prototype, "displayFormat", void 0);
        NumberFormatter = NumberFormatter_1 = __decorate([
            Formatter('Number')
        ], NumberFormatter);
        return NumberFormatter;
    }());
    Serenity.NumberFormatter = NumberFormatter;
    var UrlFormatter = /** @class */ (function () {
        function UrlFormatter() {
        }
        UrlFormatter.prototype.format = function (ctx) {
            var url = (!Q.isEmptyOrNull(this.urlProperty) ?
                Q.coalesce(ctx.item[this.urlProperty], '').toString() :
                Q.coalesce(ctx.value, '').toString());
            if (Q.isEmptyOrNull(url))
                return '';
            if (!Q.isEmptyOrNull(this.urlFormat))
                url = Q.format(this.urlFormat, url);
            if (url != null && Q.startsWith(url, '~/'))
                url = Q.resolveUrl(url);
            var display = (!Q.isEmptyOrNull(this.displayProperty) ?
                Q.coalesce(ctx.item[this.displayProperty], '').toString() :
                Q.coalesce(ctx.value, '').toString());
            if (!Q.isEmptyOrNull(this.displayFormat))
                display = Q.format(this.displayFormat, display);
            var s = "<a href='" + Q.attrEncode(url) + "'";
            if (!Q.isEmptyOrNull(this.target))
                s += " target='" + this.target + "'";
            s += '>' + Q.htmlEncode(display) + '</a>';
            return s;
        };
        UrlFormatter.prototype.initializeColumn = function (column) {
            column.referencedFields = column.referencedFields || [];
            if (!Q.isEmptyOrNull(this.displayProperty)) {
                column.referencedFields.push(this.displayProperty);
                return;
            }
            if (!Q.isEmptyOrNull(this.urlProperty)) {
                column.referencedFields.push(this.urlProperty);
                return;
            }
        };
        __decorate([
            Option()
        ], UrlFormatter.prototype, "displayProperty", void 0);
        __decorate([
            Option()
        ], UrlFormatter.prototype, "displayFormat", void 0);
        __decorate([
            Option()
        ], UrlFormatter.prototype, "urlProperty", void 0);
        __decorate([
            Option()
        ], UrlFormatter.prototype, "urlFormat", void 0);
        __decorate([
            Option()
        ], UrlFormatter.prototype, "target", void 0);
        UrlFormatter = __decorate([
            Formatter('Url', [Serenity.ISlickFormatter, IInitializeColumn])
        ], UrlFormatter);
        return UrlFormatter;
    }());
    Serenity.UrlFormatter = UrlFormatter;
    var FormatterTypeRegistry;
    (function (FormatterTypeRegistry) {
        var knownTypes;
        function setTypeKeysWithoutFormatterSuffix() {
            var suffix = 'formatter';
            for (var _i = 0, _a = Object.keys(knownTypes); _i < _a.length; _i++) {
                var k = _a[_i];
                if (!Q.endsWith(k, suffix))
                    continue;
                var p = k.substr(0, k.length - suffix.length);
                if (Q.isEmptyOrNull(p))
                    continue;
                if (knownTypes[p] != null)
                    continue;
                knownTypes[p] = knownTypes[k];
            }
        }
        function initialize() {
            if (knownTypes) {
                return;
            }
            knownTypes = {};
            var types = Q.getTypes();
            for (var _i = 0, types_1 = types; _i < types_1.length; _i++) {
                var type = types_1[_i];
                if (!Q.isAssignableFrom(Serenity.ISlickFormatter, type))
                    continue;
                var fullName = Q.getTypeFullName(type).toLowerCase();
                knownTypes[fullName] = type;
                for (var _a = 0, _b = Q.Config.rootNamespaces; _a < _b.length; _a++) {
                    var k = _b[_a];
                    if (Q.startsWith(fullName, k.toLowerCase() + '.')) {
                        var kx = fullName.substr(k.length + 1).toLowerCase();
                        if (knownTypes[kx] == null) {
                            knownTypes[kx] = type;
                        }
                    }
                }
            }
            setTypeKeysWithoutFormatterSuffix();
        }
        function get(key) {
            if (Q.isEmptyOrNull(key))
                throw new Q.ArgumentNullException('key');
            initialize();
            var formatterType = knownTypes[key.toLowerCase()];
            if (formatterType == null) {
                throw new Q.Exception(Q.format("Can't find {0} formatter type!", key));
            }
            return formatterType;
        }
        FormatterTypeRegistry.get = get;
        function reset() {
            knownTypes = null;
        }
        FormatterTypeRegistry.reset = reset;
    })(FormatterTypeRegistry = Serenity.FormatterTypeRegistry || (Serenity.FormatterTypeRegistry = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IDataGrid = /** @class */ (function () {
        function IDataGrid() {
        }
        IDataGrid = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IDataGrid')
        ], IDataGrid);
        return IDataGrid;
    }());
    Serenity.IDataGrid = IDataGrid;
    var DataGrid = /** @class */ (function (_super) {
        __extends(DataGrid, _super);
        function DataGrid(container, options) {
            var _this = _super.call(this, container, options) || this;
            _this.restoringSettings = 0;
            var self = _this;
            _this.element.addClass('s-DataGrid').html('');
            _this.element.addClass('s-' + Q.getTypeName(Q.getInstanceType(_this)));
            var layout = function () {
                self.layout();
                if (self.layoutTimer != null)
                    Q.LayoutTimer.store(self.layoutTimer);
            };
            _this.element.addClass('require-layout').on('layout.' + _this.uniqueName, layout);
            if (_this.useLayoutTimer())
                _this.layoutTimer = Q.LayoutTimer.onSizeChange(function () { return _this.element && _this.element[0]; }, Q.debounce(layout, 50));
            _this.setTitle(_this.getInitialTitle());
            var buttons = _this.getButtons();
            if (buttons != null) {
                _this.createToolbar(buttons);
            }
            _this.slickContainer = _this.createSlickContainer();
            _this.view = _this.createView();
            _this.slickGrid = _this.createSlickGrid();
            if (_this.enableFiltering()) {
                _this.createFilterBar();
            }
            if (_this.usePager()) {
                _this.createPager();
            }
            _this.bindToSlickEvents();
            _this.bindToViewEvents();
            if (buttons != null) {
                _this.createToolbarExtensions();
            }
            _this.createQuickFilters();
            _this.updateDisabledState();
            _this.updateInterface();
            _this.initialSettings = _this.getCurrentSettings(null);
            _this.restoreSettings(null, null);
            window.setTimeout(function () { return _this.initialPopulate(); }, 0);
            return _this;
        }
        DataGrid_1 = DataGrid;
        DataGrid.prototype.useLayoutTimer = function () {
            return true;
        };
        DataGrid.prototype.attrs = function (attrType) {
            return Q.getAttributes(Q.getInstanceType(this), attrType, true);
        };
        DataGrid.prototype.layout = function () {
            if (!this.element || !this.element.is(':visible') || this.slickContainer == null)
                return;
            var responsiveHeight = this.element.hasClass('responsive-height');
            var madeAutoHeight = this.slickGrid != null && this.slickGrid.getOptions().autoHeight;
            var shouldAutoHeight = responsiveHeight && window.innerWidth < 768;
            if (shouldAutoHeight) {
                if (this.element[0] && this.element[0].style.height != "auto")
                    this.element[0].style.height = "auto";
                if (!madeAutoHeight) {
                    this.slickContainer.css('height', 'auto')
                        .children('.slick-pane').each(function (i, e) {
                        if (e.style.height != null && e.style.height != "auto")
                            e.style.height = "auto";
                    });
                    this.slickGrid.setOptions({ autoHeight: true });
                }
            }
            else {
                if (madeAutoHeight) {
                    this.slickContainer.children('.slick-viewport').css('height', 'auto');
                    this.slickGrid.setOptions({ autoHeight: false });
                }
                Q.layoutFillHeight(this.slickContainer);
            }
            this.slickGrid.resizeCanvas();
        };
        DataGrid.prototype.getInitialTitle = function () {
            return null;
        };
        DataGrid.prototype.createToolbarExtensions = function () {
        };
        DataGrid.prototype.ensureQuickFilterBar = function () {
            if (this.quickFiltersDiv == null)
                this.createQuickFilters([]);
            return this.quickFiltersBar;
        };
        DataGrid.prototype.createQuickFilters = function (filters) {
            var _this = this;
            if (this.quickFiltersDiv == null && (filters != null ||
                ((filters = this.getQuickFilters()) && filters != null && filters.length))) {
                this.quickFiltersDiv = $('<div/>').addClass('quick-filters-bar');
                if (this.toolbar) {
                    $('<div/>').addClass('clear').appendTo(this.toolbar.element);
                    this.quickFiltersDiv.appendTo(this.toolbar.element);
                }
                else {
                    this.quickFiltersDiv.appendTo($('<div/>').addClass('s-Toolbar').insertBefore(this.slickContainer));
                }
                this.quickFiltersBar = new Serenity.QuickFilterBar(this.quickFiltersDiv, {
                    filters: filters,
                    getTitle: function (filter) { return _this.determineText(function (pre) { return pre + filter.field; }); },
                    idPrefix: this.uniqueName + '_QuickFilter_'
                });
                this.quickFiltersBar.onChange = function (e) { return _this.quickFilterChange(e); };
            }
        };
        DataGrid.prototype.getQuickFilters = function () {
            return this.allColumns.filter(function (x) {
                return x.sourceItem &&
                    x.sourceItem.quickFilter === true &&
                    (x.sourceItem.readPermission == null ||
                        Q.Authorization.hasPermission(x.sourceItem.readPermission));
            }).map(function (x) { return DataGrid_1.propertyItemToQuickFilter(x.sourceItem); })
                .filter(function (x) { return x != null; });
        };
        DataGrid.propertyItemToQuickFilter = function (item) {
            var quick = {};
            var name = item.name;
            var title = Q.tryGetText(item.title);
            if (title == null) {
                title = item.title;
                if (title == null) {
                    title = name;
                }
            }
            var filteringType = Serenity.FilteringTypeRegistry.get(Q.coalesce(item.filteringType, 'String'));
            if (filteringType === Serenity.DateFiltering) {
                quick = Serenity.QuickFilterBar.dateRange(name, title);
            }
            else if (filteringType === Serenity.DateTimeFiltering) {
                quick = Serenity.QuickFilterBar.dateTimeRange(name, title);
            }
            else if (filteringType === Serenity.BooleanFiltering) {
                var q = item.quickFilterParams || {};
                var f = item.filteringParams || {};
                var trueText = q['trueText'];
                if (trueText == null) {
                    trueText = f['trueText'];
                }
                var falseText = q['falseText'];
                if (falseText == null) {
                    falseText = f['falseText'];
                }
                quick = Serenity.QuickFilterBar.boolean(name, title, trueText, falseText);
            }
            else {
                var filtering = new filteringType();
                if (filtering && Q.isInstanceOfType(filtering, Serenity.IQuickFiltering)) {
                    Serenity.ReflectionOptionsSetter.set(filtering, item.filteringParams);
                    filtering.set_field(item);
                    filtering.set_operator({ key: Serenity.FilterOperators.EQ });
                    filtering.initQuickFilter(quick);
                    quick.options = Q.extend(Q.deepClone(quick.options), item.quickFilterParams);
                }
                else {
                    return null;
                }
            }
            if (!!item.quickFilterSeparator) {
                quick.separator = true;
            }
            quick.cssClass = item.quickFilterCssClass;
            return quick;
        };
        DataGrid.prototype.findQuickFilter = function (type, field) {
            if (this.quickFiltersBar != null)
                return this.quickFiltersBar.find(type, field);
            return $('#' + this.uniqueName + '_QuickFilter_' + field).getWidget(type);
        };
        DataGrid.prototype.tryFindQuickFilter = function (type, field) {
            if (this.quickFiltersBar != null)
                return this.quickFiltersBar.tryFind(type, field);
            var el = $('#' + this.uniqueName + '_QuickFilter_' + field);
            if (!el.length)
                return null;
            return el.tryGetWidget(type);
        };
        DataGrid.prototype.createIncludeDeletedButton = function () {
            if (!Q.isEmptyOrNull(this.getIsActiveProperty()) ||
                !Q.isEmptyOrNull(this.getIsDeletedProperty())) {
                Serenity.GridUtils.addIncludeDeletedToggle(this.toolbar.element, this.view, null, false);
            }
        };
        DataGrid.prototype.getQuickSearchFields = function () {
            return null;
        };
        DataGrid.prototype.createQuickSearchInput = function () {
            var _this = this;
            Serenity.GridUtils.addQuickSearchInput(this.toolbar.element, this.view, this.getQuickSearchFields(), function () { return _this.persistSettings(null); });
        };
        DataGrid.prototype.destroy = function () {
            if (this.layoutTimer) {
                this.layoutTimer = Q.LayoutTimer.off(this.layoutTimer);
            }
            if (this.quickFiltersBar) {
                this.quickFiltersBar.destroy();
                this.quickFiltersBar = null;
            }
            if (this.toolbar) {
                this.toolbar.destroy();
                this.toolbar = null;
            }
            if (this.slickGrid) {
                this.slickGrid.onClick.clear();
                this.slickGrid.onSort.clear();
                this.slickGrid.onColumnsResized.clear();
                this.slickGrid.onColumnsReordered.clear();
                this.slickGrid.destroy();
                this.slickGrid = null;
            }
            if (this.view) {
                this.view.onDataChanged.clear();
                this.view.onSubmit = null;
                this.view.setFilter(null);
                this.view = null;
            }
            this.titleDiv = null;
            _super.prototype.destroy.call(this);
        };
        DataGrid.prototype.getItemCssClass = function (item, index) {
            var activeFieldName = this.getIsActiveProperty();
            var deletedFieldName = this.getIsDeletedProperty();
            if (Q.isEmptyOrNull(activeFieldName) && Q.isEmptyOrNull(deletedFieldName)) {
                return null;
            }
            if (activeFieldName) {
                var value = item[activeFieldName];
                if (value == null) {
                    return null;
                }
                if (typeof (value) === 'number') {
                    if (value < 0) {
                        return 'deleted';
                    }
                    else if (value === 0) {
                        return 'inactive';
                    }
                }
                else if (typeof (value) === 'boolean') {
                    if (value === false) {
                        return 'deleted';
                    }
                }
            }
            else {
                return item[deletedFieldName] ? 'deleted' : null;
            }
            return null;
        };
        DataGrid.prototype.getItemMetadata = function (item, index) {
            var itemClass = this.getItemCssClass(item, index);
            if (Q.isEmptyOrNull(itemClass)) {
                return new Object();
            }
            return { cssClasses: itemClass };
        };
        DataGrid.prototype.postProcessColumns = function (columns) {
            Serenity.SlickHelper.setDefaults(columns, this.getLocalTextDbPrefix());
            return columns;
        };
        DataGrid.prototype.initialPopulate = function () {
            var self = this;
            if (this.populateWhenVisible()) {
                Serenity.LazyLoadHelper.executeEverytimeWhenShown(this.element, function () {
                    self.refreshIfNeeded();
                }, false);
                if (this.element.is(':visible') && this.view) {
                    this.view.populate();
                }
            }
            else if (this.view) {
                this.view.populate();
            }
        };
        DataGrid.prototype.canFilterColumn = function (column) {
            return (column.sourceItem != null &&
                column.sourceItem.notFilterable !== true &&
                (column.sourceItem.readPermission == null ||
                    Q.Authorization.hasPermission(column.sourceItem.readPermission)));
        };
        DataGrid.prototype.initializeFilterBar = function () {
            var _this = this;
            this.filterBar.set_store(new Serenity.FilterStore(this.allColumns
                .filter(function (c) { return _this.canFilterColumn(c); })
                .map(function (x) { return x.sourceItem; })));
            this.filterBar.get_store().add_changed(function (s, e) {
                if (_this.restoringSettings <= 0) {
                    _this.persistSettings(null);
                    _this.view && (_this.view.seekToPage = 1);
                    _this.refresh();
                }
            });
        };
        DataGrid.prototype.createSlickGrid = function () {
            var visibleColumns;
            this.allColumns = this.getColumns();
            visibleColumns = this.postProcessColumns(this.allColumns).filter(function (x) {
                return x.visible !== false;
            });
            var slickOptions = this.getSlickOptions();
            var grid = new Slick.Grid(this.slickContainer, this.view, visibleColumns, slickOptions);
            grid.registerPlugin(new Slick.AutoTooltips({
                enableForHeaderCells: true
            }));
            this.slickGrid = grid;
            this.rows = this.slickGrid;
            this.setInitialSortOrder();
            return grid;
        };
        DataGrid.prototype.setInitialSortOrder = function () {
            var sortBy = this.getDefaultSortBy();
            if (this.view) {
                this.view.sortBy = Array.prototype.slice.call(sortBy);
            }
            var mapped = sortBy.map(function (s) {
                var x = {};
                if (s && Q.endsWith(s.toLowerCase(), ' desc')) {
                    x.columnId = Q.trimEnd(s.substr(0, s.length - 5));
                    x.sortAsc = false;
                }
                else {
                    x.columnId = s;
                    x.sortAsc = true;
                }
                return x;
            });
            this.slickGrid.setSortColumns(mapped);
        };
        DataGrid.prototype.itemAt = function (row) {
            return this.slickGrid.getDataItem(row);
        };
        DataGrid.prototype.rowCount = function () {
            return this.slickGrid.getDataLength();
        };
        DataGrid.prototype.getItems = function () {
            return this.view.getItems();
        };
        DataGrid.prototype.setItems = function (value) {
            this.view.setItems(value, true);
        };
        DataGrid.prototype.bindToSlickEvents = function () {
            var _this = this;
            var self = this;
            this.slickGridOnSort = function (e, p) {
                self.view.populateLock();
                try {
                    var sortBy = [];
                    var col;
                    if (!!p.multiColumnSort) {
                        for (var i = 0; !!(i < p.sortCols.length); i++) {
                            var x = p.sortCols[i];
                            col = x.sortCol;
                            if (col == null) {
                                col = {};
                            }
                            sortBy.push(col.field + (!!x.sortAsc ? '' : ' DESC'));
                        }
                    }
                    else {
                        var col = p.sortCol;
                        if (col == null) {
                            col = {};
                        }
                        sortBy.push(col.field + (!!p.sortAsc ? '' : ' DESC'));
                    }
                    self.view.seekToPage = 1;
                    self.view.sortBy = sortBy;
                }
                finally {
                    self.view.populateUnlock();
                }
                if (self.view.getLocalSort && self.view.getLocalSort()) {
                    self.view.sort();
                }
                else {
                    self.view.populate();
                }
                _this.persistSettings(null);
            };
            this.slickGrid.onSort.subscribe(this.slickGridOnSort);
            this.slickGridOnClick = function (e1, p1) {
                self.onClick(e1, p1.row, p1.cell);
            };
            this.slickGrid.onClick.subscribe(this.slickGridOnClick);
            this.slickGrid.onColumnsReordered.subscribe(function (e2, p2) {
                return _this.persistSettings(null);
            });
            this.slickGrid.onColumnsResized.subscribe(function (e3, p3) {
                return _this.persistSettings(null);
            });
        };
        DataGrid.prototype.getAddButtonCaption = function () {
            return Q.coalesce(Q.tryGetText('Controls.DataGrid.NewButton'), 'New');
        };
        DataGrid.prototype.getButtons = function () {
            return [];
        };
        DataGrid.prototype.editItem = function (entityOrId) {
            throw new Error("Not Implemented!");
        };
        DataGrid.prototype.editItemOfType = function (itemType, entityOrId) {
            if (itemType === this.getItemType()) {
                this.editItem(entityOrId);
                return;
            }
            throw new Error("Not Implemented!");
        };
        DataGrid.prototype.onClick = function (e, row, cell) {
            if (e.isDefaultPrevented()) {
                return;
            }
            var target = $(e.target);
            if (!target.hasClass('s-EditLink')) {
                target = target.closest('a');
            }
            if (target.hasClass('s-EditLink')) {
                e.preventDefault();
                this.editItemOfType(Serenity.SlickFormatting.getItemType(target), Serenity.SlickFormatting.getItemId(target));
            }
        };
        DataGrid.prototype.viewDataChanged = function (e, rows) {
            this.markupReady();
            this.layout();
        };
        DataGrid.prototype.bindToViewEvents = function () {
            var self = this;
            this.view.onDataChanged.subscribe(function (e, d) {
                return self.viewDataChanged(e, d);
            });
            this.view.onSubmit = function (view) {
                return self.onViewSubmit();
            };
            this.view.setFilter(function (item, view1) {
                return self.onViewFilter(item);
            });
            this.view.onProcessData = function (response, view2) {
                return self.onViewProcessData(response);
            };
        };
        DataGrid.prototype.onViewProcessData = function (response) {
            return response;
        };
        DataGrid.prototype.onViewFilter = function (item) {
            return true;
        };
        DataGrid.prototype.getIncludeColumns = function (include) {
            var columns = this.slickGrid.getColumns();
            for (var _i = 0, columns_2 = columns; _i < columns_2.length; _i++) {
                var column = columns_2[_i];
                if (column.field) {
                    include[column.field] = true;
                }
                if (column.referencedFields) {
                    for (var _a = 0, _b = column.referencedFields; _a < _b.length; _a++) {
                        var x = _b[_a];
                        include[x] = true;
                    }
                }
            }
        };
        DataGrid.prototype.setCriteriaParameter = function () {
            delete this.view.params['Criteria'];
            if (this.filterBar) {
                var criteria = this.filterBar.get_store().get_activeCriteria();
                if (!Serenity.Criteria.isEmpty(criteria)) {
                    this.view.params.Criteria = criteria;
                }
            }
        };
        DataGrid.prototype.setEquality = function (field, value) {
            Q.setEquality(this.view.params, field, value);
        };
        DataGrid.prototype.setIncludeColumnsParameter = function () {
            var include = {};
            this.getIncludeColumns(include);
            var array = [];
            for (var _i = 0, _a = Object.keys(include); _i < _a.length; _i++) {
                var key = _a[_i];
                array.push(key);
            }
            this.view.params.IncludeColumns = array;
        };
        DataGrid.prototype.onViewSubmit = function () {
            if (this.isDisabled || !this.getGridCanLoad()) {
                return false;
            }
            this.setCriteriaParameter();
            this.setIncludeColumnsParameter();
            this.invokeSubmitHandlers();
            return true;
        };
        DataGrid.prototype.markupReady = function () {
        };
        DataGrid.prototype.createSlickContainer = function () {
            return $('<div class="grid-container"></div>').appendTo(this.element);
        };
        DataGrid.prototype.createView = function () {
            var opt = this.getViewOptions();
            return new Slick.RemoteView(opt);
        };
        DataGrid.prototype.getDefaultSortBy = function () {
            if (this.slickGrid) {
                var columns = this.slickGrid.getColumns().filter(function (x) {
                    return x.sortOrder && x.sortOrder !== 0;
                });
                if (columns.length > 0) {
                    columns.sort(function (x1, y) {
                        return x1.sortOrder < y.sortOrder ? -1 : (x1.sortOrder > y.sortOrder ? 1 : 0);
                    });
                    var list = [];
                    for (var i = 0; i < columns.length; i++) {
                        var col = columns[i];
                        list.push(col.field + ((col.sortOrder < 0) ? ' DESC' : ''));
                    }
                    return list;
                }
            }
            return [];
        };
        DataGrid.prototype.usePager = function () {
            return false;
        };
        DataGrid.prototype.enableFiltering = function () {
            var attr = this.attrs(Serenity.FilterableAttribute);
            return attr.length > 0 && attr[0].value;
        };
        DataGrid.prototype.populateWhenVisible = function () {
            return false;
        };
        DataGrid.prototype.createFilterBar = function () {
            var filterBarDiv = $('<div/>').appendTo(this.element);
            this.filterBar = new Serenity.FilterDisplayBar(filterBarDiv);
            this.initializeFilterBar();
        };
        DataGrid.prototype.getPagerOptions = function () {
            return {
                view: this.view,
                rowsPerPage: 20,
                rowsPerPageOptions: [20, 100, 500, 2500]
            };
        };
        DataGrid.prototype.createPager = function () {
            var pagerDiv = $('<div></div>').appendTo(this.element);
            new Serenity.SlickPager(pagerDiv, this.getPagerOptions());
        };
        DataGrid.prototype.getViewOptions = function () {
            var _this = this;
            var opt = {};
            opt.idField = this.getIdProperty();
            opt.sortBy = this.getDefaultSortBy();
            if (!this.usePager()) {
                opt.rowsPerPage = 0;
            }
            else if (this.element.hasClass('responsive-height')) {
                opt.rowsPerPage = (($(window.window).width() < 768) ? 20 : 100);
            }
            else {
                opt.rowsPerPage = 100;
            }
            opt.getItemMetadata = function (item, index) {
                return _this.getItemMetadata(item, index);
            };
            return opt;
        };
        DataGrid.prototype.createToolbar = function (buttons) {
            var toolbarDiv = $('<div class="grid-toolbar"></div>').appendTo(this.element);
            this.toolbar = new Serenity.Toolbar(toolbarDiv, { buttons: buttons, hotkeyContext: this.element[0] });
        };
        DataGrid.prototype.getTitle = function () {
            if (!this.titleDiv) {
                return null;
            }
            return this.titleDiv.children('.title-text').text();
        };
        DataGrid.prototype.setTitle = function (value) {
            if (value !== this.getTitle()) {
                if (value == null) {
                    if (this.titleDiv) {
                        this.titleDiv.remove();
                        this.titleDiv = null;
                    }
                }
                else {
                    if (!this.titleDiv) {
                        this.titleDiv = $('<div class="grid-title"><div class="title-text"></div></div>')
                            .prependTo(this.element);
                    }
                    this.titleDiv.children('.title-text').text(value);
                }
                this.layout();
            }
        };
        DataGrid.prototype.getItemType = function () {
            return 'Item';
        };
        DataGrid.prototype.itemLink = function (itemType, idField, text, cssClass, encode) {
            if (encode === void 0) { encode = true; }
            if (itemType == null) {
                itemType = this.getItemType();
            }
            if (idField == null) {
                idField = this.getIdProperty();
            }
            return Serenity.SlickFormatting.itemLink(itemType, idField, text, cssClass, encode);
        };
        DataGrid.prototype.getColumnsKey = function () {
            var attr = this.attrs(Serenity.ColumnsKeyAttribute);
            if (attr && attr.length > 0) {
                return attr[0].value;
            }
            return null;
        };
        DataGrid.prototype.getPropertyItems = function () {
            var attr = this.attrs(Serenity.ColumnsKeyAttribute);
            var columnsKey = this.getColumnsKey();
            if (!Q.isEmptyOrNull(columnsKey)) {
                return Q.getColumns(columnsKey);
            }
            return [];
        };
        DataGrid.prototype.getColumns = function () {
            var propertyItems = this.getPropertyItems();
            return this.propertyItemsToSlickColumns(propertyItems);
        };
        DataGrid.prototype.propertyItemsToSlickColumns = function (propertyItems) {
            var columns = Serenity.PropertyItemSlickConverter.toSlickColumns(propertyItems);
            for (var i = 0; i < propertyItems.length; i++) {
                var item = propertyItems[i];
                var column = columns[i];
                if (item.editLink === true) {
                    var oldFormat = { $: column.format };
                    var css = { $: (item.editLinkCssClass) != null ? item.editLinkCssClass : null };
                    column.format = this.itemLink(item.editLinkItemType != null ? item.editLinkItemType : null, item.editLinkIdField != null ? item.editLinkIdField : null, function (ctx) {
                        if (this.oldFormat.$ != null) {
                            return this.oldFormat.$(ctx);
                        }
                        return Q.htmlEncode(ctx.value);
                    }.bind({ oldFormat: oldFormat }), function (ctx1) {
                        return Q.coalesce(this.css.$, '');
                    }.bind({ css: css }), false);
                    if (!Q.isEmptyOrNull(item.editLinkIdField)) {
                        column.referencedFields = column.referencedFields || [];
                        column.referencedFields.push(item.editLinkIdField);
                    }
                }
            }
            return columns;
        };
        DataGrid.prototype.getSlickOptions = function () {
            var opt = {};
            opt.multiSelect = false;
            opt.multiColumnSort = true;
            opt.enableCellNavigation = false;
            opt.headerRowHeight = Serenity.DataGrid.defaultHeaderHeight;
            opt.rowHeight = Serenity.DataGrid.defaultRowHeight;
            return opt;
        };
        DataGrid.prototype.populateLock = function () {
            this.view.populateLock();
        };
        DataGrid.prototype.populateUnlock = function () {
            this.view.populateUnlock();
        };
        DataGrid.prototype.getGridCanLoad = function () {
            return true;
        };
        DataGrid.prototype.refresh = function () {
            if (!this.populateWhenVisible()) {
                this.internalRefresh();
                return;
            }
            if (this.slickContainer.is(':visible')) {
                this.slickContainer.data('needsRefresh', false);
                this.internalRefresh();
                return;
            }
            this.slickContainer.data('needsRefresh', true);
        };
        DataGrid.prototype.refreshIfNeeded = function () {
            if (!!this.slickContainer.data('needsRefresh')) {
                this.slickContainer.data('needsRefresh', false);
                this.internalRefresh();
            }
        };
        DataGrid.prototype.internalRefresh = function () {
            this.view.populate();
        };
        DataGrid.prototype.setIsDisabled = function (value) {
            if (this.isDisabled !== value) {
                this.isDisabled = value;
                if (this.isDisabled) {
                    this.view.setItems([], true);
                }
                this.updateDisabledState();
            }
        };
        Object.defineProperty(DataGrid.prototype, "readOnly", {
            get: function () {
                return this.get_readOnly();
            },
            set: function (value) {
                this.set_readOnly(value);
            },
            enumerable: true,
            configurable: true
        });
        DataGrid.prototype.get_readOnly = function () {
            return !!this._readonly;
        };
        DataGrid.prototype.set_readOnly = function (value) {
            if (!!this._readonly != !!value) {
                this._readonly = !!value;
                this.updateInterface();
            }
        };
        DataGrid.prototype.updateInterface = function () {
            this.toolbar && this.toolbar.updateInterface();
        };
        DataGrid.prototype.getLocalTextDbPrefix = function () {
            if (this.localTextDbPrefix == null) {
                this.localTextDbPrefix = Q.coalesce(this.getLocalTextPrefix(), '');
                if (this.localTextDbPrefix.length > 0 && !Q.endsWith(this.localTextDbPrefix, '.')) {
                    this.localTextDbPrefix = 'Db.' + this.localTextDbPrefix + '.';
                }
            }
            return this.localTextDbPrefix;
        };
        DataGrid.prototype.getLocalTextPrefix = function () {
            var attr = this.attrs(Serenity.LocalTextPrefixAttribute);
            if (attr.length >= 1)
                return attr[0].value;
            return '';
        };
        DataGrid.prototype.getIdProperty = function () {
            if (this.idProperty == null) {
                var attr = this.attrs(Serenity.IdPropertyAttribute);
                if (attr.length === 1) {
                    this.idProperty = attr[0].value;
                }
                else {
                    this.idProperty = 'ID';
                }
            }
            return this.idProperty;
        };
        DataGrid.prototype.getIsDeletedProperty = function () {
            return null;
        };
        DataGrid.prototype.getIsActiveProperty = function () {
            if (this.isActiveProperty == null) {
                var attr = this.attrs(Serenity.IsActivePropertyAttribute);
                if (attr.length === 1) {
                    this.isActiveProperty = attr[0].value;
                }
                else {
                    this.isActiveProperty = '';
                }
            }
            return this.isActiveProperty;
        };
        DataGrid.prototype.updateDisabledState = function () {
            this.slickContainer.toggleClass('ui-state-disabled', !!this.isDisabled);
        };
        DataGrid.prototype.resizeCanvas = function () {
            this.slickGrid.resizeCanvas();
        };
        DataGrid.prototype.subDialogDataChange = function () {
            this.refresh();
        };
        DataGrid.prototype.addFilterSeparator = function () {
            this.ensureQuickFilterBar().addSeparator();
        };
        DataGrid.prototype.determineText = function (getKey) {
            var localTextPrefix = this.getLocalTextDbPrefix();
            if (!Q.isEmptyOrNull(localTextPrefix)) {
                var local = Q.tryGetText(getKey(localTextPrefix));
                if (local != null) {
                    return local;
                }
            }
            return null;
        };
        DataGrid.prototype.addQuickFilter = function (opt) {
            return this.ensureQuickFilterBar().add(opt);
        };
        DataGrid.prototype.addDateRangeFilter = function (field, title) {
            return this.ensureQuickFilterBar().addDateRange(field, title);
        };
        DataGrid.prototype.dateRangeQuickFilter = function (field, title) {
            return Serenity.QuickFilterBar.dateRange(field, title);
        };
        DataGrid.prototype.addDateTimeRangeFilter = function (field, title) {
            return this.ensureQuickFilterBar().addDateTimeRange(field, title);
        };
        DataGrid.prototype.dateTimeRangeQuickFilter = function (field, title) {
            return Serenity.QuickFilterBar.dateTimeRange(field, title);
        };
        DataGrid.prototype.addBooleanFilter = function (field, title, yes, no) {
            return this.ensureQuickFilterBar().addBoolean(field, title, yes, no);
        };
        DataGrid.prototype.booleanQuickFilter = function (field, title, yes, no) {
            return Serenity.QuickFilterBar.boolean(field, title, yes, no);
        };
        DataGrid.prototype.invokeSubmitHandlers = function () {
            if (this.quickFiltersBar != null) {
                this.quickFiltersBar.onSubmit(this.view.params);
            }
        };
        DataGrid.prototype.quickFilterChange = function (e) {
            this.persistSettings(null);
            this.view && (this.view.seekToPage = 1);
            this.refresh();
        };
        DataGrid.prototype.getPersistanceStorage = function () {
            return Serenity.DataGrid.defaultPersistanceStorage;
        };
        DataGrid.prototype.getPersistanceKey = function () {
            var key = 'GridSettings:';
            var path = window.location.pathname;
            if (!Q.isEmptyOrNull(path)) {
                key += path.substr(1).split(String.fromCharCode(47)).slice(0, 2).join('/') + ':';
            }
            key += Q.getTypeFullName(Q.getInstanceType(this));
            return key;
        };
        DataGrid.prototype.gridPersistanceFlags = function () {
            return {};
        };
        DataGrid.prototype.canShowColumn = function (column) {
            if (column == null) {
                return false;
            }
            var item = column.sourceItem;
            if (item == null) {
                return true;
            }
            if (item.filterOnly === true) {
                return false;
            }
            if (item.readPermission == null) {
                return true;
            }
            return Q.Authorization.hasPermission(item.readPermission);
        };
        DataGrid.prototype.getPersistedSettings = function () {
            var storage = this.getPersistanceStorage();
            if (storage == null)
                return null;
            var json = Q.trimToNull(storage.getItem(this.getPersistanceKey()));
            if (json != null && Q.startsWith(json, '{') && Q.endsWith(json, '}'))
                return JSON.parse(json);
            return null;
        };
        DataGrid.prototype.restoreSettings = function (settings, flags) {
            var _this = this;
            if (!this.slickGrid)
                return;
            if (settings == null) {
                settings = this.getPersistedSettings();
                if (settings == null)
                    return;
            }
            var columns = this.slickGrid.getColumns();
            var colById = null;
            var updateColById = function (cl) {
                colById = {};
                for (var $t1 = 0; $t1 < cl.length; $t1++) {
                    var c = cl[$t1];
                    colById[c.id] = c;
                }
            };
            this.view.beginUpdate();
            this.restoringSettings++;
            try {
                flags = flags || this.gridPersistanceFlags();
                if (settings.columns != null) {
                    if (flags.columnVisibility !== false) {
                        var visible = {};
                        updateColById(this.allColumns);
                        var newColumns = [];
                        for (var $t2 = 0; $t2 < settings.columns.length; $t2++) {
                            var x = settings.columns[$t2];
                            if (x.id != null && x.visible === true) {
                                var column = colById[x.id];
                                if (this.canShowColumn(column)) {
                                    column.visible = true;
                                    newColumns.push(column);
                                    delete colById[x.id];
                                }
                            }
                        }
                        for (var $t3 = 0; $t3 < this.allColumns.length; $t3++) {
                            var c1 = this.allColumns[$t3];
                            if (colById[c1.id] != null) {
                                c1.visible = false;
                                newColumns.push(c1);
                            }
                        }
                        this.allColumns = newColumns;
                        columns = this.allColumns.filter(function (x1) {
                            return x1.visible === true;
                        });
                    }
                    if (flags.columnWidths !== false) {
                        updateColById(columns);
                        for (var $t4 = 0; $t4 < settings.columns.length; $t4++) {
                            var x2 = settings.columns[$t4];
                            if (x2.id != null && x2.width != null && x2.width !== 0) {
                                var column1 = colById[x2.id];
                                if (column1 != null) {
                                    column1.width = x2.width;
                                }
                            }
                        }
                    }
                    if (flags.sortColumns !== false) {
                        updateColById(columns);
                        var list = [];
                        var sortColumns = settings.columns.filter(function (x3) {
                            return x3.id != null && Q.coalesce(x3.sort, 0) !== 0;
                        });
                        sortColumns.sort(function (a, b) {
                            // sort holds two informations:
                            // absoulte value: order of sorting
                            // sign: positive = ascending, negativ = descending
                            // so we have to compare absolute values here
                            return Math.abs(a.sort) - Math.abs(b.sort);
                        });
                        for (var $t5 = 0; $t5 < sortColumns.length; $t5++) {
                            var x4 = sortColumns[$t5];
                            var column2 = colById[x4.id];
                            if (column2 != null) {
                                list.push({
                                    columnId: x4.id,
                                    sortAsc: x4.sort > 0
                                });
                            }
                        }
                        this.view.sortBy = list.map(function (x5) {
                            return x5.columnId + ((x5.sortAsc === false) ? ' DESC' : '');
                        });
                        this.slickGrid.setSortColumns(list);
                    }
                    this.slickGrid.setColumns(columns);
                    this.slickGrid.invalidate();
                }
                if (settings.filterItems != null &&
                    flags.filterItems !== false &&
                    this.filterBar != null &&
                    this.filterBar.get_store() != null) {
                    var items = this.filterBar.get_store().get_items();
                    items.length = 0;
                    items.push.apply(items, settings.filterItems);
                    this.filterBar.get_store().raiseChanged();
                }
                if (settings.includeDeleted != null &&
                    flags.includeDeleted !== false) {
                    var includeDeletedToggle = this.element.find('.s-IncludeDeletedToggle');
                    if (!!settings.includeDeleted !== includeDeletedToggle.hasClass('pressed')) {
                        includeDeletedToggle.children('a').click();
                    }
                }
                if (settings.quickFilters != null &&
                    flags.quickFilters !== false &&
                    this.quickFiltersDiv != null &&
                    this.quickFiltersDiv.length > 0) {
                    this.quickFiltersDiv.find('.quick-filter-item').each(function (i, e) {
                        var field = $(e).data('qffield');
                        if (Q.isEmptyOrNull(field)) {
                            return;
                        }
                        var widget = $('#' + _this.uniqueName + '_QuickFilter_' + field).tryGetWidget(Serenity.Widget);
                        if (widget == null) {
                            return;
                        }
                        var state = settings.quickFilters[field];
                        var loadState = $(e).data('qfloadstate');
                        if (loadState != null) {
                            loadState(widget, state);
                        }
                        else {
                            Serenity.EditorUtils.setValue(widget, state);
                        }
                    });
                }
                if (flags.quickSearch === true && (settings.quickSearchField !== undefined || settings.quickSearchText !== undefined)) {
                    var qsInput = this.toolbar.element.find('.s-QuickSearchInput').first();
                    if (qsInput.length > 0) {
                        var qsWidget = qsInput.tryGetWidget(Serenity.QuickSearchInput);
                        qsWidget && qsWidget.restoreState(settings.quickSearchText, settings.quickSearchField);
                    }
                }
            }
            finally {
                this.restoringSettings--;
                this.view.endUpdate();
            }
        };
        DataGrid.prototype.persistSettings = function (flags) {
            var storage = this.getPersistanceStorage();
            if (!storage) {
                return;
            }
            var settings = this.getCurrentSettings(flags);
            storage.setItem(this.getPersistanceKey(), $.toJSON(settings));
        };
        DataGrid.prototype.getCurrentSettings = function (flags) {
            var _this = this;
            flags = flags || this.gridPersistanceFlags();
            var settings = {};
            if (flags.columnVisibility !== false || flags.columnWidths !== false || flags.sortColumns !== false) {
                settings.columns = [];
                var sortColumns = this.slickGrid.getSortColumns();
                var columns = this.slickGrid.getColumns();
                for (var _i = 0, columns_3 = columns; _i < columns_3.length; _i++) {
                    var column = columns_3[_i];
                    var p = {
                        id: column.id
                    };
                    if (flags.columnVisibility !== false) {
                        p.visible = true;
                    }
                    if (flags.columnWidths !== false) {
                        p.width = column.width;
                    }
                    if (flags.sortColumns !== false) {
                        var sort = Q.indexOf(sortColumns, function (x) { return x.columnId == column.id; });
                        p.sort = ((sort >= 0) ? ((sortColumns[sort].sortAsc !== false) ? (sort + 1) : (-sort - 1)) : 0);
                    }
                    settings.columns.push(p);
                }
            }
            if (flags.includeDeleted !== false) {
                settings.includeDeleted = this.element.find('.s-IncludeDeletedToggle').hasClass('pressed');
            }
            if (flags.filterItems !== false && (this.filterBar != null) && (this.filterBar.get_store() != null)) {
                settings.filterItems = this.filterBar.get_store().get_items().slice();
            }
            if (flags.quickSearch === true) {
                var qsInput = this.toolbar.element.find('.s-QuickSearchInput').first();
                if (qsInput.length > 0) {
                    var qsWidget = qsInput.tryGetWidget(Serenity.QuickSearchInput);
                    if (qsWidget != null) {
                        settings.quickSearchField = qsWidget.get_field();
                        settings.quickSearchText = qsWidget.element.val();
                    }
                }
            }
            if (flags.quickFilters !== false && (this.quickFiltersDiv != null) && this.quickFiltersDiv.length > 0) {
                settings.quickFilters = {};
                this.quickFiltersDiv.find('.quick-filter-item').each(function (i, e) {
                    var field = $(e).data('qffield');
                    if (Q.isEmptyOrNull(field)) {
                        return;
                    }
                    var widget = $('#' + _this.uniqueName + '_QuickFilter_' + field).tryGetWidget(Serenity.Widget);
                    if (widget == null) {
                        return;
                    }
                    var saveState = $(e).data('qfsavestate');
                    var state = (saveState != null) ? saveState(widget) : Serenity.EditorUtils.getValue(widget);
                    settings.quickFilters[field] = state;
                    if (flags.quickFilterText === true && $(e).hasClass('quick-filter-active')) {
                        var getDisplayText = $(e).data('qfdisplaytext');
                        var filterLabel = $(e).find('.quick-filter-label').text();
                        var displayText;
                        if (getDisplayText != null) {
                            displayText = getDisplayText(widget, filterLabel);
                        }
                        else {
                            displayText = filterLabel + ' = ' + Serenity.EditorUtils.getDisplayText(widget);
                        }
                        if (!Q.isEmptyOrNull(displayText)) {
                            if (!Q.isEmptyOrNull(settings.quickFilterText)) {
                                settings.quickFilterText += ' ' + Q.coalesce(Q.tryGetText('Controls.FilterPanel.And'), 'and') + ' ';
                                settings.quickFilterText += displayText;
                            }
                            else {
                                settings.quickFilterText = displayText;
                            }
                        }
                    }
                });
            }
            return settings;
        };
        DataGrid.prototype.getElement = function () {
            return this.element;
        };
        DataGrid.prototype.getGrid = function () {
            return this.slickGrid;
        };
        DataGrid.prototype.getView = function () {
            return this.view;
        };
        DataGrid.prototype.getFilterStore = function () {
            return (this.filterBar == null) ? null : this.filterBar.get_store();
        };
        var DataGrid_1;
        DataGrid = DataGrid_1 = __decorate([
            Serenity.Decorators.registerClass('Serenity.DataGrid', [IDataGrid, Serenity.IReadOnly]),
            Serenity.Decorators.element("<div/>")
        ], DataGrid);
        return DataGrid;
    }(Serenity.Widget));
    Serenity.DataGrid = DataGrid;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var ColumnPickerDialog = /** @class */ (function (_super) {
        __extends(ColumnPickerDialog, _super);
        function ColumnPickerDialog() {
            var _this = _super.call(this) || this;
            new Serenity.QuickSearchInput(_this.byId("Search"), {
                onSearch: function (fld, txt, done) {
                    txt = Q.trimToNull(txt);
                    if (txt != null)
                        txt = Select2.util.stripDiacritics(txt.toLowerCase());
                    _this.element.find('li').each(function (x, e) {
                        $(e).toggle(!txt || Select2.util.stripDiacritics($(e).text().toLowerCase()).indexOf(txt) >= 0);
                    });
                    done && done(true);
                }
            });
            _this.ulVisible = _this.byId("VisibleCols");
            _this.ulHidden = _this.byId("HiddenCols");
            _this.dialogTitle = Q.text("Controls.ColumnPickerDialog.Title");
            return _this;
        }
        ColumnPickerDialog.createToolButton = function (grid) {
            function onClick() {
                var picker = new Serenity.ColumnPickerDialog();
                picker.allColumns = grid.allColumns;
                if (grid.initialSettings) {
                    var initialSettings = grid.initialSettings;
                    if (initialSettings.columns && initialSettings.columns.length)
                        picker.defaultColumns = initialSettings.columns.map(function (x) { return x.id; });
                }
                picker.visibleColumns = grid.slickGrid.getColumns().map(function (x) { return x.id; });
                picker.done = function () {
                    grid.allColumns = picker.allColumns;
                    var visible = picker.allColumns.filter(function (x) { return x.visible === true; });
                    grid.slickGrid.setColumns(visible);
                    grid.persistSettings();
                    grid.refresh();
                };
                Q["Router"] && Q["Router"].dialog && Q["Router"].dialog(grid.element, picker.element, function () { return "columns"; });
                picker.dialogOpen();
            }
            grid.element.on('handleroute.' + grid.uniqueName, function (e, arg) {
                if (arg && !arg.handled && arg.route == "columns") {
                    onClick();
                }
            });
            return {
                hint: Q.text("Controls.ColumnPickerDialog.Title"),
                cssClass: "column-picker-button",
                onClick: onClick
            };
        };
        ColumnPickerDialog.prototype.getDialogButtons = function () {
            var _this = this;
            return [
                {
                    text: Q.text("Controls.ColumnPickerDialog.RestoreDefaults"),
                    click: function () {
                        var liByKey = {};
                        _this.ulVisible.children().add(_this.ulHidden.children()).each(function (x, e) {
                            var el = $(e);
                            liByKey[el.data('key')] = el;
                        });
                        var last = null;
                        for (var _i = 0, _a = _this.defaultColumns; _i < _a.length; _i++) {
                            var id = _a[_i];
                            var li = liByKey[id];
                            if (!li)
                                continue;
                            if (last == null)
                                li.prependTo(_this.ulVisible);
                            else
                                li.insertAfter(last);
                            last = li;
                            var key = li.data('key');
                            delete liByKey[key];
                        }
                        for (var key in liByKey)
                            liByKey[key].appendTo(_this.ulHidden);
                        _this.updateListStates();
                    }
                },
                {
                    text: Q.text("Dialogs.OkButton"),
                    click: function () {
                        var newColumns = [];
                        for (var _i = 0, _a = _this.allColumns; _i < _a.length; _i++) {
                            var col = _a[_i];
                            col.visible = false;
                        }
                        _this.visibleColumns = _this.ulVisible.children().toArray().map(function (x) {
                            var id = $(x).data("key");
                            var col = _this.colById[id];
                            col.visible = true;
                            newColumns.push(col);
                            return id;
                        });
                        for (var _b = 0, _c = _this.allColumns; _b < _c.length; _b++) {
                            var col = _c[_b];
                            if (!col.visible)
                                newColumns.push(col);
                        }
                        _this.allColumns = newColumns;
                        _this.done && _this.done();
                        _this.dialogClose();
                    }
                },
                {
                    text: Q.text("Dialogs.CancelButton"),
                    click: function () {
                        _this.dialogClose();
                    }
                }
            ];
        };
        ColumnPickerDialog.prototype.getDialogOptions = function () {
            var opt = _super.prototype.getDialogOptions.call(this);
            opt.width = 600;
            return opt;
        };
        ColumnPickerDialog.prototype.getTitle = function (col) {
            if (col.id == "__select__")
                return "[x]";
            return col.name || col.toolTip || col.id;
        };
        ColumnPickerDialog.prototype.allowHide = function (col) {
            return col.sourceItem == null || col.sourceItem.allowHide == null || col.sourceItem.allowHide;
        };
        ColumnPickerDialog.prototype.createLI = function (col) {
            var allowHide = this.allowHide(col);
            return $("\n<li data-key=\"" + col.id + "\" class=\"" + (allowHide ? "" : "cant-hide") + "\">\n  <span class=\"drag-handle\">\u2630</span>\n  " + Q.htmlEncode(this.getTitle(col)) + "\n  " + (allowHide ? "<i class=\"js-hide\" title=\"" + Q.text("Controls.ColumnPickerDialog.HideHint") + "\">\u2716</i>" : '') + "\n  <i class=\"js-show fa fa-eye\" title=\"" + Q.text("Controls.ColumnPickerDialog.ShowHint") + "\"></i>\n</li>");
        };
        ColumnPickerDialog.prototype.updateListStates = function () {
            this.ulVisible.children().removeClass("bg-info").addClass("bg-success");
            this.ulHidden.children().removeClass("bg-success").addClass("bg-info");
        };
        ColumnPickerDialog.prototype.setupColumns = function () {
            var _this = this;
            this.allColumns = this.allColumns || [];
            this.visibleColumns = this.visibleColumns || [];
            var visible = {};
            for (var _i = 0, _a = this.visibleColumns; _i < _a.length; _i++) {
                var id = _a[_i];
                visible[id] = true;
            }
            this.colById = {};
            for (var _b = 0, _c = this.allColumns; _b < _c.length; _b++) {
                var c_1 = _c[_b];
                this.colById[c_1.id] = c_1;
            }
            if (this.defaultColumns == null)
                this.defaultColumns = this.visibleColumns.slice(0);
            var hidden = [];
            for (var _d = 0, _e = this.allColumns; _d < _e.length; _d++) {
                var c_2 = _e[_d];
                if (!visible[c_2.id] && (!c_2.sourceItem ||
                    (c_2.sourceItem.filterOnly !== true &&
                        (c_2.sourceItem.readPermission == null || Q.Authorization.hasPermission(c_2.sourceItem.readPermission))))) {
                    hidden.push(c_2);
                }
            }
            var hiddenColumns = hidden.sort(function (a, b) { return Q.Culture.stringCompare(_this.getTitle(a), _this.getTitle(b)); });
            for (var _f = 0, _g = this.visibleColumns; _f < _g.length; _f++) {
                var id = _g[_f];
                var c = this.colById[id];
                if (!c)
                    continue;
                this.createLI(c).appendTo(this.ulVisible);
            }
            for (var _h = 0, hiddenColumns_1 = hiddenColumns; _h < hiddenColumns_1.length; _h++) {
                var c_3 = hiddenColumns_1[_h];
                this.createLI(c_3).appendTo(this.ulHidden);
            }
            this.updateListStates();
            if (typeof Sortable !== "undefined" &&
                Sortable.create) {
                Sortable.create(this.ulVisible[0], {
                    group: this.uniqueName + "_group",
                    filter: '.js-hide',
                    onFilter: function (evt) {
                        $(evt.item).appendTo(_this.ulHidden);
                        _this.updateListStates();
                    },
                    onMove: function (x) {
                        if ($(x.dragged).hasClass('cant-hide') &&
                            x.from == _this.ulVisible[0] &&
                            x.to !== x.from)
                            return false;
                        return true;
                    },
                    onEnd: function (evt) { return _this.updateListStates(); }
                });
                Sortable.create(this.ulHidden[0], {
                    group: this.uniqueName + "_group",
                    sort: false,
                    filter: '.js-show',
                    onFilter: function (evt) {
                        $(evt.item).appendTo(_this.ulVisible);
                        _this.updateListStates();
                    },
                    onEnd: function (evt) { return _this.updateListStates(); }
                });
            }
        };
        ColumnPickerDialog.prototype.onDialogOpen = function () {
            _super.prototype.onDialogOpen.call(this);
            this.element.find("input").removeAttr("disabled");
            this.element.closest('.ui-dialog').children(".ui-dialog-buttonpane")
                .find('button').eq(0).addClass("restore-defaults")
                .next().focus();
            this.setupColumns();
            Q.centerDialog(this.element);
        };
        ColumnPickerDialog.prototype.getTemplate = function () {
            return "\n<div class=\"search\"><input id=\"~_Search\" type=\"text\" disabled /></div>\n<div class=\"columns-container\">\n<div class=\"column-list visible-list bg-success\">\n  <h5><i class=\"fa fa-eye\"></i> " + Q.text("Controls.ColumnPickerDialog.VisibleColumns") + "</h5>\n  <ul id=\"~_VisibleCols\"></ul>\n</div>\n<div class=\"column-list hidden-list bg-info\">\n  <h5><i class=\"fa fa-list\"></i> " + Q.text("Controls.ColumnPickerDialog.HiddenColumns") + "</h5>\n  <ul id=\"~_HiddenCols\"></ul>\n</div>\n</div>";
        };
        ColumnPickerDialog = __decorate([
            Serenity.Decorators.registerClass(),
            Serenity.Decorators.resizable(),
            Serenity.Decorators.responsive()
        ], ColumnPickerDialog);
        return ColumnPickerDialog;
    }(Serenity.TemplatedDialog));
    Serenity.ColumnPickerDialog = ColumnPickerDialog;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var EntityGrid = /** @class */ (function (_super) {
        __extends(EntityGrid, _super);
        function EntityGrid(container, options) {
            var _this = _super.call(this, container, options) || this;
            _this.element.addClass('route-handler')
                .on('handleroute.' + _this.uniqueName, function (e, arg) {
                if (!!arg.handled)
                    return;
                if (!!(arg.route === 'new')) {
                    arg.handled = true;
                    _this.addButtonClick();
                    return;
                }
                var parts = arg.route.split('/');
                if (!!(parts.length === 2 && parts[0] === 'edit')) {
                    arg.handled = true;
                    _this.editItem(parts[1]);
                    return;
                }
                if (!!(parts.length === 2 && parts[1] === 'new')) {
                    arg.handled = true;
                    _this.editItemOfType(Q.cast(parts[0], String), null);
                    return;
                }
                if (!!(parts.length === 3 && parts[1] === 'edit')) {
                    arg.handled = true;
                    _this.editItemOfType(Q.cast(parts[0], String), parts[2]);
                }
            });
            return _this;
        }
        EntityGrid.prototype.usePager = function () {
            return true;
        };
        EntityGrid.prototype.createToolbarExtensions = function () {
            this.createIncludeDeletedButton();
            this.createQuickSearchInput();
        };
        EntityGrid.prototype.getInitialTitle = function () {
            return this.getDisplayName();
        };
        EntityGrid.prototype.getLocalTextPrefix = function () {
            var result = _super.prototype.getLocalTextPrefix.call(this);
            if (Q.isEmptyOrNull(result)) {
                return this.getEntityType();
            }
            return result;
        };
        EntityGrid.prototype.getEntityType = function () {
            if (this.entityType != null)
                return this.entityType;
            var attr = this.attrs(Serenity.EntityTypeAttribute);
            if (attr.length === 1) {
                return (this.entityType = attr[0].value);
            }
            var name = Q.getTypeFullName(Q.getInstanceType(this));
            var px = name.indexOf('.');
            if (px >= 0) {
                name = name.substring(px + 1);
            }
            if (Q.endsWith(name, 'Grid')) {
                name = name.substr(0, name.length - 4);
            }
            else if (Q.endsWith(name, 'SubGrid')) {
                name = name.substr(0, name.length - 7);
            }
            this.entityType = name;
            return this.entityType;
        };
        EntityGrid.prototype.getDisplayName = function () {
            if (this.displayName != null)
                return this.displayName;
            var attr = this.attrs(System.ComponentModel.DisplayNameAttribute);
            if (attr.length >= 1) {
                this.displayName = attr[0].displayName;
                this.displayName = Q.LT.getDefault(this.displayName, this.displayName);
            }
            else {
                this.displayName = Q.tryGetText(this.getLocalTextDbPrefix() + 'EntityPlural');
                if (this.displayName == null)
                    this.displayName = this.getEntityType();
            }
            return this.displayName;
        };
        EntityGrid.prototype.getItemName = function () {
            if (this.itemName != null)
                return this.itemName;
            var attr = this.attrs(Serenity.ItemNameAttribute);
            if (attr.length >= 1) {
                this.itemName = attr[0].value;
                this.itemName = Q.LT.getDefault(this.itemName, this.itemName);
            }
            else {
                this.itemName = Q.tryGetText(this.getLocalTextDbPrefix() + 'EntitySingular');
                if (this.itemName == null)
                    this.itemName = this.getEntityType();
            }
            return this.itemName;
        };
        EntityGrid.prototype.getAddButtonCaption = function () {
            return Q.format(Q.text('Controls.EntityGrid.NewButton'), this.getItemName());
        };
        EntityGrid.prototype.getButtons = function () {
            var _this = this;
            var buttons = [];
            buttons.push({
                title: this.getAddButtonCaption(),
                cssClass: 'add-button',
                hotkey: 'alt+n',
                onClick: function () {
                    _this.addButtonClick();
                },
                disabled: function () { return !_this.hasInsertPermission() || _this.readOnly; }
            });
            buttons.push(this.newRefreshButton(true));
            buttons.push(Serenity.ColumnPickerDialog.createToolButton(this));
            return buttons;
        };
        EntityGrid.prototype.newRefreshButton = function (noText) {
            var _this = this;
            return {
                title: (noText ? null : Q.text('Controls.EntityGrid.RefreshButton')),
                hint: (noText ? Q.text('Controls.EntityGrid.RefreshButton') : null),
                cssClass: 'refresh-button',
                onClick: function () {
                    _this.refresh();
                }
            };
        };
        EntityGrid.prototype.addButtonClick = function () {
            this.editItem(new Object());
        };
        EntityGrid.prototype.editItem = function (entityOrId) {
            var _this = this;
            this.createEntityDialog(this.getItemType(), function (dlg) {
                var dialog = Q.safeCast(dlg, Serenity['IEditDialog']);
                if (dialog != null) {
                    dialog.load(entityOrId, function () {
                        dialog.dialogOpen(_this.openDialogsAsPanel);
                    });
                    return;
                }
                throw new Error(Q.format("{0} doesn't implement IEditDialog!", Q.getTypeFullName(Q.getInstanceType(dlg))));
            });
        };
        EntityGrid.prototype.editItemOfType = function (itemType, entityOrId) {
            var _this = this;
            if (itemType === this.getItemType()) {
                this.editItem(entityOrId);
                return;
            }
            this.createEntityDialog(itemType, function (dlg) {
                var dialog = Q.safeCast(dlg, Serenity['IEditDialog']);
                if (dialog != null) {
                    dialog.load(entityOrId, function () {
                        return dialog.dialogOpen(_this.openDialogsAsPanel);
                    });
                    return;
                }
                throw new Error(Q.format("{0} doesn't implement IEditDialog!", Q.getTypeFullName(Q.getInstanceType(dlg))));
            });
        };
        EntityGrid.prototype.getService = function () {
            if (this.service != null)
                return this.service;
            var attr = this.attrs(Serenity.ServiceAttribute);
            if (attr.length >= 1)
                this.service = attr[0].value;
            else
                this.service = Q.replaceAll(this.getEntityType(), '.', '/');
            return this.service;
        };
        EntityGrid.prototype.getViewOptions = function () {
            var opt = _super.prototype.getViewOptions.call(this);
            opt.url = Q.resolveUrl('~/Services/' + this.getService() + '/List');
            return opt;
        };
        EntityGrid.prototype.getItemType = function () {
            return this.getEntityType();
        };
        EntityGrid.prototype.routeDialog = function (itemType, dialog) {
            var _this = this;
            Q["Router"] && Q["Router"].dialog && Q["Router"].dialog(this.element, dialog.element, function () {
                var hash = '';
                if (itemType !== _this.getItemType())
                    hash = itemType + '/';
                if (!!(dialog != null && dialog.entityId != null))
                    hash += 'edit/' + dialog.entityId.toString();
                else
                    hash += 'new';
                return hash;
            });
        };
        EntityGrid.prototype.getInsertPermission = function () {
            return null;
        };
        EntityGrid.prototype.hasInsertPermission = function () {
            var insertPermission = this.getInsertPermission();
            return insertPermission == null || Q.Authorization.hasPermission(this.getInsertPermission());
        };
        EntityGrid.prototype.transferDialogReadOnly = function (dialog) {
            if (this.readOnly)
                Serenity.EditorUtils.setReadOnly(dialog, true);
        };
        EntityGrid.prototype.initDialog = function (dialog) {
            var _this = this;
            Serenity.SubDialogHelper.bindToDataChange(dialog, this, function (e, dci) {
                _this.subDialogDataChange();
            }, true);
            this.transferDialogReadOnly(dialog);
            this.routeDialog(this.getItemType(), dialog);
        };
        EntityGrid.prototype.initEntityDialog = function (itemType, dialog) {
            var _this = this;
            if (itemType === this.getItemType()) {
                this.initDialog(dialog);
                return;
            }
            Serenity.SubDialogHelper.bindToDataChange(dialog, this, function (e, dci) {
                _this.subDialogDataChange();
            }, true);
            this.transferDialogReadOnly(dialog);
            this.routeDialog(itemType, dialog);
        };
        EntityGrid.prototype.createEntityDialog = function (itemType, callback) {
            var _this = this;
            var dialogClass = this.getDialogTypeFor(itemType);
            var dialog = Serenity.Widget.create({
                type: dialogClass,
                options: this.getDialogOptionsFor(itemType),
                init: function (d) {
                    _this.initEntityDialog(itemType, d);
                    callback && callback(d);
                }
            });
            return dialog;
        };
        EntityGrid.prototype.getDialogOptions = function () {
            return {};
        };
        EntityGrid.prototype.getDialogOptionsFor = function (itemType) {
            if (itemType === this.getItemType())
                return this.getDialogOptions();
            return {};
        };
        EntityGrid.prototype.getDialogTypeFor = function (itemType) {
            if (itemType === this.getItemType()) {
                return this.getDialogType();
            }
            return Serenity.DialogTypeRegistry.get(itemType);
        };
        EntityGrid.prototype.getDialogType = function () {
            if (this.dialogType != null)
                return this.dialogType;
            var attr = this.attrs(Serenity.DialogTypeAttribute);
            if (attr.length >= 1)
                this.dialogType = attr[0].value;
            else
                this.dialogType = Serenity.DialogTypeRegistry.get(this.getEntityType());
            return this.dialogType;
        };
        EntityGrid = __decorate([
            Serenity.Decorators.registerClass('Serenity.EntityGrid')
        ], EntityGrid);
        return EntityGrid;
    }(Serenity.DataGrid));
    Serenity.EntityGrid = EntityGrid;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    /**
     * A mixin that can be applied to a DataGrid for tree functionality
     */
    var TreeGridMixin = /** @class */ (function () {
        function TreeGridMixin(options) {
            this.options = options;
            var dg = this.dataGrid = options.grid;
            var idProperty = dg.getIdProperty();
            var getId = this.getId = function (item) { return item[idProperty]; };
            dg.element.find('.grid-container').on('click', function (e) {
                if ($(e.target).hasClass('s-TreeToggle')) {
                    var src = dg.slickGrid.getCellFromEvent(e);
                    if (src.cell >= 0 &&
                        src.row >= 0) {
                        Serenity.SlickTreeHelper.toggleClick(e, src.row, src.row, dg.view, getId);
                    }
                }
            });
            var oldViewFilter = dg.onViewFilter;
            dg.onViewFilter = function (item) {
                if (!oldViewFilter.apply(this, [item]))
                    return false;
                return Serenity.SlickTreeHelper.filterById(item, dg.view, options.getParentId);
            };
            var oldProcessData = dg.onViewProcessData;
            dg.onViewProcessData = function (response) {
                response = oldProcessData.apply(this, [response]);
                response.Entities = TreeGridMixin.applyTreeOrdering(response.Entities, getId, options.getParentId);
                Serenity.SlickTreeHelper.setIndents(response.Entities, getId, options.getParentId, (options.initialCollapse && options.initialCollapse()) || false);
                return response;
            };
            if (options.toggleField) {
                var col = Q.tryFirst(dg['allColumns'] || dg.slickGrid.getColumns() || [], function (x) { return x.field == options.toggleField; });
                if (col) {
                    col.format = Serenity.SlickFormatting.treeToggle(function () { return dg.view; }, getId, col.format || (function (ctx) { return Q.htmlEncode(ctx.value); }));
                    col.formatter = Serenity.SlickHelper.convertToFormatter(col.format);
                }
            }
        }
        /**
         * Expands / collapses all rows in a grid automatically
         */
        TreeGridMixin.prototype.toggleAll = function () {
            Serenity.SlickTreeHelper.setCollapsed(this.dataGrid.view.getItems(), !this.dataGrid.view.getItems().every(function (x) { return x._collapsed == true; }));
            this.dataGrid.view.setItems(this.dataGrid.view.getItems(), true);
        };
        TreeGridMixin.prototype.collapseAll = function () {
            Serenity.SlickTreeHelper.setCollapsed(this.dataGrid.view.getItems(), true);
            this.dataGrid.view.setItems(this.dataGrid.view.getItems(), true);
        };
        TreeGridMixin.prototype.expandAll = function () {
            Serenity.SlickTreeHelper.setCollapsed(this.dataGrid.view.getItems(), false);
            this.dataGrid.view.setItems(this.dataGrid.view.getItems(), true);
        };
        /**
         * Reorders a set of items so that parents comes before their children.
         * This method is required for proper tree ordering, as it is not so easy to perform with SQL.
         * @param items list of items to be ordered
         * @param getId a delegate to get ID of a record (must return same ID with grid identity field)
         * @param getParentId a delegate to get parent ID of a record
         */
        TreeGridMixin.applyTreeOrdering = function (items, getId, getParentId) {
            var result = [];
            var byId = Q.toGrouping(items, getId);
            var byParentId = Q.toGrouping(items, getParentId);
            var visited = {};
            function takeChildren(theParentId) {
                if (visited[theParentId])
                    return;
                visited[theParentId] = true;
                for (var _i = 0, _a = (byParentId[theParentId] || []); _i < _a.length; _i++) {
                    var child = _a[_i];
                    result.push(child);
                    takeChildren(getId(child));
                }
            }
            for (var _i = 0, items_2 = items; _i < items_2.length; _i++) {
                var item = items_2[_i];
                var parentId = getParentId(item);
                if (parentId == null ||
                    !((byId[parentId] || []).length)) {
                    result.push(item);
                    takeChildren(getId(item));
                }
            }
            return result;
        };
        return TreeGridMixin;
    }());
    Serenity.TreeGridMixin = TreeGridMixin;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var CheckTreeEditor = /** @class */ (function (_super) {
        __extends(CheckTreeEditor, _super);
        function CheckTreeEditor(div, opt) {
            var _this = _super.call(this, div, opt) || this;
            div.addClass('s-CheckTreeEditor');
            _this.updateItems();
            return _this;
        }
        CheckTreeEditor.prototype.getIdProperty = function () {
            return "id";
        };
        CheckTreeEditor.prototype.getTreeItems = function () {
            return [];
        };
        CheckTreeEditor.prototype.updateItems = function () {
            var items = this.getTreeItems();
            var itemById = {};
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                item.children = [];
                if (!Q.isEmptyOrNull(item.id)) {
                    itemById[item.id] = item;
                }
                if (!Q.isEmptyOrNull(item.parentId)) {
                    var parent = itemById[item.parentId];
                    if (parent != null) {
                        parent.children.push(item);
                    }
                }
            }
            this.view.addData({ Entities: items, Skip: 0, Take: 0, TotalCount: items.length });
            this.updateSelectAll();
            this.updateFlags();
        };
        CheckTreeEditor.prototype.getEditValue = function (property, target) {
            if (this.getDelimited())
                target[property.name] = this.get_value().join(",");
            else
                target[property.name] = this.get_value();
        };
        CheckTreeEditor.prototype.setEditValue = function (source, property) {
            var value = source[property.name];
            this.set_value(value);
        };
        CheckTreeEditor.prototype.getButtons = function () {
            var _this = this;
            var selectAllText = this.getSelectAllText();
            if (Q.isEmptyOrNull(selectAllText)) {
                return null;
            }
            var self = this;
            var buttons = [];
            buttons.push(Serenity.GridSelectAllButtonHelper.define(function () {
                return self;
            }, function (x) {
                return x.id;
            }, function (x1) {
                return x1.isSelected;
            }, function (x2, v) {
                if (x2.isSelected !== v) {
                    x2.isSelected = v;
                    _this.itemSelectedChanged(x2);
                }
            }, null, function () {
                _this.updateFlags();
            }));
            return buttons;
        };
        CheckTreeEditor.prototype.itemSelectedChanged = function (item) {
        };
        CheckTreeEditor.prototype.getSelectAllText = function () {
            return Q.coalesce(Q.tryGetText('Controls.CheckTreeEditor.SelectAll'), 'Select All');
        };
        CheckTreeEditor.prototype.isThreeStateHierarchy = function () {
            return false;
        };
        CheckTreeEditor.prototype.createSlickGrid = function () {
            this.element.addClass('slick-no-cell-border').addClass('slick-no-odd-even');
            var result = _super.prototype.createSlickGrid.call(this);
            this.element.addClass('slick-hide-header');
            result.resizeCanvas();
            return result;
        };
        CheckTreeEditor.prototype.onViewFilter = function (item) {
            if (!_super.prototype.onViewFilter.call(this, item)) {
                return false;
            }
            var items = this.view.getItems();
            var self = this;
            return Serenity.SlickTreeHelper.filterCustom(item, function (x) {
                if (x.parentId == null) {
                    return null;
                }
                if (self.byId == null) {
                    self.byId = {};
                    for (var i = 0; i < items.length; i++) {
                        var o = items[i];
                        if (o.id != null) {
                            self.byId[o.id] = o;
                        }
                    }
                }
                return self.byId[x.parentId];
            });
        };
        CheckTreeEditor.prototype.getInitialCollapse = function () {
            return false;
        };
        CheckTreeEditor.prototype.onViewProcessData = function (response) {
            response = _super.prototype.onViewProcessData.call(this, response);
            this.byId = null;
            Serenity.SlickTreeHelper.setIndents(response.Entities, function (x) {
                return x.id;
            }, function (x1) {
                return x1.parentId;
            }, this.getInitialCollapse());
            return response;
        };
        CheckTreeEditor.prototype.onClick = function (e, row, cell) {
            _super.prototype.onClick.call(this, e, row, cell);
            if (!e.isDefaultPrevented()) {
                Serenity.SlickTreeHelper.toggleClick(e, row, cell, this.view, function (x) {
                    return x.id;
                });
            }
            if (e.isDefaultPrevented()) {
                return;
            }
            var target = $(e.target);
            if (target.hasClass('check-box')) {
                e.preventDefault();
                if (this._readOnly)
                    return;
                var checkedOrPartial = target.hasClass('checked') || target.hasClass('partial');
                var item = this.itemAt(row);
                var anyChanged = item.isSelected !== !checkedOrPartial;
                this.view.beginUpdate();
                try {
                    if (item.isSelected !== !checkedOrPartial) {
                        item.isSelected = !checkedOrPartial;
                        this.view.updateItem(item.id, item);
                        this.itemSelectedChanged(item);
                    }
                    anyChanged = this.setAllSubTreeSelected(item, item.isSelected) || anyChanged;
                    this.updateSelectAll();
                    this.updateFlags();
                }
                finally {
                    this.view.endUpdate();
                }
                if (anyChanged) {
                    this.element.triggerHandler('change');
                }
            }
        };
        CheckTreeEditor.prototype.updateSelectAll = function () {
            Serenity.GridSelectAllButtonHelper.update(this, function (x) {
                return x.isSelected;
            });
        };
        CheckTreeEditor.prototype.updateFlags = function () {
            var view = this.view;
            var items = view.getItems();
            var threeState = this.isThreeStateHierarchy();
            if (!threeState) {
                return;
            }
            view.beginUpdate();
            try {
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (item.children == null || item.children.length === 0) {
                        var allsel = this.getDescendantsSelected(item);
                        if (allsel !== item.isAllDescendantsSelected) {
                            item.isAllDescendantsSelected = allsel;
                            view.updateItem(item.id, item);
                        }
                        continue;
                    }
                    var allSelected = this.allDescendantsSelected(item);
                    var selected = allSelected || this.anyDescendantsSelected(item);
                    if (allSelected !== item.isAllDescendantsSelected || selected !== item.isSelected) {
                        var selectedChange = item.isSelected !== selected;
                        item.isAllDescendantsSelected = allSelected;
                        item.isSelected = selected;
                        view.updateItem(item.id, item);
                        if (selectedChange) {
                            this.itemSelectedChanged(item);
                        }
                    }
                }
            }
            finally {
                view.endUpdate();
            }
        };
        CheckTreeEditor.prototype.getDescendantsSelected = function (item) {
            return true;
        };
        CheckTreeEditor.prototype.setAllSubTreeSelected = function (item, selected) {
            var result = false;
            for (var i = 0; i < item.children.length; i++) {
                var sub = item.children[i];
                if (sub.isSelected !== selected) {
                    result = true;
                    sub.isSelected = selected;
                    this.view.updateItem(sub.id, sub);
                    this.itemSelectedChanged(sub);
                }
                if (sub.children.length > 0) {
                    result = this.setAllSubTreeSelected(sub, selected) || result;
                }
            }
            return result;
        };
        CheckTreeEditor.prototype.allItemsSelected = function () {
            for (var i = 0; i < this.rowCount(); i++) {
                var row = this.itemAt(i);
                if (!row.isSelected) {
                    return false;
                }
            }
            return this.rowCount() > 0;
        };
        CheckTreeEditor.prototype.allDescendantsSelected = function (item) {
            if (item.children.length > 0) {
                for (var i = 0; i < item.children.length; i++) {
                    var sub = item.children[i];
                    if (!sub.isSelected) {
                        return false;
                    }
                    if (!this.allDescendantsSelected(sub)) {
                        return false;
                    }
                }
            }
            return true;
        };
        CheckTreeEditor.prototype.getDelimited = function () {
            return !!!!this.options['delimited'];
        };
        CheckTreeEditor.prototype.anyDescendantsSelected = function (item) {
            if (item.children.length > 0) {
                for (var i = 0; i < item.children.length; i++) {
                    var sub = item.children[i];
                    if (sub.isSelected) {
                        return true;
                    }
                    if (this.anyDescendantsSelected(sub)) {
                        return true;
                    }
                }
            }
            return false;
        };
        CheckTreeEditor.prototype.getColumns = function () {
            var _this = this;
            var self = this;
            var columns = [];
            columns.push({
                field: 'text', name: 'Kayıt', width: 80, format: Serenity.SlickFormatting.treeToggle(function () {
                    return self.view;
                }, function (x) {
                    return x.id;
                }, function (ctx) {
                    var cls = 'check-box';
                    var item = ctx.item;
                    if (item.hideCheckBox) {
                        return _this.getItemText(ctx);
                    }
                    var threeState = _this.isThreeStateHierarchy();
                    if (item.isSelected) {
                        if (threeState && !item.isAllDescendantsSelected) {
                            cls += ' partial';
                        }
                        else {
                            cls += ' checked';
                        }
                    }
                    if (_this._readOnly)
                        cls += ' readonly';
                    return '<span class="' + cls + '"></span>' + _this.getItemText(ctx);
                })
            });
            return columns;
        };
        CheckTreeEditor.prototype.getItemText = function (ctx) {
            return Q.htmlEncode(ctx.value);
        };
        CheckTreeEditor.prototype.getSlickOptions = function () {
            var opt = _super.prototype.getSlickOptions.call(this);
            opt.forceFitColumns = true;
            return opt;
        };
        CheckTreeEditor.prototype.sortItems = function () {
            if (!this.moveSelectedUp()) {
                return;
            }
            var oldIndexes = {};
            var list = this.view.getItems();
            var i = 0;
            for (var $t1 = 0; $t1 < list.length; $t1++) {
                var x = list[$t1];
                oldIndexes[x.id] = i++;
            }
            list.sort(function (x1, y) {
                if (x1.isSelected && !y.isSelected) {
                    return -1;
                }
                if (y.isSelected && !x1.isSelected) {
                    return 1;
                }
                var c = Q.Culture.stringCompare(x1.text, y.text);
                if (c !== 0) {
                    return c;
                }
                return oldIndexes[x1.id] < oldIndexes[y.id] ? -1 : (oldIndexes[x1.id] > oldIndexes[y.id] ? 1 : 0);
            });
            this.view.setItems(list, true);
        };
        CheckTreeEditor.prototype.moveSelectedUp = function () {
            return false;
        };
        CheckTreeEditor.prototype.get_readOnly = function () {
            return this._readOnly;
        };
        CheckTreeEditor.prototype.set_readOnly = function (value) {
            if (!!this._readOnly != !!value) {
                this._readOnly = !!value;
                this.view.refresh();
            }
        };
        CheckTreeEditor.prototype.get_value = function () {
            var list = [];
            var items = this.view.getItems();
            for (var i = 0; i < items.length; i++) {
                if (items[i].isSelected) {
                    list.push(items[i].id);
                }
            }
            return list;
        };
        Object.defineProperty(CheckTreeEditor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        CheckTreeEditor.prototype.set_value = function (value) {
            var selected = {};
            if (value != null) {
                if (typeof value == "string") {
                    value = value.split(',')
                        .map(function (x) { return Q.trimToNull(x); })
                        .filter(function (x) { return x != null; });
                }
                for (var i = 0; i < value.length; i++) {
                    selected[value[i]] = true;
                }
            }
            this.view.beginUpdate();
            try {
                var items = this.view.getItems();
                for (var i1 = 0; i1 < items.length; i1++) {
                    var item = items[i1];
                    var select = selected[item.id];
                    if (select !== item.isSelected) {
                        item.isSelected = select;
                        this.view.updateItem(item.id, item);
                    }
                }
                this.updateSelectAll();
                this.updateFlags();
                this.sortItems();
            }
            finally {
                this.view.endUpdate();
            }
        };
        CheckTreeEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.CheckTreeEditor', [Serenity.IGetEditValue, Serenity.ISetEditValue, Serenity.IReadOnly]),
            Serenity.Decorators.element("<div/>")
        ], CheckTreeEditor);
        return CheckTreeEditor;
    }(Serenity.DataGrid));
    Serenity.CheckTreeEditor = CheckTreeEditor;
    var CheckLookupEditor = /** @class */ (function (_super) {
        __extends(CheckLookupEditor, _super);
        function CheckLookupEditor(div, options) {
            var _this = _super.call(this, div, options) || this;
            _this.enableUpdateItems = true;
            _this.setCascadeFrom(_this.options.cascadeFrom);
            _this.updateItems();
            Q.ScriptData.bindToChange('Lookup.' + _this.getLookupKey(), _this.uniqueName, function () { return _this.updateItems(); });
            return _this;
        }
        CheckLookupEditor.prototype.updateItems = function () {
            if (this.enableUpdateItems)
                _super.prototype.updateItems.call(this);
        };
        CheckLookupEditor.prototype.getLookupKey = function () {
            return this.options.lookupKey;
        };
        CheckLookupEditor.prototype.getButtons = function () {
            return Q.coalesce(_super.prototype.getButtons.call(this), this.options.hideSearch ? null : []);
        };
        CheckLookupEditor.prototype.createToolbarExtensions = function () {
            var _this = this;
            _super.prototype.createToolbarExtensions.call(this);
            Serenity.GridUtils.addQuickSearchInputCustom(this.toolbar.element, function (field, text) {
                _this.searchText = Select2.util.stripDiacritics(text || '').toUpperCase();
                _this.view.setItems(_this.view.getItems(), true);
            });
        };
        CheckLookupEditor.prototype.getSelectAllText = function () {
            if (!this.options.showSelectAll)
                return null;
            return _super.prototype.getSelectAllText.call(this);
        };
        CheckLookupEditor.prototype.cascadeItems = function (items) {
            var val = this.get_cascadeValue();
            if (val == null || val === '') {
                if (!Q.isEmptyOrNull(this.get_cascadeField())) {
                    return [];
                }
                return items;
            }
            var key = val.toString();
            var fld = this.get_cascadeField();
            return items.filter(function (x) {
                var itemKey = Q.coalesce(x[fld], Serenity.ReflectionUtils.getPropertyValue(x, fld));
                return !!(itemKey != null && itemKey.toString() === key);
            });
        };
        CheckLookupEditor.prototype.filterItems = function (items) {
            var val = this.get_filterValue();
            if (val == null || val === '') {
                return items;
            }
            var key = val.toString();
            var fld = this.get_filterField();
            return items.filter(function (x) {
                var itemKey = Q.coalesce(x[fld], Serenity.ReflectionUtils.getPropertyValue(x, fld));
                return !!(itemKey != null && itemKey.toString() === key);
            });
        };
        CheckLookupEditor.prototype.getLookupItems = function (lookup) {
            return this.filterItems(this.cascadeItems(lookup.items));
        };
        CheckLookupEditor.prototype.getTreeItems = function () {
            var lookup = Q.getLookup(this.options.lookupKey);
            var items = this.getLookupItems(lookup);
            return items.map(function (item) { return ({
                id: Q.coalesce(item[lookup.idField], "").toString(),
                text: Q.coalesce(item[lookup.textField], "").toString(),
                source: item
            }); });
        };
        CheckLookupEditor.prototype.onViewFilter = function (item) {
            return _super.prototype.onViewFilter.call(this, item) &&
                (Q.isEmptyOrNull(this.searchText) ||
                    Select2.util.stripDiacritics(item.text || '')
                        .toUpperCase().indexOf(this.searchText) >= 0);
        };
        CheckLookupEditor.prototype.moveSelectedUp = function () {
            return this.options.checkedOnTop;
        };
        CheckLookupEditor.prototype.get_cascadeFrom = function () {
            return this.options.cascadeFrom;
        };
        Object.defineProperty(CheckLookupEditor.prototype, "cascadeFrom", {
            get: function () {
                return this.get_cascadeFrom();
            },
            set: function (value) {
                this.set_cascadeFrom(value);
            },
            enumerable: true,
            configurable: true
        });
        CheckLookupEditor.prototype.getCascadeFromValue = function (parent) {
            return Serenity.EditorUtils.getValue(parent);
        };
        CheckLookupEditor.prototype.setCascadeFrom = function (value) {
            var _this = this;
            if (Q.isEmptyOrNull(value)) {
                if (this.cascadeLink != null) {
                    this.cascadeLink.set_parentID(null);
                    this.cascadeLink = null;
                }
                this.options.cascadeFrom = null;
                return;
            }
            this.cascadeLink = new Serenity.CascadedWidgetLink(Serenity.Widget, this, function (p) {
                _this.set_cascadeValue(_this.getCascadeFromValue(p));
            });
            this.cascadeLink.set_parentID(value);
            this.options.cascadeFrom = value;
        };
        CheckLookupEditor.prototype.set_cascadeFrom = function (value) {
            if (value !== this.options.cascadeFrom) {
                this.setCascadeFrom(value);
                this.updateItems();
            }
        };
        CheckLookupEditor.prototype.get_cascadeField = function () {
            return Q.coalesce(this.options.cascadeField, this.options.cascadeFrom);
        };
        Object.defineProperty(CheckLookupEditor.prototype, "cascadeField", {
            get: function () {
                return this.get_cascadeField();
            },
            set: function (value) {
                this.set_cascadeField(value);
            },
            enumerable: true,
            configurable: true
        });
        CheckLookupEditor.prototype.set_cascadeField = function (value) {
            this.options.cascadeField = value;
        };
        CheckLookupEditor.prototype.get_cascadeValue = function () {
            return this.options.cascadeValue;
        };
        Object.defineProperty(CheckLookupEditor.prototype, "cascadeValue", {
            get: function () {
                return this.get_cascadeValue();
            },
            set: function (value) {
                this.set_cascadeValue(value);
            },
            enumerable: true,
            configurable: true
        });
        CheckLookupEditor.prototype.set_cascadeValue = function (value) {
            if (this.options.cascadeValue !== value) {
                this.options.cascadeValue = value;
                this.value = [];
                this.updateItems();
            }
        };
        CheckLookupEditor.prototype.get_filterField = function () {
            return this.options.filterField;
        };
        Object.defineProperty(CheckLookupEditor.prototype, "filterField", {
            get: function () {
                return this.get_filterField();
            },
            set: function (value) {
                this.set_filterField(value);
            },
            enumerable: true,
            configurable: true
        });
        CheckLookupEditor.prototype.set_filterField = function (value) {
            this.options.filterField = value;
        };
        CheckLookupEditor.prototype.get_filterValue = function () {
            return this.options.filterValue;
        };
        Object.defineProperty(CheckLookupEditor.prototype, "filterValue", {
            get: function () {
                return this.get_filterValue();
            },
            set: function (value) {
                this.set_filterValue(value);
            },
            enumerable: true,
            configurable: true
        });
        CheckLookupEditor.prototype.set_filterValue = function (value) {
            if (this.options.filterValue !== value) {
                this.options.filterValue = value;
                this.value = null;
                this.updateItems();
            }
        };
        CheckLookupEditor = __decorate([
            Serenity.Decorators.registerEditor("Serenity.CheckLookupEditor")
        ], CheckLookupEditor);
        return CheckLookupEditor;
    }(CheckTreeEditor));
    Serenity.CheckLookupEditor = CheckLookupEditor;
})(Serenity || (Serenity = {}));
//# sourceMappingURL=serenity-grids.js.map