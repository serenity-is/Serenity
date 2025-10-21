import { FormatterContext, FormatterResult, Group } from "@serenity-is/sleekgrid";
import { PropertyItem } from "../base";
import { IAggregator } from "./aggregators";

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
    getter?: string | ((item: TItem) => any);
    getterIsAFn?: boolean;
    /** 
     * The format function for the group value. Note that the group item is in ctx.item and its value 
     * is in ctx.item.value, not in ctx.value as it is set by the grid to ctx.item["__groupdisplaycolumnfield__"]
     * so never use or rely on ctx.value here!
     */
    format?: (ctx: FormatterContext<Group<TItem>>) => FormatterResult;
    /** @obsolete use format */
    formatter?: (group: Group<TItem>) => string;
    comparer?: (a: Group<TItem>, b: Group<TItem>) => number;
    aggregators?: IAggregator[];
    aggregateChildGroups?: boolean;
    aggregateCollapsed?: boolean;
    aggregateEmpty?: boolean;
    collapsed?: boolean;
    displayTotalsRow?: boolean;
    lazyTotalsCalculation?: boolean;
    predefinedValues?: any[];
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
    aggregators: IAggregator[];
}

export interface PagingOptions {
    rowsPerPage?: number;
    page?: number;
}
