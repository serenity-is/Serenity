import { PropertyItem } from "../base";
import { FormatterContext, FormatterResult, Group } from "@serenity-is/sleekgrid";

export type Format<TItem = any> = (ctx: FormatterContext<TItem>) => FormatterResult;

declare module "@serenity-is/sleekgrid" {
    export interface Column<TItem = any> {
        referencedFields?: string[];
        sourceItem?: PropertyItem;
    }
}

export interface Formatter {
    format(ctx: FormatterContext): FormatterResult;
}

export interface GroupInfo<TItem> {
    getter?: any;
    formatter?: (p1: Group<TItem>) => string;
    comparer?: (a: Group<TItem>, b: Group<TItem>) => number;
    aggregators?: any[];
    aggregateCollapsed?: boolean;
    lazyTotalsCalculation?: boolean;
}

export interface PagerOptions {
    view?: any;
    showRowsPerPage?: boolean;
    rowsPerPage?: number;
    rowsPerPageOptions?: number[],
    onChangePage?: (newPage: number) => void;
    onRowsPerPageChange?: (n: number) => void;
}

export interface SummaryOptions {
    aggregators: any[];
}

export interface PagingOptions {
    rowsPerPage?: number;
    page?: number;
}
