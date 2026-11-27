import { FormatterContext, FormatterResult, Group, ISleekGrid } from "@serenity-is/sleekgrid";
import { PropertyItem, RenderableContent } from "../base";
import { IAggregator } from "./aggregators";
import type { PagingInfo } from "./iremoteview";

/** Formatter function type that maps a formatter context to a result. @typeParam TItem - Row item type. */
export type Format<TItem = any> = (ctx: FormatterContext<TItem>) => FormatterResult;

declare module "@serenity-is/sleekgrid" {
    export interface Column<TItem = any> {
        /** Fields that this column depends on for its formatting or values */
        referencedFields?: string[];
        /** Source PropertyItem from which this column was created */
        sourceItem?: PropertyItem;
        /** If false, the hide column action will be hidden for this column (column picker / via menu) */
        togglable?: boolean;
        /** If false, the move column actions will be hidden for this column (column picker / via menu) */
        movable?: boolean;
    }
}

/** Legacy formatter contract. Prefer {@link Format}. */
export interface Formatter {
    /** Formats a cell value. @param ctx - Formatter context with item/column/value/grid. @returns Formatted result. */
    format(ctx: FormatterContext): FormatterResult;
}

/** Configuration for a single grouping level. */
export interface GroupInfo<TItem> {
    /** Field name or getter for the group value. */
    getter?: string | ((item: TItem) => any);
    /** True if `getter` is a function. */
    getterIsAFn?: boolean;
    /** 
     * Formats the group header. Note: group value is in `ctx.item.value`, not `ctx.value`.
     * @param ctx - Formatter context for the group row.
     * @returns Formatter result.
     */
    format?: (ctx: FormatterContext<Group<TItem>>) => FormatterResult;
    /** @deprecated Use `format` instead. @param group - Group object. @returns Formatted group title. */
    formatter?: (group: Group<TItem>) => string;
    /** Comparator for group ordering. @param a - First group. @param b - Second group. @returns Negative / zero / positive. */
    comparer?: (a: Group<TItem>, b: Group<TItem>) => number;
    /** Aggregators applied to this group level. */
    aggregators?: IAggregator[];
    /** Whether to aggregate child groups as well. */
    aggregateChildGroups?: boolean;
    /** Whether collapsed groups still show aggregates. */
    aggregateCollapsed?: boolean;
    /** Whether empty groups still show aggregates. */
    aggregateEmpty?: boolean;
    /** True if groups start collapsed. */
    collapsed?: boolean;
    /**
     * Where the totals of a group of this level are shown, e.g. the aggregates of the summary columns:
     * on a row of their own (`true`), on the row of the group itself (`"groupheader"`), or not at all
     * (`false`). Defaults to `true`.
     */
    displayTotalsRow?: boolean | "groupheader";
    /** True to calculate totals lazily. */
    lazyTotalsCalculation?: boolean;
    /** Predefined group values to ensure groups exist even without data. */
    predefinedValues?: any[];
}

/** Options for the slick pager control. */
export interface PagerOptions {
    /** Data view instance. */
    view?: any;
    /**
     * The grid that shows the rows of the view. It is given to a view that loads its rows as the grid is
     * scrolled, see {@link IGridBoundView.bindToGrid}.
     */
    grid?: ISleekGrid;
    /**
     * Gets the content the pager shows before its controls, e.g. the totals of the page and of all the rows.
     * It is called on every update, so it can follow the rows, and it is not shown when it returns nothing.
     * A string is shown as text, so anything with markup is returned as an element or a fragment instead.
     * The content is only rendered again when what it returns changes, so a function that builds a new
     * element on every call renders on every update too.
     * Default is what the view gives, see {@link IRemoteView.getPagingInfoText}.
     */
    getInfoText?: (info: PagingInfo) => RenderableContent;
    /** Whether to show rows-per-page selector. Default is true, but false for a view that shows a range of its rows, e.g. an infinite scroll view, whose number of rows is set by the developer. */
    showRowsPerPage?: boolean;
    /**
     * Whether to show the buttons that go to the first and last pages, which most pagers leave out, as
     * the page box takes a number anyway. Shift+click on the previous and next buttons goes to the first and last
     * pages instead, while they are left out. Default is `SlickPager.defaults.showFirstLast`, which is false.
     */
    showFirstLast?: boolean;
    /** Whether to show a reload button. Default is true, even when the grid has a refresh button of its own. */
    showReload?: boolean;
    /** Current rows per page. */
    rowsPerPage?: number;
    /** Choices for rows-per-page selector. */
    rowsPerPageOptions?: number[],
    /**
     * Whether to page without the total number of the rows: no last page button, no total of the pages, and a next page button
     * that is only enabled while there are more rows. Default is the setting of the view, see {@link RemoteViewOptions.indeterminatePaging}.
     */
    indeterminatePaging?: boolean;
    /** Callback when page changes. @param newPage - New page index (1-based). */
    onChangePage?: (newPage: number) => void;
    /** Callback when rows-per-page changes. @param n - New rows-per-page value. */
    onRowsPerPageChange?: (n: number) => void;
}

/** Aggregator configuration for view-level summaries. */
export interface SummaryOptions {
    /** Aggregators used for grand totals. */
    aggregators: IAggregator[];
}

/** Paging state for a remote/slick data view. */
export interface PagingOptions {
    /** Rows per page. */
    rowsPerPage?: number;
    /** Current page (1-based). */
    page?: number;
}
