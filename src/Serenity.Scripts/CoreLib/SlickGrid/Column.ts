import { PropertyItem } from "../Services/PropertyItem";
import { GroupTotals } from "./Grouping";

export interface FormatterContext {
    row?: number;
    cell?: number;
    value?: any;
    column?: any;
    item?: any;
}

export interface Formatter {
    format(ctx: FormatterContext): string;
}

export type Format = (ctx: FormatterContext) => string;

export type AsyncPostRender = (cellNode: any, row: number, item: any, column: Column, clean?: boolean) => void;
export type ColumnFormatter = (row: number, cell: number, value: any, column: Column, item: any) => string;

export interface Column {
    asyncPostRender?: AsyncPostRender;
    behavior?: any;
    cannotTriggerInsert?: boolean;
    cssClass?: string;
    defaultSortAsc?: boolean;
    editor?: Function;
    field: string;
    focusable?: boolean;
    formatter?: ColumnFormatter;
    headerCssClass?: string;
    id?: string;
    maxWidth?: any;
    minWidth?: number;
    name?: string;
    rerenderOnResize?: boolean;
    resizable?: boolean;
    selectable?: boolean;
    sortable?: boolean;
    toolTip?: string;
    width?: number;
    format?: (ctx: FormatterContext) => string;
    referencedFields?: string[];
    sourceItem?: PropertyItem;
    sortOrder?: number;
    groupTotalsFormatter?: (p1?: GroupTotals<any>, p2?: Column) => string;
    visible?: boolean;
}