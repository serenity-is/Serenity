import type { Column, Event, Grid, Group, IPlugin, ItemMetadata, Range, SelectionModel } from "@serenity-is/sleekgrid"
import { PropertyItem } from "../q";

export type Format<TItem = any> = (ctx: FormatterContext<TItem>) => string;

declare module "@serenity-is/sleekgrid" {
    interface Column<TItem = any> {
        referencedFields?: string[];
        format?: Format<TItem>;
        sourceItem?: PropertyItem;
    }
}

export interface FormatterContext<TItem = any> {
    addAttrs?: { [key: string]: string; };
    addClass?: string;
    cell?: number;
    column?: Column<TItem>;
    grid?: Grid<TItem>;
    item?: TItem;
    row?: number;
    toolTip?: string;
    value?: any;
}

export interface Formatter {
    format(ctx: FormatterContext): string;
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
    

export declare class RowSelectionModel implements SelectionModel {
    init(grid: Grid): void;
    destroy?: () => void;
    setSelectedRanges(ranges: Range[]): void;
    onSelectedRangesChanged: Event<Range[]>;
    refreshSelections?(): void;
}

export interface SummaryOptions {
    aggregators: any[];
}
    
export interface PagingOptions {
    rowsPerPage?: number;
    page?: number;
}
