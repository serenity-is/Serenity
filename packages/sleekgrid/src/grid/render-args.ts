import type { IGrid } from "../core";
import type { CellStylesHash } from "../core/formatting";
import type { ViewRange } from "../core/viewrange";
import type { CachedRow } from "./internal";

export interface RowCellCommonRenderArgs<TItem> {
    activeCell: number;
    activeRow: number;
    cachedRow?: CachedRow;
    cellCssClasses?: Record<string, CellStylesHash>;
    frozenPinned: {
        frozenBottomFirst: number,
        frozenTopLast: number,
        pinnedStartLast: number,
        pinnedEndFirst: number
    };
    grid: Pick<IGrid<TItem>, "getColumns" | "getData" | "getDataItemValueForColumn" | "getDataLength" | "getFormatter"> & {
        getOptions: () => { addNewRowCssClass?: string }
    };
    item: TItem;
    row: number;
}

export interface RowRenderArgs<TItem> extends RowCellCommonRenderArgs<TItem> {
    colLeft: number[];
    colRight: number[];
    range: ViewRange;
    sbCenter: string[];
    sbEnd: string[];
    sbStart: string[];
    getFrozenRowOffset: (row: number) => number;
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
