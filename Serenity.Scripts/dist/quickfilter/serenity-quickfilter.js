var Serenity;
(function (Serenity) {
    var QuickFilterBar = /** @class */ (function (_super) {
        __extends(QuickFilterBar, _super);
        function QuickFilterBar(container, options) {
            var _this = _super.call(this, container, options) || this;
            container.addClass('quick-filters-bar').addClass('clear');
            var filters = _this.options.filters;
            for (var f = 0; f < filters.length; f++) {
                var filter = filters[f];
                _this.add(filter);
            }
            _this.options.idPrefix = Q.coalesce(_this.options.idPrefix, _this.uniqueName + '_');
            return _this;
        }
        QuickFilterBar_1 = QuickFilterBar;
        QuickFilterBar.prototype.addSeparator = function () {
            this.element.append($('<hr/>'));
        };
        QuickFilterBar.prototype.add = function (opt) {
            var _this = this;
            if (opt == null) {
                throw new Q.ArgumentNullException('opt');
            }
            if (opt.separator) {
                this.addSeparator();
            }
            var item = $("<div class='quick-filter-item'><span class='quick-filter-label'></span></div>")
                .appendTo(this.element)
                .data('qffield', opt.field).children();
            var title = opt.title;
            if (title == null) {
                title = this.options.getTitle ? this.options.getTitle(opt) : null;
                if (title == null) {
                    title = opt.field;
                }
            }
            var quickFilter = item.text(title).parent();
            if (opt.displayText != null) {
                quickFilter.data('qfdisplaytext', opt.displayText);
            }
            if (opt.saveState != null) {
                quickFilter.data('qfsavestate', opt.saveState);
            }
            if (opt.loadState != null) {
                quickFilter.data('qfloadstate', opt.loadState);
            }
            if (!Q.isEmptyOrNull(opt.cssClass)) {
                quickFilter.addClass(opt.cssClass);
            }
            var widget = Serenity.Widget.create({
                type: opt.type,
                element: function (e) {
                    if (!Q.isEmptyOrNull(opt.field)) {
                        e.attr('id', _this.options.idPrefix + opt.field);
                    }
                    e.attr('placeholder', ' ');
                    e.appendTo(quickFilter);
                    if (opt.element != null) {
                        opt.element(e);
                    }
                },
                options: opt.options,
                init: opt.init
            });
            var submitHandler = function (request) {
                if (quickFilter.hasClass('ignore')) {
                    return;
                }
                request.EqualityFilter = request.EqualityFilter || {};
                var value = Serenity.EditorUtils.getValue(widget);
                var active = value != null && !Q.isEmptyOrNull(value.toString());
                if (opt.handler != null) {
                    var args = {
                        field: opt.field,
                        request: request,
                        equalityFilter: request.EqualityFilter,
                        value: value,
                        active: active,
                        widget: widget,
                        handled: true
                    };
                    opt.handler(args);
                    quickFilter.toggleClass('quick-filter-active', args.active);
                    if (!args.handled) {
                        request.EqualityFilter[opt.field] = value;
                    }
                }
                else {
                    request.EqualityFilter[opt.field] = value;
                    quickFilter.toggleClass('quick-filter-active', active);
                }
            };
            widget.changeSelect2(function (e1) {
                // use timeout give cascaded dropdowns a chance to update / clear themselves
                window.setTimeout(function () { return _this.onChange && _this.onChange(e1); }, 0);
            });
            this.add_submitHandlers(submitHandler);
            widget.element.bind('remove.' + this.uniqueName, function (x) {
                _this.remove_submitHandlers(submitHandler);
            });
            return widget;
        };
        QuickFilterBar.prototype.addDateRange = function (field, title) {
            return this.add(QuickFilterBar_1.dateRange(field, title));
        };
        QuickFilterBar.dateRange = function (field, title) {
            var end = null;
            return {
                field: field,
                type: Serenity.DateEditor,
                title: title,
                element: function (e1) {
                    end = Serenity.Widget.create({
                        type: Serenity.DateEditor,
                        element: function (e2) {
                            e2.insertAfter(e1);
                        },
                        options: null,
                        init: null
                    });
                    end.element.change(function (x) {
                        e1.triggerHandler('change');
                    });
                    $('<span/>').addClass('range-separator').text('-').insertAfter(e1);
                },
                handler: function (args) {
                    var active1 = !Q.isTrimmedEmpty(args.widget.value);
                    var active2 = !Q.isTrimmedEmpty(end.value);
                    if (active1 && !Q.parseDate(args.widget.element.val())) {
                        active1 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        args.widget.element.val('');
                    }
                    if (active2 && !Q.parseDate(end.element.val())) {
                        active2 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        end.element.val('');
                    }
                    args.active = active1 || active2;
                    if (active1) {
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and', [[args.field], '>=', args.widget.value]);
                    }
                    if (active2) {
                        var next = new Date(end.valueAsDate.valueOf());
                        next.setDate(next.getDate() + 1);
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and', [[args.field], '<', Q.formatDate(next, 'yyyy-MM-dd')]);
                    }
                },
                displayText: function (w, l) {
                    var v1 = Serenity.EditorUtils.getDisplayText(w);
                    var v2 = Serenity.EditorUtils.getDisplayText(end);
                    if (Q.isEmptyOrNull(v1) && Q.isEmptyOrNull(v2)) {
                        return null;
                    }
                    var text1 = l + ' >= ' + v1;
                    var text2 = l + ' <= ' + v2;
                    if (!Q.isEmptyOrNull(v1) && !Q.isEmptyOrNull(v2)) {
                        return text1 + ' ' + Q.coalesce(Q.tryGetText('Controls.FilterPanel.And'), 'and') + ' ' + text2;
                    }
                    else if (!Q.isEmptyOrNull(v1)) {
                        return text1;
                    }
                    else {
                        return text2;
                    }
                },
                saveState: function (w1) {
                    return [Serenity.EditorUtils.getValue(w1), Serenity.EditorUtils.getValue(end)];
                },
                loadState: function (w2, state) {
                    if (state == null || !Q.isArray(state) || state.length !== 2) {
                        state = [null, null];
                    }
                    Serenity.EditorUtils.setValue(w2, state[0]);
                    Serenity.EditorUtils.setValue(end, state[1]);
                }
            };
        };
        QuickFilterBar.prototype.addDateTimeRange = function (field, title) {
            return this.add(QuickFilterBar_1.dateTimeRange(field, title));
        };
        QuickFilterBar.dateTimeRange = function (field, title) {
            var end = null;
            return {
                field: field,
                type: Serenity.DateTimeEditor,
                title: title,
                element: function (e1) {
                    end = Serenity.Widget.create({
                        type: Serenity.DateTimeEditor,
                        element: function (e2) {
                            e2.insertAfter(e1);
                        },
                        options: null,
                        init: null
                    });
                    end.element.change(function (x) {
                        e1.triggerHandler('change');
                    });
                    $('<span/>').addClass('range-separator').text('-').insertAfter(e1);
                },
                init: function (i) {
                    i.element.parent().find('.time').change(function (x1) {
                        i.element.triggerHandler('change');
                    });
                },
                handler: function (args) {
                    var active1 = !Q.isTrimmedEmpty(args.widget.value);
                    var active2 = !Q.isTrimmedEmpty(end.value);
                    if (active1 && !Q.parseDate(args.widget.element.val())) {
                        active1 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        args.widget.element.val('');
                    }
                    if (active2 && !Q.parseDate(end.element.val())) {
                        active2 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        end.element.val('');
                    }
                    args.active = active1 || active2;
                    if (active1) {
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and', [[args.field], '>=', args.widget.value]);
                    }
                    if (active2) {
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and', [[args.field], '<=', end.value]);
                    }
                },
                displayText: function (w, l) {
                    var v1 = Serenity.EditorUtils.getDisplayText(w);
                    var v2 = Serenity.EditorUtils.getDisplayText(end);
                    if (Q.isEmptyOrNull(v1) && Q.isEmptyOrNull(v2)) {
                        return null;
                    }
                    var text1 = l + ' >= ' + v1;
                    var text2 = l + ' <= ' + v2;
                    if (!Q.isEmptyOrNull(v1) && !Q.isEmptyOrNull(v2)) {
                        return text1 + ' ' + Q.coalesce(Q.tryGetText('Controls.FilterPanel.And'), 'and') + ' ' + text2;
                    }
                    else if (!Q.isEmptyOrNull(v1)) {
                        return text1;
                    }
                    else {
                        return text2;
                    }
                },
                saveState: function (w1) {
                    return [Serenity.EditorUtils.getValue(w1), Serenity.EditorUtils.getValue(end)];
                },
                loadState: function (w2, state) {
                    if (state == null || !Q.isArray(state) || state.length !== 2) {
                        state = [null, null];
                    }
                    Serenity.EditorUtils.setValue(w2, state[0]);
                    Serenity.EditorUtils.setValue(end, state[1]);
                }
            };
        };
        QuickFilterBar.prototype.addBoolean = function (field, title, yes, no) {
            return this.add(QuickFilterBar_1.boolean(field, title, yes, no));
        };
        QuickFilterBar.boolean = function (field, title, yes, no) {
            var opt = {};
            var items = [];
            var trueText = yes;
            if (trueText == null) {
                trueText = Q.text('Controls.FilterPanel.OperatorNames.true');
            }
            items.push(['1', trueText]);
            var falseText = no;
            if (falseText == null) {
                falseText = Q.text('Controls.FilterPanel.OperatorNames.false');
            }
            items.push(['0', falseText]);
            opt.items = items;
            return {
                field: field,
                type: Serenity.SelectEditor,
                title: title,
                options: opt,
                handler: function (args) {
                    args.equalityFilter[args.field] = args.value == null || Q.isEmptyOrNull(args.value.toString()) ?
                        null : !!Q.toId(args.value);
                }
            };
        };
        QuickFilterBar.prototype.destroy = function () {
            this.submitHandlers = null;
            _super.prototype.destroy.call(this);
        };
        QuickFilterBar.prototype.onSubmit = function (request) {
            this.submitHandlers && this.submitHandlers(request);
        };
        QuickFilterBar.prototype.add_submitHandlers = function (action) {
            this.submitHandlers = Q.delegateCombine(this.submitHandlers, action);
        };
        QuickFilterBar.prototype.remove_submitHandlers = function (action) {
            this.submitHandlers = Q.delegateRemove(this.submitHandlers, action);
        };
        QuickFilterBar.prototype.clear_submitHandlers = function () {
        };
        QuickFilterBar.prototype.find = function (type, field) {
            return $('#' + this.options.idPrefix + field).getWidget(type);
        };
        QuickFilterBar.prototype.tryFind = function (type, field) {
            var el = $('#' + this.options.idPrefix + field);
            if (!el.length)
                return null;
            return el.tryGetWidget(type);
        };
        var QuickFilterBar_1;
        QuickFilterBar = QuickFilterBar_1 = __decorate([
            Serenity.Decorators.registerClass('Serenity.QuickFilterBar'),
            Serenity.Decorators.element("<div/>")
        ], QuickFilterBar);
        return QuickFilterBar;
    }(Serenity.Widget));
    Serenity.QuickFilterBar = QuickFilterBar;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var QuickSearchInput = /** @class */ (function (_super) {
        __extends(QuickSearchInput, _super);
        function QuickSearchInput(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            input.attr('title', Q.text('Controls.QuickSearch.Hint'))
                .attr('placeholder', Q.text('Controls.QuickSearch.Placeholder'));
            _this.lastValue = Q.trim(Q.coalesce(input.val(), ''));
            var self = _this;
            _this.element.bind('keyup.' + _this.uniqueName, function () {
                self.checkIfValueChanged();
            });
            _this.element.bind('change.' + _this.uniqueName, function () {
                self.checkIfValueChanged();
            });
            $('<span><i></i></span>').addClass('quick-search-icon')
                .insertBefore(input);
            if (Q.isValue(_this.options.fields) && _this.options.fields.length > 0) {
                var a = $('<a/>').addClass('quick-search-field').attr('title', Q.text('Controls.QuickSearch.FieldSelection')).insertBefore(input);
                var menu = $('<ul></ul>').css('width', '120px');
                for (var _i = 0, _a = _this.options.fields; _i < _a.length; _i++) {
                    var item = _a[_i];
                    var field = { $: item };
                    $('<li><a/></li>').appendTo(menu).children().attr('href', '#')
                        .text(Q.coalesce(item.title, '')).click(function (e) {
                        e.preventDefault();
                        this.$this.fieldChanged = self.field !== this.field.$;
                        self.field = this.field.$;
                        this.$this.updateInputPlaceHolder();
                        this.$this.checkIfValueChanged();
                    }.bind({
                        field: field,
                        $this: _this
                    }));
                }
                new Serenity.PopupMenuButton(a, {
                    positionMy: 'right top',
                    positionAt: 'right bottom',
                    menu: menu
                });
                _this.field = _this.options.fields[0];
                _this.updateInputPlaceHolder();
            }
            _this.element.bind('execute-search.' + _this.uniqueName, function (e1) {
                if (!!_this.timer) {
                    window.clearTimeout(_this.timer);
                }
                _this.searchNow(Q.trim(Q.coalesce(_this.element.val(), '')));
            });
            return _this;
        }
        QuickSearchInput.prototype.checkIfValueChanged = function () {
            if (this.element.hasClass('ignore-change')) {
                return;
            }
            var value = this.get_value();
            if (value == this.lastValue && (!this.fieldChanged || Q.isEmptyOrNull(value))) {
                this.fieldChanged = false;
                return;
            }
            this.fieldChanged = false;
            if (!!this.timer) {
                window.clearTimeout(this.timer);
            }
            var self = this;
            this.timer = window.setTimeout(function () {
                self.searchNow(value);
            }, Q.coalesce(this.options.typeDelay, 500));
            this.lastValue = value;
        };
        QuickSearchInput.prototype.get_value = function () {
            return Q.trim(Q.coalesce(this.element.val(), ''));
        };
        QuickSearchInput.prototype.get_field = function () {
            return this.field;
        };
        QuickSearchInput.prototype.set_field = function (value) {
            if (this.field !== value) {
                this.fieldChanged = true;
                this.field = value;
                this.updateInputPlaceHolder();
                this.checkIfValueChanged();
            }
        };
        QuickSearchInput.prototype.updateInputPlaceHolder = function () {
            var qsf = this.element.prevAll('.quick-search-field');
            if (this.field) {
                qsf.text(this.field.title);
            }
            else {
                qsf.text('');
            }
        };
        QuickSearchInput.prototype.restoreState = function (value, field) {
            this.fieldChanged = false;
            this.field = field;
            var value = Q.trim(Q.coalesce(value, ''));
            this.element.val(value);
            this.lastValue = value;
            if (!!this.timer) {
                window.clearTimeout(this.timer);
                this.timer = null;
            }
            this.updateInputPlaceHolder();
        };
        QuickSearchInput.prototype.searchNow = function (value) {
            var _this = this;
            this.element.parent().toggleClass(Q.coalesce(this.options.filteredParentClass, 's-QuickSearchFiltered'), !!(value.length > 0));
            this.element.parent().addClass(Q.coalesce(this.options.loadingParentClass, 's-QuickSearchLoading'))
                .addClass(Q.coalesce(this.options.loadingParentClass, 's-QuickSearchLoading'));
            var done = function (results) {
                _this.element.removeClass(Q.coalesce(_this.options.loadingParentClass, 's-QuickSearchLoading')).parent().removeClass(Q.coalesce(_this.options.loadingParentClass, 's-QuickSearchLoading'));
                if (!results) {
                    _this.element.closest('.s-QuickSearchBar')
                        .find('.quick-search-icon i')
                        .effect('shake', { distance: 2 });
                }
            };
            if (this.options.onSearch != null) {
                this.options.onSearch(((this.field != null &&
                    !Q.isEmptyOrNull(this.field.name)) ? this.field.name : null), value, done);
            }
            else {
                done(true);
            }
        };
        return QuickSearchInput;
    }(Serenity.Widget));
    Serenity.QuickSearchInput = QuickSearchInput;
})(Serenity || (Serenity = {}));
//# sourceMappingURL=serenity-quickfilter.js.map