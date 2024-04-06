import { Criteria, Fluent, ListRequest, formatDate, localText, notifyWarning, parseDate, toId, tryGetText } from "../../base";
import { ArgumentNullException } from "../../q";
import { Decorators } from "../../types/decorators";
import { DateEditor } from "../editors/dateeditor";
import { DateTimeEditor, DateTimeEditorOptions } from "../editors/datetimeeditor";
import { EditorUtils } from "../editors/editorutils";
import { SelectEditor, SelectEditorOptions } from "../editors/selecteditor";
import { delegateCombine, delegateRemove } from "../filtering/filterstore";
import { Widget, WidgetProps } from "../widgets/widget";
import { getWidgetFrom, tryGetWidget } from "../widgets/widgetutils";
import { QuickFilter } from "./quickfilter";

export interface QuickFilterBarOptions {
    filters: QuickFilter<Widget<any>, any>[];
    getTitle?: (filter: QuickFilter<Widget<any>, any>) => string;
    idPrefix?: string;
}

@Decorators.registerClass('Serenity.QuickFilterBar')
export class QuickFilterBar<P extends QuickFilterBarOptions = QuickFilterBarOptions> extends Widget<P> {

    constructor(props: WidgetProps<P>) {
        super(props);

        this.domNode.classList.add('quick-filters-bar', 'clear');

        var filters = this.options.filters;
        for (var f = 0; f < filters.length; f++) {
            var filter = filters[f];
            this.add(filter);
        }

        this.options.idPrefix = (this.options.idPrefix ?? this.uniqueName + '_');
    }

    public addSeparator(): void {
        Fluent("hr").appendTo(this.domNode);
    }

    public add<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget {

        if (opt == null) {
            throw new ArgumentNullException('opt');
        }

        if (opt.separator) {
            this.addSeparator();
        }

        var quickFilter = Fluent("div").class("quick-filter-item")
            .appendTo(this.domNode)
            .data('qffield', opt.field)

        var title = tryGetText(opt.title) ?? opt.title;
        if (title == null) {
            title = this.options.getTitle ? this.options.getTitle(opt) : null;
            if (title == null) {
                title = opt.field;
            }
        }

        Fluent("span").class("quick-filter-label").text(title).appendTo(quickFilter);

        var qfElement = quickFilter.getNode() as any;

        if (opt.displayText != null) {
            qfElement.qfdisplaytext = opt.displayText;
        }

        if (opt.saveState != null) {
            qfElement.qfsavestate = opt.saveState;
        }

        if (opt.loadState != null) {
            qfElement.qfloadstate = opt.loadState;
        }

        if (opt.cssClass) {
            quickFilter.addClass(opt.cssClass);
        }

        var widget = Widget.create({
            type: opt.type,
            options: {
                element: el => {
                    if (opt.field)
                        el.setAttribute('id', this.options.idPrefix + opt.field);
                    el.setAttribute('placeholder', ' ');
                    quickFilter.append(el);
                    if (opt.element != null) {
                        opt.element(Fluent(el));
                    }
                },
                ...opt.options
            }
        });
        opt.init?.(widget);

        var submitHandler = (request: ListRequest) => {

            if (quickFilter.hasClass('ignore')) {
                return;
            }

            request.EqualityFilter = request.EqualityFilter || {};
            var value = EditorUtils.getValue(widget);
            var active = !!value?.toString();
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

        widget.changeSelect2(e1 => {
            // use timeout give cascaded dropdowns a chance to update / clear themselves
            window.setTimeout(() => this.onChange && this.onChange(e1), 0);
        });

        this.add_submitHandlers(submitHandler);
        Fluent.on(widget.domNode, 'remove.' + this.uniqueName, x => {
            this.remove_submitHandlers(submitHandler);
        });

        return widget;
    }

    public addDateRange(field: string, title?: string): DateEditor {
        return this.add(QuickFilterBar.dateRange(field, title)) as DateEditor;
    }

    public static dateRange(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions> {
        var end: DateEditor = null;
        return <QuickFilter<DateEditor, DateTimeEditorOptions>>{
            field: field,
            type: DateEditor,
            title: title,
            element: function (el) {
                end = new DateEditor({ element: el2 => Fluent(el2).insertAfter(el) });
                Fluent.on(end.domNode, "change", () => el.trigger("change"));
                Fluent("span").class('range-separator').text('-').insertAfter(el);
            },
            handler: function (args) {
                var date1 = parseDate(args.widget.value);
                if (date1) {
                    if (isNaN(date1.valueOf())) {
                        notifyWarning(localText('Validation.DateInvalid'), '', null);
                        args.widget.domNode.value = "";
                        date1 = null;
                    }
                    else {
                        args.request.Criteria = Criteria.and(args.request.Criteria,
                            Criteria(args.field).ge(args.widget.value));
                    }
                }

                var date2 = parseDate(end.value);
                if (date2) {
                    if (isNaN(date2?.valueOf())) {
                        notifyWarning(localText('Validation.DateInvalid'), '', null);
                        end.domNode.value = "";
                        date2 = null;
                    }
                    else {
                        var next = new Date(end.valueAsDate.valueOf());
                        next.setDate(next.getDate() + 1);
                        args.request.Criteria = Criteria.and(args.request.Criteria,
                            Criteria(args.field).lt(formatDate(next, 'yyyy-MM-dd')));
                    }
                }

                args.active = !!(date1 || date2);
            },
            displayText: function (w, l) {
                var v1 = EditorUtils.getDisplayText(w);
                var v2 = EditorUtils.getDisplayText(end);
                if (!v1 && !v2)
                    return null;
                var text1 = l + ' >= ' + v1;
                var text2 = l + ' <= ' + v2;
                if (v1 && v2) {
                    return text1 + ' ' + (tryGetText('Controls.FilterPanel.And') ?? 'and') + ' ' + text2;
                }
                else if (v1) {
                    return text1;
                }
                else {
                    return text2;
                }
            },
            saveState: function (w1) {
                return [EditorUtils.getValue(w1), EditorUtils.getValue(end)];
            },
            loadState: function (w2, state) {
                if (state == null || !Array.isArray(state) || state.length !== 2) {
                    state = [null, null];
                }

                EditorUtils.setValue(w2, state[0]);
                EditorUtils.setValue(end, state[1]);
            }
        };
    }

    public addDateTimeRange(field: string, title?: string) {
        return this.add(QuickFilterBar.dateTimeRange(field, title)) as DateTimeEditor;
    }

    public static dateTimeRange(field: string, title?: string, useUtc?: boolean): QuickFilter<DateTimeEditor, DateTimeEditorOptions> {
        var end: DateTimeEditor = null;

        return <QuickFilter<DateTimeEditor, DateTimeEditorOptions>>{
            field: field,
            type: DateTimeEditor,
            title: title,
            element: function (el) {
                end = new DateTimeEditor({
                    element: el2 => Fluent(el2).insertAfter(el),
                    useUtc: useUtc == null ? undefined : useUtc,
                });
                Fluent.on(end.domNode, ".change", () => el.trigger("change"));
                Fluent("span").class('range-separator').text('-').insertAfter(el);
            },
            init: function (w) {
                Fluent.on(w.domNode.parentElement?.querySelector('.time'), "change", () => Fluent.trigger(w.domNode, "change"));
            },
            handler: function (args) {
                var date1 = parseDate(args.widget.value);
                if (date1) {
                    if (isNaN(date1?.valueOf())) {
                        notifyWarning(localText('Validation.DateInvalid'), '', null);
                        args.widget.value = "";
                        date1 = null;
                    }
                    else {
                        args.request.Criteria = Criteria.and(args.request.Criteria,
                            Criteria(args.field).ge(args.widget.value));
                    }
                }

                var date2 = parseDate(end.value);
                if (date2) {
                    if (isNaN(date2?.valueOf())) {
                        notifyWarning(localText('Validation.DateInvalid'), '', null);
                        end.value = "";
                        date2 = null;
                    }
                    else {
                        args.request.Criteria = Criteria.and(args.request.Criteria,
                            Criteria(args.field).le(end.value));
                    }
                }

                args.active = !!(date1 || date2);
            },
            displayText: function (w, l) {
                var v1 = EditorUtils.getDisplayText(w);
                var v2 = EditorUtils.getDisplayText(end);
                if (!v1 && !v2) {
                    return null;
                }
                var text1 = l + ' >= ' + v1;
                var text2 = l + ' <= ' + v2;
                if (v1 && v2) {
                    return text1 + ' ' + (tryGetText('Controls.FilterPanel.And') ?? 'and') + ' ' + text2;
                }
                else if (v1) {
                    return text1;
                }
                else {
                    return text2;
                }
            },
            saveState: function (w1) {
                return [EditorUtils.getValue(w1), EditorUtils.getValue(end)];
            },
            loadState: function (w2, state) {
                if (state == null || !Array.isArray(state) || state.length !== 2) {
                    state = [null, null];
                }
                EditorUtils.setValue(w2, state[0]);
                EditorUtils.setValue(end, state[1]);
            },
            options: useUtc == null ? null : { useUtc }
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
            trueText = localText('Controls.FilterPanel.OperatorNames.true');
        }
        items.push(['1', trueText]);

        var falseText = no;
        if (falseText == null) {
            falseText = localText('Controls.FilterPanel.OperatorNames.false');
        }

        items.push(['0', falseText]);

        opt.items = items;

        return {
            field: field,
            type: SelectEditor,
            title: title,
            options: opt,
            handler: function (args) {
                args.equalityFilter[args.field] = !args.value?.toString() ?
                    null : !!toId(args.value);
            }
        };
    }

    public onChange: (e: Event) => void;

    private submitHandlers: any;

    destroy() {
        this.submitHandlers = null;
        super.destroy();
    }

    public onSubmit(request: ListRequest) {
        this.submitHandlers && this.submitHandlers(request);
    }

    protected add_submitHandlers(action: (request: ListRequest) => void): void {
        this.submitHandlers = delegateCombine(this.submitHandlers, action);
    }

    protected remove_submitHandlers(action: (request: ListRequest) => void): void {
        this.submitHandlers = delegateRemove(this.submitHandlers, action);
    }

    protected clear_submitHandlers() {
    }

    public find<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
        return getWidgetFrom('#' + this.options.idPrefix + field, type);
    }

    public tryFind<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
        return tryGetWidget('#' + this.options.idPrefix + field, type);
    }
}