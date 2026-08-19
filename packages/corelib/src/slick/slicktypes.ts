import { FormatterContext, FormatterResult, Group } from "@serenity-is/sleekgrid";
import { PropertyItem } from "../base";
import { IAggregator } from "./aggregators";

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
    /** True to render a totals row for this level. */
    displayTotalsRow?: boolean;
    /** True to calculate totals lazily. */
    lazyTotalsCalculation?: boolean;
    /** Predefined group values to ensure groups exist even without data. */
    predefinedValues?: any[];
}

/** Options for the slick pager control. */
export interface PagerOptions {
    /** Data view instance. */
    view?: any;
    /** Whether to show rows-per-page selector. */
    showRowsPerPage?: boolean;
    /** Current rows per page. */
    rowsPerPage?: number;
    /** Choices for rows-per-page selector. */
    rowsPerPageOptions?: number[],
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
