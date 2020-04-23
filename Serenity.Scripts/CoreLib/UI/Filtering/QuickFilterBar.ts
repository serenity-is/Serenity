namespace Serenity {
    export interface QuickFilterBarOptions {
        filters: QuickFilter<Widget<any>, any>[];
        getTitle?: (filter: QuickFilter<Widget<any>, any>) => string;
        idPrefix?: string;
    }

    @Serenity.Decorators.registerClass('Serenity.QuickFilterBar')
    @Serenity.Decorators.element("<div/>")
    export class QuickFilterBar extends Widget<QuickFilterBarOptions> {

        constructor(container: JQuery, options?: QuickFilterBarOptions) {
            super(container, options);

            container.addClass('quick-filters-bar').addClass('clear');

            var filters = this.options.filters;
            for (var f = 0; f < filters.length; f++) {
                var filter = filters[f];
                this.add(filter);
            }

            this.options.idPrefix = Q.coalesce(this.options.idPrefix, this.uniqueName + '_');
        }

        public addSeparator(): void {
            this.element.append($('<hr/>'));
        }

        public add<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget {

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
                element: e => {
                    if (!Q.isEmptyOrNull(opt.field)) {
                        e.attr('id', this.options.idPrefix + opt.field);
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

            var submitHandler = (request: Serenity.ListRequest) => {

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

            Serenity.WX.changeSelect2(widget, (e1: JQueryEventObject) => {
                // use timeout give cascaded dropdowns a chance to update / clear themselves
                window.setTimeout(() => this.onChange && this.onChange(e1), 0);
            });

            this.add_submitHandlers(submitHandler);
            widget.element.bind('remove.' + this.uniqueName, x => {
                this.remove_submitHandlers(submitHandler);
            });

            return widget;
        }

        public addDateRange(field: string, title?: string): Serenity.DateEditor {
            return this.add(QuickFilterBar.dateRange(field, title)) as Serenity.DateEditor;
        }

        public static dateRange(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions> {
            var end: Serenity.DateEditor = null;
            return <QuickFilter<DateEditor, DateTimeEditorOptions>>{
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
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and',
                            [[args.field], '>=', args.widget.value]);
                    }
                    if (active2) {
                        var next = new Date(end.valueAsDate.valueOf());
                        next.setDate(next.getDate() + 1);
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and',
                            [[args.field], '<', Q.formatDate(next, 'yyyy-MM-dd')]);
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
        }

        public addDateTimeRange(field: string, title?: string) {
            return this.add(QuickFilterBar.dateTimeRange(field, title)) as Serenity.DateTimeEditor;
        }

        public static dateTimeRange(field: string, title?: string): QuickFilter<DateTimeEditor, DateTimeEditorOptions> {
            var end: Serenity.DateTimeEditor = null;

            return <QuickFilter<DateTimeEditor, DateTimeEditorOptions>>{
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
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and',
                            [[args.field], '>=', args.widget.value]);
                    }
                    if (active2) {
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and',
                            [[args.field], '<=', end.value]);
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
        }

        public addBoolean(field: string, title?: string, yes?: string, no?: string): SelectEditor {
            return this.add(QuickFilterBar.boolean(field, title, yes, no));
        }

        public static boolean(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions> {
            var opt: SelectEditorOptions = {};
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
        }

        public static propertyItemToQuickFilter(item: PropertyItem) {
            var quick: any = {};

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
                quick = QuickFilterBar.dateRange(name, title);
            }
            else if (filteringType === Serenity.DateTimeFiltering) {
                quick = QuickFilterBar.dateTimeRange(name, title);
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
                quick = QuickFilterBar.boolean(name, title, trueText, falseText);
            }
            else {
                var filtering = new (filteringType as any)() as IFiltering;
                if (filtering && Q.isInstanceOfType(filtering, Serenity.IQuickFiltering)) {
                    Serenity.ReflectionOptionsSetter.set(filtering, item.filteringParams);
                    filtering.set_field(item);
                    filtering.set_operator({ key: Serenity.FilterOperators.EQ });
                    (filtering as any).initQuickFilter(quick);
                    quick.options = Q.deepClone(quick.options, item.quickFilterParams);
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
        }

        public onChange: (e: JQueryEventObject) => void;

        private submitHandlers: any;

        destroy() {
            this.submitHandlers = null;
            super.destroy();
        }

        public onSubmit(request: Serenity.ListRequest) {
            this.submitHandlers && this.submitHandlers(request);
        }

        protected add_submitHandlers(action: (request: Serenity.ListRequest) => void): void {
            this.submitHandlers = Q.delegateCombine(this.submitHandlers, action);
        }

        protected remove_submitHandlers(action: (request: Serenity.ListRequest) => void): void {
            this.submitHandlers = Q.delegateRemove(this.submitHandlers, action);
        }

        protected clear_submitHandlers() {
        }

        public find<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
            return $('#' + this.options.idPrefix + field).getWidget(type);
        }

        public tryFind<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
            var el = $('#' + this.options.idPrefix + field);
            if (!el.length)
                return null;

            return el.tryGetWidget(type);
        }
    }
}
