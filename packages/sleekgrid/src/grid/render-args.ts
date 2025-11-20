import type { ISleekGrid } from "../core";
import type { CellStylesHash } from "../core/formatting";
import type { ViewRange } from "../core/viewrange";
import type { CachedRow } from "./internal";

export interface RowCellCommonRenderArgs<TItem> {
    activeCell: number;
    activeRow: number;
    cachedRow?: CachedRow;
    cellCssClasses?: Record<string, CellStylesHash>;
    colLeft: number[];
    colRight: number[];
    frozenPinned: {
        frozenBottomFirst: number;
        frozenTopLast: number;
        pinnedStartLast: number;
        pinnedEndFirst: number;
    };
    grid: ISleekGrid;
    item: TItem;
    row: number;
    rtl: boolean;
}

export interface RowRenderArgs<TItem> extends RowCellCommonRenderArgs<TItem> {
    range: ViewRange;
    sbCenter: string[];
    sbEnd: string[];
    sbStart: string[];
    getRowTop: (row: number) => number;
}

export interface CellRenderArgs<TItem> extends RowCellCommonRenderArgs<TItem> {
    cell: number;
    colMetadata?: any;
    colspan: number;
    sb: string[];
}

export interface RowCellRenderArgs<TItem> extends CellRenderArgs<TItem>, RowRenderArgs<TItem> {
}
