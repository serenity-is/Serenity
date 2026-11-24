import { addDisposingListener } from "@serenity-is/domwise";
import { Criteria, FilterPanelTexts, Fluent, FormValidationTexts, ListRequest, QuickFilterBarTexts, formatDate, isPromiseLike, notifyWarning, nsSerenity, parseDate, toId, tryGetText } from "../../base";
import { ComboboxEditor } from "../editors/comboboxeditor";
import { DateEditor, DateEditorOptions } from "../editors/dateeditor";
import { DateTimeEditor, DateTimeEditorOptions } from "../editors/datetimeeditor";
import { EditorUtils } from "../editors/editorutils";
import { SelectEditor, SelectEditorOptions } from "../editors/selecteditor";
import { Widget, WidgetProps } from "../widgets/widget";
import { getWidgetFrom, tryGetWidget } from "../widgets/widgetutils";
import { QuickFilter } from "./quickfilter";
import { QuickFilterPlaceholder } from "./quickfilterplaceholder";

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
    /**
     * Whether the bar has a button at its end that clears all its filters, which is shown while any of them is applied.
     * Default is {@link QuickFilterBarDefaults.clearButton}.
     */
    clearButton?: boolean;
    /**
     * Whether the filters are collapsed behind a "Filters" button while the bar is narrower than
     * {@link QuickFilterBarDefaults.collapseWidth}, e.g. on a phone. The button shows how many filters are applied,
     * and opens the filters, one in each row. The filters that are pinned, see {@link QuickFilter.pinned}, stay visible.
     * Default is {@link QuickFilterBarDefaults.collapsible}.
     */
    collapsible?: boolean;
}

/** What {@link QuickFilterBarDefaults.filterAdded} is called with, for a filter that is added to a bar. */
export interface QuickFilterAddedArgs {
    /** The bar. */
    bar: QuickFilterBar;
    /** The definition of the filter. */
    filter: QuickFilter<Widget<any>, any>;
    /** The element of the filter, which holds its label and its widget. */
    element: HTMLElement;
    /** The label of the filter. */
    label: HTMLElement;
    /** The widget of the filter, which is created by then, also when its type was loaded later. */
    widget: Widget<any>;
}

/** The defaults of the quick filter bars, which an application can set once, see {@link QuickFilterBar.defaults}. */
export interface QuickFilterBarDefaults {
    /** The default of {@link QuickFilterBarOptions.clearButton}. Default is true. */
    clearButton: boolean;
    /** The default of {@link QuickFilterBarOptions.collapsible}. Default is true. */
    collapsible: boolean;
    /**
     * The width, in pixels, that a bar is collapsed below, see {@link QuickFilterBarOptions.collapsible}. Default is 768,
     * the width below which the filters with the hidden-xs class were hidden, which are collapsed instead while it is.
     */
    collapseWidth: number;
    /**
     * Creates the quick filter of a date field that a grid makes from its columns, instead of
     * {@link QuickFilterBar.dateRange}, which has two date editors, e.g. one that edits the range in one input along with
     * the ranges that follow the current date, like the date range editor of Pro.Extensions. Not set by default.
     * The third argument is the quick filter params of the property item of the field. When `range` is false in them, e.g.
     * for a column that has `[QuickFilter(Range = false)]`, the filter should edit a single date, which matches an exact
     * value, instead of a range, see {@link QuickFilterBar.dateEqual}.
     */
    dateRangeFilter?: (field: string, title?: string, quickFilterParams?: any) => QuickFilter<Widget<any>, any>;
    /**
     * Called when a bar is created, before its filters are added, e.g. to give it a class that changes how its filters
     * look. Not set by default.
     */
    created?: (bar: QuickFilterBar) => void;
    /**
     * Called when a filter is added to a bar, once its widget is created, which is when its type is loaded for a type
     * that is loaded later, e.g. to change how the filter looks. Not set by default.
     */
    filterAdded?: (args: QuickFilterAddedArgs) => void;
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

    /** The defaults of the quick filter bars, see {@link QuickFilterBarDefaults}. */
    public static readonly defaults: QuickFilterBarDefaults = {
        clearButton: true,
        collapsible: true,
        collapseWidth: 768
    };

    /**
     * Creates a quick filter bar and adds all configured filters.
     * @param props - Widget props including the filter definitions.
     */
    constructor(props: WidgetProps<P>) {
        super(props);

        this.domNode.classList.add('quick-filters-bar', 'clear');
        QuickFilterBar.defaults.created?.(this);

        // before the filters are added, as the ids of their elements start with it
        this.options.idPrefix = (this.options.idPrefix ?? this.uniqueName + '_');

        const filters = this.options.filters;
        for (let f = 0; f < filters.length; f++) {
            const filter = filters[f];
            this.add(filter);
        }

        this.observeWidth();
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
        this.domNode.insertBefore(<hr />, this.getInsertionPoint());
    }

    /** Where a filter, or a separator, is added: after the last one, which the clear button is after unless collapsed. */
    private getInsertionPoint(): Node | null {
        const added = this.domNode.querySelectorAll(":scope > .quick-filter-item, :scope > hr");
        const last = added[added.length - 1];
        if (last)
            return last.nextSibling;
        return this.clearButton?.parentNode === this.domNode ? this.clearButton : null;
    }

    /** The filters of the bar, with their elements and widgets, which the clear button clears, see {@link clear}. */
    private readonly items: { element: HTMLElement, widget: () => Widget<any> }[] = [];

    /** The button that clears all the filters, see {@link QuickFilterBarOptions.clearButton}. */
    declare private clearButton: HTMLButtonElement;

    /** Whether the filters are being cleared, when their changes are told as one, see {@link clear}. */
    declare private clearing: boolean;

    /**
     * Clears all the filters of the bar, as its clear button does, and tells the change once, e.g. to the grid, which
     * reloads its rows, instead of once for each filter. A filter that saves its state, e.g. a date range, is cleared by
     * loading an empty state, and any other one by an empty value.
     */
    public clear(): void {
        this.clearing = true;
        try {
            for (const item of this.items) {
                const widget = item.widget();
                if (!widget?.domNode)
                    continue;

                const data = QuickFilterBar.getItemData<Widget<any>>(item.element);
                if (data?.loadState)
                    data.loadState(widget, null);
                else
                    EditorUtils.setValue(widget, null);
            }
        }
        finally {
            // after the changes of the filters, which are told with a timeout, so that they are skipped
            window.setTimeout(() => {
                this.clearing = false;
                if (this.domNode)
                    this.onChange?.(new Event("change"));
            }, 0);
        }
    }

    /**
     * Shows the clear button while any filter is applied, and keeps it after the last filter.
     */
    protected updateClearButton(): void {
        if (!(this.options.clearButton ?? QuickFilterBar.defaults.clearButton ?? true) || !this.domNode)
            return;

        if (!this.clearButton) {
            this.clearButton = <button type="button" class="quick-filters-clear" hidden title={QuickFilterBarTexts.ClearFiltersHint}
                onClick={() => this.clear()}>{QuickFilterBarTexts.ClearFilters}</button> as HTMLButtonElement;
        }

        // beside the "Filters" button while the filters are collapsed behind it, and otherwise after the last filter,
        // and before what else is in the bar, e.g. the view switch of a grid
        const items = this.domNode.querySelectorAll(":scope > .quick-filter-item");
        const anchor = this.narrow && this.toggleButton && !this.toggleButton.hidden ? this.toggleButton : items[items.length - 1];
        if (!anchor)
            this.clearButton.parentNode !== this.domNode && this.domNode.append(this.clearButton);
        else if (anchor.nextElementSibling !== this.clearButton)
            anchor.after(this.clearButton);

        this.clearButton.hidden = !this.domNode.querySelector(":scope > .quick-filter-active");
    }

    /** The button that opens and closes the filters while the bar is collapsed, see {@link QuickFilterBarOptions.collapsible}. */
    declare private toggleButton: HTMLButtonElement;

    /** Whether the bar is narrower than {@link QuickFilterBarDefaults.collapseWidth}, when its filters are collapsed. */
    declare private narrow: boolean;

    /** Whether the filters are opened while the bar is collapsed. */
    declare private expanded: boolean;

    declare private resizeObserver: ResizeObserver;

    private isCollapsible(): boolean {
        return !!(this.options.collapsible ?? QuickFilterBar.defaults.collapsible ?? true);
    }

    /** Collapses the filters while the bar is narrow, which it watches, where it can. */
    private observeWidth(): void {
        if (!this.isCollapsible() || typeof ResizeObserver === "undefined")
            return;

        let frame: number;
        this.resizeObserver = new ResizeObserver(() => {
            // in the next frame, as collapsing changes the layout, which the browser would report as a loop of the
            // observer if it was changed while it notifies
            cancelAnimationFrame(frame);
            frame = requestAnimationFrame(() => {
                const width = this.domNode?.clientWidth;
                // a bar that is not shown, e.g. in a tab that is not open, keeps how it is
                if (width)
                    this.setNarrow(width < (QuickFilterBar.defaults.collapseWidth ?? 768));
            });
        });
        this.resizeObserver.observe(this.domNode);
    }

    /**
     * Sets whether the bar is narrow, when its filters, except the pinned ones, are collapsed behind its "Filters"
     * button, which it does by itself as its width changes, see {@link QuickFilterBarOptions.collapsible}.
     * @param narrow - Whether the bar is narrow.
     */
    public setNarrow(narrow: boolean): void {
        narrow = !!narrow && this.isCollapsible();
        if (!this.domNode || narrow === !!this.narrow)
            return;

        this.narrow = narrow;
        this.domNode.classList.toggle("quick-filters-narrow", narrow);
        // the filters that were hidden on a small screen are collapsed instead, so that they can be opened
        this.domNode.querySelectorAll(narrow ? ":scope > .quick-filter-item.hidden-xs" : ":scope > .quick-filter-item.quick-filter-hidden-xs")
            .forEach(el => {
                el.classList.toggle("hidden-xs", !narrow);
                el.classList.toggle("quick-filter-hidden-xs", narrow);
            });
        this.updateToggleButton();
        // the clear button moves beside the toggle button, or back after the filters
        if (this.clearButton)
            this.updateClearButton();
    }

    /**
     * Opens or closes the filters while the bar is collapsed, as its "Filters" button does.
     * @param expanded - Whether the filters are opened.
     */
    public setExpanded(expanded: boolean): void {
        this.expanded = !!expanded;
        this.domNode?.classList.toggle("quick-filters-expanded", this.expanded);
        this.toggleButton?.setAttribute("aria-expanded", this.expanded ? "true" : "false");
    }

    /**
     * Shows the "Filters" button while the bar is narrow and it has a filter to collapse, with how many of those
     * filters are applied, and keeps it at the start of the bar.
     */
    protected updateToggleButton(): void {
        if (!this.domNode || (!this.narrow && !this.toggleButton))
            return;

        if (!this.toggleButton) {
            this.toggleButton = <button type="button" class="quick-filters-toggle" aria-expanded="false" onClick={() => this.setExpanded(!this.expanded)}>
                <span class="quick-filters-toggle-text">{QuickFilterBarTexts.Filters}</span>
                <span class="quick-filters-count" hidden></span>
            </button> as HTMLButtonElement;
            this.setExpanded(this.expanded);
        }

        if (this.domNode.firstElementChild !== this.toggleButton)
            this.domNode.prepend(this.toggleButton);

        const collapsed = this.domNode.querySelectorAll(":scope > .quick-filter-item:not(.quick-filter-pinned)");
        this.toggleButton.hidden = !this.narrow || !collapsed.length;

        const active = Array.from(collapsed).filter(el => el.classList.contains("quick-filter-active")).length;
        const count = this.toggleButton.querySelector<HTMLElement>(".quick-filters-count");
        count.textContent = active ? String(active) : "";
        count.hidden = !active;
    }

    /**
     * Adds a quick filter widget to the bar and wires its submit handler.
     * @param opt - Quick filter definition.
     * @returns The created widget instance, or a placeholder if the type of the widget has to be loaded.
     */
    public add<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget {

        if (opt == null)
            throw new Error("QuickFilterBar.add(): 'opt' argument is null!");

        if (opt.separator) {
            this.addSeparator();
        }

        const qfElement = this.domNode.insertBefore(<div class="quick-filter-item" data-qffield={opt.field}></div>,
            this.getInsertionPoint()) as HTMLDivElement;

        let title = tryGetText(opt.title) ?? opt.title;
        if (title == null) {
            title = this.options.getTitle ? this.options.getTitle(opt) : null;
            if (title == null) {
                title = opt.field;
            }
        }

        const qfLabel = qfElement.appendChild(<span class="quick-filter-label">{title}</span>) as HTMLElement;

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

        if (opt.pinned) {
            qfElement.classList.add("quick-filter-pinned");
        }

        if (this.narrow && qfElement.classList.contains("hidden-xs")) {
            qfElement.classList.remove("hidden-xs");
            qfElement.classList.add("quick-filter-hidden-xs");
        }

        const createWidget = (type: { new(options?: TOptions): TWidget, prototype: TWidget }, place: (el: HTMLElement) => void) => Widget.create({
            type,
            options: {
                element: el => {
                    if (opt.field)
                        el.setAttribute('id', this.options.idPrefix + opt.field);
                    el.setAttribute('placeholder', ' ');
                    place(el);
                    if (opt.element != null) {
                        opt.element(Fluent(el));
                    }
                },
                // the dropdown of a select fits its results, which can be longer than the filter is wide, unless it is
                // told otherwise
                ...((type.prototype as any) instanceof ComboboxEditor ? { dropdownAutoWidth: true } : null),
                ...opt.options
            }
        });

        let widget: TWidget;

        const removeHandlerOnDisposing = (w: Widget<any>) => addDisposingListener(w.domNode, () => {
            // a placeholder is disposed when it is replaced, but the handler continues with the actual widget
            if (w === (widget as Widget<any>))
                this.submitHandlers = this.submitHandlers?.filter(h => h !== submitHandler);
        });

        const activate = (w: TWidget) => {
            opt.init?.(w);
            QuickFilterBar.defaults.filterAdded?.({ bar: this, filter: opt, element: qfElement, label: qfLabel, widget: w });
            w.changeSelect2(e1 => {
                // use timeout give cascaded dropdowns a chance to update / clear themselves; while all the filters
                // are cleared, their changes are told once, see clear
                window.setTimeout(() => !this.clearing && this.onChange && this.onChange(e1), 0);
            });
            removeHandlerOnDisposing(w);
        };

        const submitHandler = (request: ListRequest) => {

            if (qfElement.classList.contains('ignore')) {
                return;
            }

            request.EqualityFilter = request.EqualityFilter || {};
            const value = EditorUtils.getValue(widget);
            const active = !!value?.toString();
            if (opt.handler != null) {
                const args = {
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

        this.submitHandlers.push(submitHandler);
        this.items.push({ element: qfElement, widget: () => widget });
        this.updateToggleButton();

        if (isPromiseLike(opt.type)) {
            const placeholder = new QuickFilterPlaceholder({
                element: el => {
                    if (opt.field)
                        el.setAttribute('id', this.options.idPrefix + opt.field);
                    qfElement.append(el);
                }
            });
            widget = placeholder as Widget<any> as TWidget;
            removeHandlerOnDisposing(placeholder);

            opt.type.then(type => {
                // the bar or the quick filter is destroyed meanwhile
                if (!this.domNode || placeholder.domNode?.parentNode !== qfElement)
                    return;

                const loaded = createWidget(type, el => placeholder.domNode.replaceWith(el));
                widget = loaded;
                const value = placeholder.value;
                placeholder.destroy();
                // before listening to the changes, as this is not a change of the value of the quick filter
                if (value != null)
                    EditorUtils.setValue(loaded, value);
                activate(loaded);
            });
        }
        else {
            widget = createWidget(opt.type, el => qfElement.append(el));
            activate(widget);
        }

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
     * Creates a quick filter of a date field that matches a single date, e.g. the orders of a day. Its value goes to the
     * equality filter of the request by default, so the field has to be equal to it. It is the filter of a date column
     * that tells its quick filter not to edit a range, see {@link QuickFilterBar.dateRange} and
     * `QuickFilterAttribute.Range`.
     * @param field - Field name the filter is bound to.
     * @param title - Optional display title.
     * @returns A quick filter definition for a single date.
     */
    public static dateEqual(field: string, title?: string): QuickFilter<DateEditor, DateEditorOptions> {
        return {
            field: field,
            type: DateEditor,
            title: title
        };
    }

    /**
     * Creates a date range quick filter definition for the specified field.
     * @param field - Field name the filter is bound to.
     * @param title - Optional display title.
     * @param quickFilterParams - The quick filter params of the column of the field, if any. When `range` is false in
     * them, e.g. for a column that has `[QuickFilter(Range = false)]`, the filter of a single date is returned instead.
     * @returns A quick filter definition for a date range, or for a single date.
     */
    public static dateRange(field: string, title?: string, quickFilterParams?: any): QuickFilter<DateEditor, DateTimeEditorOptions> {
        if (quickFilterParams?.["range"] === false)
            return QuickFilterBar.dateEqual(field, title);

        let end: DateEditor = null;
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
                let date1 = parseDate(args.widget.value);
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

                let date2 = parseDate(end.value);
                if (date2) {
                    if (isNaN(date2?.valueOf())) {
                        notifyWarning(FormValidationTexts.DateInvalid, '', null);
                        end.domNode.value = "";
                        date2 = null;
                    }
                    else {
                        const next = new Date(end.valueAsDate.valueOf());
                        next.setDate(next.getDate() + 1);
                        args.request.Criteria = Criteria.and(args.request.Criteria,
                            Criteria(args.field).lt(formatDate(next, 'yyyy-MM-dd')));
                    }
                }

                args.active = !!(date1 || date2);
            },
            displayText: function (w, l) {
                const v1 = EditorUtils.getDisplayText(w);
                const v2 = EditorUtils.getDisplayText(end);
                if (!v1 && !v2)
                    return null;
                const text1 = l + ' >= ' + v1;
                const text2 = l + ' <= ' + v2;
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
        let end: DateTimeEditor = null;

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
                let date1 = parseDate(args.widget.value);
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

                let date2 = parseDate(end.value);
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
                const v1 = EditorUtils.getDisplayText(w);
                const v2 = EditorUtils.getDisplayText(end);
                if (!v1 && !v2) {
                    return null;
                }
                const text1 = l + ' >= ' + v1;
                const text2 = l + ' <= ' + v2;
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
        const opt: SelectEditorOptions = {};
        const items = [];

        let trueText = yes;
        if (trueText == null) {
            trueText = (FilterPanelTexts.OperatorNames as any).true;
        }
        items.push(['1', trueText]);

        let falseText = no;
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
        this.resizeObserver?.disconnect();
        this.resizeObserver = null;
        super.destroy();
    }

    /**
     * Invokes all registered submit handlers with the given list request.
     * @param request - The list request being prepared.
     */
    public onSubmit(request: ListRequest) {
        this.submitHandlers?.forEach(handler => handler(request));
        // the handlers tell which filters are applied
        // the toggle button first, as the clear button is beside it while the filters are collapsed
        this.updateToggleButton();
        this.updateClearButton();
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