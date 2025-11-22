import { escapeHtml, type IDataView } from "../core";
import type { Column, ColumnMetadata } from "../core/column";
import type { CellRenderArgs, RowRenderArgs } from "./render-args";
import { renderCell } from "./render-cell";

export function renderRow<TItem>(this: void, args: RowRenderArgs<TItem>): void {

    const { activeRow, colLeft, colRight, grid, item, frozenPinned, range, row, sbStart, sbCenter, sbEnd } = args;
    const { pinnedStartLast, pinnedEndFirst, frozenTopLast, frozenBottomFirst } = frozenPinned;
    const dataLoading = row < grid.getDataLength() && !item;
    const cols = grid.getColumns();
    let rowCss = "slick-row" +
        (row <= frozenTopLast || frozenBottomFirst <= row ? ' frozen' : '') +
        (dataLoading ? " loading" : "") +
        (activeRow === row ? " active" : "") +
        (row % 2 == 1 ? " odd" : " even");

    if (item == null && row < 0)
        rowCss += " " + grid.getOptions().addNewRowCssClass;

    const itemMetadata = (grid.getData() as IDataView<TItem>).getItemMetadata?.(row);

    if (itemMetadata && itemMetadata.cssClasses) {
        rowCss += " " + itemMetadata.cssClasses;
    }

    const rowTag = `<div class="${escapeHtml(rowCss)}" data-row="${row}">`;

    sbCenter.push(rowTag);

    if (frozenPinned.pinnedStartLast >= 0) {
        sbStart.push(rowTag);
    }

    if (frozenPinned.pinnedEndFirst != Infinity) {
        sbEnd.push(rowTag);
    }

    let colspan: number | "*", col: Column;
    for (let cell = 0, colCount = cols.length; cell < colCount; cell++) {
        let colMetadata: ColumnMetadata = null;
        col = cols[cell];
        colspan = 1;

        if (itemMetadata && itemMetadata.columns) {
            colMetadata = itemMetadata.columns[col.id] || itemMetadata.columns[cell];
            colspan = (colMetadata && colMetadata.colspan) || 1;
            if (colspan === "*") {
                colspan = colCount - cell;
            }
        }

        const pinnedStart = cell <= pinnedStartLast;
        const pinnedEnd = cell >= pinnedEndFirst;
        // Do not render cells outside of the viewport.
        if (pinnedStart || pinnedEnd || colRight[Math.min(colCount - 1, cell + colspan - 1)] > range.leftPx) {
            if (!(pinnedStart || pinnedEnd) && colLeft[cell] > range.rightPx) {
                // All columns to the right are outside the range.
                if (pinnedEndFirst != Infinity)
                    break;
                cell = pinnedEndFirst - 1;
                continue;
            }
            const cellArgs = args as unknown as CellRenderArgs<TItem>;
            cellArgs.cell = cell;
            cellArgs.colspan = colspan;
            cellArgs.colMetadata = colMetadata;
            cellArgs.sb = pinnedStart ? sbStart : pinnedEnd ? sbEnd : sbCenter;
            renderCell(cellArgs);
        }

        if (colspan > 1) {
            cell += (colspan - 1);
        }
    }

    sbCenter.push("</div>");

    if (pinnedStartLast >= 0) {
        sbStart.push("</div>");
    }

    if (pinnedEndFirst != Infinity) {
        sbEnd.push("</div>");
    }
}
