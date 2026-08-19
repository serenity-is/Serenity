import { addDisposingListener } from "@serenity-is/domwise";
import { Criteria, FilterPanelTexts, Fluent, FormValidationTexts, ListRequest, formatDate, notifyWarning, nsSerenity, parseDate, toId, tryGetText } from "../../base";
import { DateEditor } from "../editors/dateeditor";
import { DateTimeEditor, DateTimeEditorOptions } from "../editors/datetimeeditor";
import { EditorUtils } from "../editors/editorutils";
import { SelectEditor, SelectEditorOptions } from "../editors/selecteditor";
import { Widget, WidgetProps } from "../widgets/widget";
import { getWidgetFrom, tryGetWidget } from "../widgets/widgetutils";
import { QuickFilter } from "./quickfilter";

/**
 * Options for the {@link QuickFilterBar} widget.
 */
export interface QuickFilterBarOptions {
    /** Quick filter definitions to render in the bar. */
    filters: QuickFilter<Widget<any>, any>[];
    /** Optional callback that returns the display title for a filter. */
    getTitle?: (filter: QuickFilter<Widget<any>, any>) => string;
    /** Prefix used for generated element ids; defaults to the widget unique name. */
    idPrefix?: string;
}

/**
 * Per-item data attached to a quick filter element for state persistence and display.
 * @typeParam TWidget - Widget type that backs the quick filter.
 */
export interface QuickFilterItemData<TWidget> {
    /** Returns the human-readable text for the active filter display. */
    displayText?: (w: TWidget, l: string) => string;
    /** Persists the widget state for grid settings. */
    saveState?: (w: TWidget) => any;
    /** Restores persisted widget state. */
    loadState?: (w: TWidget, state: any) => void;
}

/**
 * A bar that renders quick filters for a grid, including date ranges, boolean
 * toggles, and custom filter widgets, and submits their values with list requests.
 * @typeParam P - Options type for the widget.
 */
export class QuickFilterBar<P extends QuickFilterBarOptions = QuickFilterBarOptions> extends Widget<P> {

    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates a quick filter bar and adds all configured filters.
     * @param props - Widget props including the filter definitions.
     */
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

    private static readonly itemDataMap = new WeakMap<Node, QuickFilterItemData<any>>();

    /**
     * Returns the per-item data attached to a quick filter element, if any.
     * @param filterItem - The quick filter container element.
     * @returns The item data, or undefined if none was attached.
     */
    static getItemData<TWidget>(filterItem: Node): QuickFilterItemData<TWidget> | undefined {
        return this.itemDataMap.get(filterItem);
    }

    /**
     * Adds a visual separator to the bar.
     */
    public addSeparator(): void {
        this.domNode.append(<hr />)
    }

    /**
     * Adds a quick filter widget to the bar and wires its submit handler.
     * @param opt - Quick filter definition.
     * @returns The created widget instance.
     */
    public add<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget {

        if (opt == null) 
            throw new Error("QuickFilterBar.add(): 'opt' argument is null!");

        if (opt.separator) {
            this.addSeparator();
        }

        const qfElement = this.domNode.appendChild(<div class="quick-filter-item" data-qffield={opt.field}></div>) as HTMLDivElement;

        var title = tryGetText(opt.title) ?? opt.title;
        if (title == null) {
            title = this.options.getTitle ? this.options.getTitle(opt) : null;
            if (title == null) {
                title = opt.field;
            }
        }

        qfElement.appendChild(<span class="quick-filter-label">{title}</span>);

        const qfData = {} as QuickFilterItemData<TWidget>;

        if (opt.displayText != null) {
            qfData.displayText = opt.displayText;
        }

        if (opt.saveState != null) {
            qfData.saveState = opt.saveState;
        }

        if (opt.loadState != null) {
            qfData.loadState = opt.loadState;
        }

        if (qfData.displayText || qfData.saveState || qfData.loadState) {
            QuickFilterBar.itemDataMap.set(qfElement, qfData);
        }

        if (opt.cssClass) {
            Fluent.addClass(qfElement, opt.cssClass);
        }

        var widget = Widget.create({
            type: opt.type,
            options: {
                element: el => {
                    if (opt.field)
                        el.setAttribute('id', this.options.idPrefix + opt.field);
                    el.setAttribute('placeholder', ' ');
                    qfElement.append(el);
                    if (opt.element != null) {
                        opt.element(Fluent(el));
                    }
                },
                ...opt.options
            }
        });
        opt.init?.(widget);

        var submitHandler = (request: ListRequest) => {

            if (qfElement.classList.contains('ignore')) {
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
                qfElement.classList.toggle('quick-filter-active', !!args.active);
                if (!args.handled) {
                    request.EqualityFilter[opt.field] = value;
                }
            }
            else {
                request.EqualityFilter[opt.field] = value;
                qfElement.classList.toggle('quick-filter-active', !!active);
            }
        };

        widget.changeSelect2(e1 => {
            // use timeout give cascaded dropdowns a chance to update / clear themselves
            window.setTimeout(() => this.onChange && this.onChange(e1), 0);
        });

        this.submitHandlers.push(submitHandler);
        addDisposingListener(widget.domNode, () => this.submitHandlers = this.submitHandlers?.filter(h => h !== submitHandler));

        return widget;
    }

    /**
     * Adds a date range quick filter for the specified field.
     * @param field - Field name the filter is bound to.
     * @param title - Optional display title.
     * @returns The created date editor.
     */
    public addDateRange(field: string, title?: string): DateEditor {
        return this.add(QuickFilterBar.dateRange(field, title)) as DateEditor;
    }

    /**
     * Creates a date range quick filter definition for the specified field.
     * @param field - Field name the filter is bound to.
     * @param title - Optional display title.
     * @returns A quick filter definition for a date range.
     */
    public static dateRange(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions> {
        var end: DateEditor = null;
        return {
            field: field,
            type: DateEditor,
            title: title,
            element: function (el) {
                end = new DateEditor({ element: el2 => Fluent(el2).insertAfter(el) });
                Fluent.on(end.domNode, "change." + end.uniqueName, () => el.trigger("change"));
                el.after(<span class="range-separator">-</span>);
            },
            handler: function (args) {
                var date1 = parseDate(args.widget.value);
                if (date1) {
                    if (isNaN(date1.valueOf())) {
                        notifyWarning(FormValidationTexts.DateInvalid, '', null);
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
                        notifyWarning(FormValidationTexts.DateInvalid, '', null);
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
                    return text1 + ' ' + (FilterPanelTexts.asTry().And ?? 'and') + ' ' + text2;
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

    /**
     * Adds a date-time range quick filter for the specified field.
     * @param field - Field name the filter is bound to.
     * @param title - Optional display title.
     * @returns The created date-time editor.
     */
    public addDateTimeRange(field: string, title?: string) {
        return this.add(QuickFilterBar.dateTimeRange(field, title)) as DateTimeEditor;
    }

    /**
     * Creates a date-time range quick filter definition for the specified field.
     * @param field - Field name the filter is bound to.
     * @param title - Optional display title.
     * @param useUtc - Whether the editor should use UTC values.
     * @returns A quick filter definition for a date-time range.
     */
    public static dateTimeRange(field: string, title?: string, useUtc?: boolean): QuickFilter<DateTimeEditor, DateTimeEditorOptions> {
        var end: DateTimeEditor = null;

        return {
            field: field,
            type: DateTimeEditor,
            title: title,
            element: function (el) {
                end = new DateTimeEditor({
                    element: el2 => Fluent(el2).insertAfter(el),
                    useUtc: useUtc == null ? undefined : useUtc,
                });
                Fluent.on(end.domNode, "change." + end.uniqueName, () => el.trigger("change"));
                el.after(<span class="range-separator">-</span>);
            },
            init: function (w) {
                Fluent.on(w.domNode.parentElement?.querySelector('.time'), "change", () => Fluent.trigger(w.domNode, "change"));
            },
            handler: function (args) {
                var date1 = parseDate(args.widget.value);
                if (date1) {
                    if (isNaN(date1?.valueOf())) {
                        notifyWarning(FormValidationTexts.DateInvalid, '', null);
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
                        notifyWarning(FormValidationTexts.DateInvalid, '', null);
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
                    return text1 + ' ' + (FilterPanelTexts.asTry().And ?? 'and') + ' ' + text2;
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

    /**
     * Adds a boolean quick filter for the specified field.
     * @param field - Field name the filter is bound to.
     * @param title - Optional display title.
     * @param yes - Optional text for the true option.
     * @param no - Optional text for the false option.
     * @returns The created select editor.
     */
    public addBoolean(field: string, title?: string, yes?: string, no?: string): SelectEditor {
        return this.add(QuickFilterBar.boolean(field, title, yes, no));
    }

    /**
     * Creates a boolean quick filter definition for the specified field.
     * @param field - Field name the filter is bound to.
     * @param title - Optional display title.
     * @param yes - Optional text for the true option.
     * @param no - Optional text for the false option.
     * @returns A quick filter definition for a boolean value.
     */
    public static boolean(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions> {
        var opt: SelectEditorOptions = {};
        var items = [];

        var trueText = yes;
        if (trueText == null) {
            trueText = (FilterPanelTexts.OperatorNames as any).true;
        }
        items.push(['1', trueText]);

        var falseText = no;
        if (falseText == null) {
            falseText = (FilterPanelTexts.OperatorNames as any).false;
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

    /** Callback invoked when a quick filter value changes. */
    declare public onChange: (e: Event) => void;

    private submitHandlers = [] as ((request: ListRequest) => void)[];

    /**
     * Cleans up submit handlers and delegates to the base destroy.
     */
    override destroy() {
        this.submitHandlers = null;
        super.destroy();
    }

    /**
     * Invokes all registered submit handlers with the given list request.
     * @param request - The list request being prepared.
     */
    public onSubmit(request: ListRequest) {
        this.submitHandlers?.forEach(handler => handler(request));
    }

    /**
     * Finds the widget instance for a quick filter by field name.
     * @param type - Widget constructor type.
     * @param field - Field name of the quick filter.
     * @returns The widget instance.
     */
    public find<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
        const selector = '#' + this.options.idPrefix + field;

        return getWidgetFrom(this.domNode?.querySelector(selector) ?? selector, type);
    }

    /**
     * Tries to find the widget instance for a quick filter by field name.
     * @param type - Widget constructor type.
     * @param field - Field name of the quick filter.
     * @returns The widget instance, or null if not found.
     */
    public tryFind<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
        const selector = '#' + this.options.idPrefix + field;
        return tryGetWidget(this.domNode?.querySelector(selector) ?? selector, type);
    }
}