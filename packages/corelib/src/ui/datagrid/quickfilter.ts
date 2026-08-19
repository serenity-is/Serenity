import { Fluent, ListRequest } from "../../base";
import { Widget, WidgetProps } from "../widgets/widget";

/**
 * Arguments passed to a quick filter handler when a list request is prepared.
 * @typeParam TWidget - The widget type that backs the quick filter.
 */
export interface QuickFilterArgs<TWidget> {
    /** Field name the filter is bound to. */
    field?: string;
    /** Widget instance for the quick filter, if created. */
    widget?: TWidget;
    /** Current list request being built; handler may mutate criteria. */
    request?: ListRequest;
    /** Equality filter value derived from the widget, if any. */
    equalityFilter?: any;
    /** Canonical value of the filter. */
    value?: any;
    /** Whether the filter is currently considered active. */
    active?: boolean;
    /** When set, the framework skips default equality-filter handling. */
    handled?: boolean;
}

/**
 * Definition for a single quick filter rendered in the grid toolbar.
 * @typeParam TWidget - Widget type that provides the filter UI.
 * @typeParam P - Props/options type for the widget.
 */
export interface QuickFilter<TWidget extends Widget<P>, P> {
    /** Field name associated with the quick filter. */
    field?: string;
    /** Widget constructor used to create the filter editor. */
    type?: { new(options?: P): TWidget, prototype: TWidget };
    /** Callback invoked when the list request is prepared; may mutate the request. */
    handler?: (h: QuickFilterArgs<TWidget>) => void;
    /** Title / label shown for the filter. */
    title?: string;
    /** Options passed to the widget constructor; merged with {@link WidgetProps}. */
    options?: P & WidgetProps<{}>;
    /** Optional callback to customize the filter container element. */
    element?: (e: Fluent) => void;
    /** Callback invoked after the widget instance is created for additional setup. */
    init?: (w: TWidget) => void;
    /** When true, a visual separator is rendered before this filter. */
    separator?: boolean;
    /** Extra CSS class applied to the filter item container. */
    cssClass?: string;
    /** Restores persisted filter state into the widget. */
    loadState?: (w: TWidget, state: any) => void;
    /** Persists widget state for grid settings. */
    saveState?: (w: TWidget) => any;
    /** Returns human-readable text for the active filter display. */
    displayText?: (w: TWidget, label: string) => string;
}