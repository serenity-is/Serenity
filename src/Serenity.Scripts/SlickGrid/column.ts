import type { GroupTotals } from "./group";
import type { Grid } from "./grid";
import type { Editor, ValidationResult } from "./editor";
import type { AsyncPostCleanup, AsyncPostRender, ColumnFormatter } from "./formatting";

export interface Column<TItem = any> {
    asyncPostRender?: AsyncPostRender<TItem>;
    asyncPostRenderCleanup?: AsyncPostCleanup<TItem>;
    behavior?: any;
    cannotTriggerInsert?: boolean;
    cssClass?: string;
    defaultSortAsc?: boolean;
    editor?: Editor;
    field: string;
    frozen?: boolean;
    focusable?: boolean;
    footerCssClass?: string;
    formatter?: ColumnFormatter<TItem>;
    groupTotalsFormatter?: (p1?: GroupTotals<TItem>, p2?: Column<TItem>, grid?: Grid<TItem>) => string;
    headerCssClass?: string;
    id?: string;
    maxWidth?: any;
    minWidth?: number;
    name?: string;
    previousWidth?: number;
    referencedFields?: string[];
    rerenderOnResize?: boolean;
    resizable?: boolean;
    selectable?: boolean;
    sortable?: boolean;
    sortOrder?: number;
    toolTip?: string;
    validator?: (value: any) => ValidationResult;
    visible?: boolean;
    width?: number;
}

export interface ColumnMetadata<TItem = any> {
    colspan: number | '*';
    formatter?: ColumnFormatter<TItem>;
}

export interface ColumnSort {
    columnId: string;
    sortAsc?: boolean;
}

export interface ItemMetadata<TItem = any> {
    columns?: { [key: string]: ColumnMetadata<TItem> };
    formatter?: ColumnFormatter<TItem>;
}