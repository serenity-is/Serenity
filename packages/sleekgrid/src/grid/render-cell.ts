import { formatterContext, type FormatterResult } from "../core/formatting";
import { escapeHtml } from "../core/util";
import type { CellRenderArgs } from "./render-args";

export function renderCell<TItem>(this: void, { activeCell, activeRow, cell, cellCssClasses, colMetadata,
    colspan, grid, item, sb, row, rtl, frozenPinned, cachedRow }: CellRenderArgs<TItem>): void {
    const cols = grid.getColumns();
    const column = cols[cell];
    let klass = "slick-cell" + (rtl ? " r" : " l") + cell + (rtl ? " l" : " r") + Math.min(cols.length - 1, cell + colspan - 1) +
        (column.cssClass ? " " + column.cssClass : "");

    if (cell <= frozenPinned.pinnedStartLast)
        klass += ' frozen pinned-start';
    else if (cell >= frozenPinned.pinnedEndFirst)
        klass += ' frozen pinned-end';

    if (activeCell === cell && activeRow === row)
        klass += " active";

    if (colMetadata && colMetadata.cssClass) {
        klass += " " + colMetadata.cssClass;
    }

    for (const key in cellCssClasses) {
        const cls = cellCssClasses[key][row];
        if (cls && cls[column.id]) {
            klass += (" " + cls[column.id]);
        }
    }

    // if there is a corresponding row (if not, this is the Add New row or this data hasn't been loaded yet)
    var fmtResult: FormatterResult;
    const ctx = formatterContext<TItem>({
        cell,
        column,
        item,
        row,
        grid
    });

    if (item) {
        ctx.value = grid.getDataItemValueForColumn(item, column);
        fmtResult = grid.getFormatter(row, column)(ctx);
        if (typeof fmtResult === "string" && fmtResult.length) {
            if (ctx.enableHtmlRendering)
                fmtResult = (ctx.sanitizer ?? escapeHtml)(fmtResult);
            else
                fmtResult = escapeHtml(fmtResult);
        }
    }

    klass = escapeHtml(klass);

    if (ctx.addClass?.length || ctx.addAttrs?.length || ctx.tooltip?.length) {
        if (ctx.addClass?.length)
            klass += (" " + escapeHtml(ctx.addClass));

        sb.push('<div class="' + klass + '"');

        if (ctx.addClass?.length)
            sb.push(' data-fmtcls="' + escapeHtml(ctx.addClass) + '"');

        var attrs = ctx.addAttrs;
        if (attrs != null) {
            var ks = [];
            for (var k in attrs) {
                sb.push(k + '="' + escapeHtml(attrs[k]) + '"');
                ks.push(k);
            }
            sb.push(' data-fmtatt="' + escapeHtml(ks.join(',')) + '"');
        }

        var toolTip = ctx.tooltip;
        if (toolTip != null && toolTip.length)
            sb.push('tooltip="' + escapeHtml(toolTip) + '"');

        if (fmtResult != null && !(fmtResult instanceof Node))
            sb.push('>' + fmtResult + '</div>');
        else
            sb.push('></div>');
    }
    else if (fmtResult != null && !(fmtResult instanceof Node))
        sb.push('<div class="' + klass + '">' + fmtResult + '</div>');
    else
        sb.push('<div class="' + klass + '"></div>');

    cachedRow.cellRenderQueue.push(cell);
    cachedRow.cellRenderContent.push(fmtResult instanceof Node ? fmtResult : void 0);
    cachedRow.cellColSpans[cell] = colspan;
}
