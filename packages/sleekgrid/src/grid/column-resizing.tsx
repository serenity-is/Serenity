import type { Column } from "../core/column";

export function autosizeColumns(cols: Column[], availWidth: number, absoluteColMinWidth: number): boolean {
    var i, c,
        widths = [],
        shrinkLeeway = 0,
        total = 0,
        prevTotal;

    for (i = 0; i < cols.length; i++) {
        c = cols[i];
        widths.push(c.width);
        total += c.width;
        if (c.resizable) {
            shrinkLeeway += c.width - Math.max(c.minWidth, absoluteColMinWidth);
        }
    }

    // shrink
    prevTotal = total;
    while (total > availWidth && shrinkLeeway) {
        var shrinkProportion = (total - availWidth) / shrinkLeeway;
        for (i = 0; i < cols.length && total > availWidth; i++) {
            c = cols[i];
            var width = widths[i];
            if (!c.resizable || width <= c.minWidth || width <= absoluteColMinWidth) {
                continue;
            }
            var absMinWidth = Math.max(c.minWidth, absoluteColMinWidth);
            var shrinkSize = Math.floor(shrinkProportion * (width - absMinWidth)) || 1;
            shrinkSize = Math.min(shrinkSize, width - absMinWidth);
            total -= shrinkSize;
            shrinkLeeway -= shrinkSize;
            widths[i] -= shrinkSize;
        }
        if (prevTotal <= total) {  // avoid infinite loop
            break;
        }
        prevTotal = total;
    }

    // grow
    prevTotal = total;
    while (total < availWidth) {
        var growProportion = availWidth / total;
        for (i = 0; i < cols.length && total < availWidth; i++) {
            c = cols[i];
            var currentWidth = widths[i];
            var growSize;

            if (!c.resizable || c.maxWidth <= currentWidth) {
                growSize = 0;
            } else {
                growSize = Math.min(Math.floor(growProportion * currentWidth) - currentWidth, (c.maxWidth - currentWidth) || 1000000) || 1;
            }
            total += growSize;
            widths[i] += (total <= availWidth ? growSize : 0);
        }
        if (prevTotal >= total) {  // avoid infinite loop
            break;
        }
        prevTotal = total;
    }

    var reRender = false;
    for (i = 0; i < cols.length; i++) {
        if (cols[i].rerenderOnResize && cols[i].width != widths[i]) {
            reRender = true;
        }
        cols[i].width = widths[i];
    }

    return reRender;
}


export function shrinkOrStretchColumn(cols: Column[], colIdx: number, d: number, forceFit: boolean, absoluteColMinWidth: number): void {
    var c: Column, j: number, x: number, actualMinWidth: number;

    if (d < 0) { // shrink column
        x = d;

        for (j = colIdx; j >= 0; j--) {
            c = cols[j];
            if (c.resizable) {
                actualMinWidth = Math.max(c.minWidth || 0, absoluteColMinWidth);
                if (x && c.previousWidth + x < actualMinWidth) {
                    x += c.previousWidth - actualMinWidth;
                    c.width = actualMinWidth;
                } else {
                    c.width = c.previousWidth + x;
                    x = 0;
                }
            }
        }

        if (forceFit) {
            x = -d;
            for (j = colIdx + 1; j < cols.length; j++) {
                c = cols[j];
                if (c.resizable) {
                    if (x && c.maxWidth && (c.maxWidth - c.previousWidth < x)) {
                        x -= c.maxWidth - c.previousWidth;
                        c.width = c.maxWidth;
                    } else {
                        c.width = c.previousWidth + x;
                        x = 0;
                    }
                }
            }
        }
    } else { // stretch column
        x = d;

        for (j = colIdx; j >= 0; j--) {
            c = cols[j];
            if (c.resizable) {
                if (x && c.maxWidth && (c.maxWidth - c.previousWidth < x)) {
                    x -= c.maxWidth - c.previousWidth;
                    c.width = c.maxWidth;
                } else {
                    c.width = c.previousWidth + x;
                    x = 0;
                }
            }
        }

        if (forceFit) {
            x = -d;
            for (j = colIdx + 1; j < cols.length; j++) {
                c = cols[j];
                if (c.resizable) {
                    actualMinWidth = Math.max(c.minWidth || 0, absoluteColMinWidth);
                    if (x && c.previousWidth + x < actualMinWidth) {
                        x += c.previousWidth - actualMinWidth;
                        c.width = actualMinWidth;

                    } else {
                        c.width = c.previousWidth + x;
                        x = 0;
                    }
                }
            }
        }
    }
}
