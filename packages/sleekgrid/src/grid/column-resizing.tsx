import type { GridOptions } from "../core";
import type { Column } from "../core/column";
import type { EditorLock } from "../core/editing";

export function setupColumnResize<TItem>(this: void, { absoluteColMinWidth, container,
    cols, colResizing, colResized, disposer, headerColsElements, getEditorLock, options, removeNode
}: {
    absoluteColMinWidth: number,
    disposer: AbortController
    cols: Column<TItem>[],
    colResizing: (cell: number) => void,
    colResized: (invalidateAll: boolean, cell: number) => void,
    container: HTMLElement,
    getEditorLock: () => EditorLock,
    headerColsElements: HTMLElement[],
    options: Pick<GridOptions, "forceFitColumns">,
    removeNode: (node: Element) => void,
}) {
    let resizingCell: number, pageX: number, minPageX: number, maxPageX: number;

    let headerColEls: Element[] = [];
    headerColsElements.forEach(hcols => {
        headerColEls = headerColEls.concat(Array.from(hcols.children)
            .filter(x => x.classList.contains("slick-header-column")));
    });

    let firstResizable: number, lastResizable: number;
    headerColEls.forEach((el, i) => {
        if (i > cols.length)
            return;
        const handle = el.querySelector(".slick-resizable-handle");
        handle && removeNode(handle);
        if (cols[i].resizable) {
            if (firstResizable === undefined) {
                firstResizable = i;
            }
            lastResizable = i;
        }
    });

    if (firstResizable === undefined) {
        return;
    }

    const docMouseMove = (e: MouseEvent) => {
        if (resizingCell == null)
            return;
        var dist;
        var thisPageX = e.pageX;
        dist = Math.min(maxPageX, Math.max(minPageX, thisPageX)) - pageX;
        if (isNaN(dist)) {
            return;
        }
        shrinkOrStretchColumn({
            cols: cols,
            cell: resizingCell,
            dist,
            forceFit: options.forceFitColumns,
            absoluteColMinWidth: absoluteColMinWidth
        });
        colResizing(resizingCell);
    };

    const docMouseUp = (e: any) => {
        document.removeEventListener('mousemove', docMouseMove);
        document.removeEventListener('mouseup', docMouseUp);
        container.classList.remove('slick-column-resizing');
        let invalidateAll = false;
        for (let j = 0; j < headerColEls.length; j++) {
            const c = cols[j];
            const newWidth = (headerColEls[j] as HTMLElement).offsetWidth;

            if (c.previousWidth !== newWidth && c.rerenderOnResize) {
                invalidateAll = true;
            }
        }
        colResized(invalidateAll, resizingCell);
        resizingCell = null;
    };

    const mouseDown = (e: MouseEvent) => {
        if (!getEditorLock().commitCurrentEdit()) {
            e.preventDefault();
            return;
        }

        resizingCell = Number((e.target as HTMLElement)?.closest?.<HTMLElement>(".slick-header-column")?.dataset?.c ?? "");
        if (isNaN(resizingCell)) {
            return;
        }

        pageX = e.pageX;

        // lock each column's width option to current width
        headerColEls.forEach((e, z) => {
            cols[z].previousWidth = (e as HTMLElement).offsetWidth;
        });

        ({ maxPageX, minPageX } = calcMinMaxPageXOnDragStart({
            cols: cols,
            cell: resizingCell,
            pageX,
            absoluteColMinWidth,
            forceFit: options.forceFitColumns
        }));
        document.addEventListener('mousemove', docMouseMove, { signal: disposer.signal });
        document.addEventListener('mouseup', docMouseUp, { signal: disposer.signal });
        container.classList.add("slick-column-resizing");
        e.preventDefault();
    };

    headerColEls.forEach((headerEl, cell) => {
        if (cell < firstResizable || (options.forceFitColumns && cell >= lastResizable)) {
            return;
        }

        const handle = headerEl.appendChild(<div class="slick-resizable-handle" /> as HTMLElement);
        handle.addEventListener("mousedown", mouseDown, { signal: disposer.signal });
    });
}

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

function shrinkOrStretchColumn({ absoluteColMinWidth, cols, dist, cell: cell, forceFit }: {
    absoluteColMinWidth: number,
    cols: Column[],
    cell: number,
    dist: number,
    forceFit: boolean
}): void {
    var c: Column, j: number, x: number, actualMinWidth: number;

    if (dist < 0) { // shrink column
        x = dist;

        for (j = cell; j >= 0; j--) {
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
            x = -dist;
            for (j = cell + 1; j < cols.length; j++) {
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
    } else if (dist > 0) { // stretch column
        x = dist;

        for (j = cell; j >= 0; j--) {
            c = cols[j];
            if (c.resizable) {
                if (x && c.maxWidth && (c.maxWidth - (c.previousWidth || 0) < x)) {
                    x -= c.maxWidth - (c.previousWidth || 0);
                    c.width = c.maxWidth;
                } else {
                    c.width = (c.previousWidth || 0) + x;
                    x = 0;
                    if (x == 0)
                        break;
                }
            }
        }

        if (forceFit) {
            x = -dist;
            for (j = cell + 1; j < cols.length; j++) {
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

function calcMinMaxPageXOnDragStart({ absoluteColMinWidth, cols, cell, forceFit, pageX }: {
    absoluteColMinWidth: number,
    cols: Column[],
    cell: number,
    forceFit: boolean,
    pageX: number
}): { maxPageX: number; minPageX: number; } {
    var shrinkLeewayOnRight = null, stretchLeewayOnRight = null, j: number, c: Column;
    if (forceFit) {
        shrinkLeewayOnRight = 0;
        stretchLeewayOnRight = 0;
        // colums on right affect maxPageX/minPageX
        for (j = cell + 1; j < cols.length; j++) {
            c = cols[j];
            if (c.resizable) {
                if (stretchLeewayOnRight != null) {
                    if (c.maxWidth) {
                        stretchLeewayOnRight += c.maxWidth - c.previousWidth;
                    } else {
                        stretchLeewayOnRight = null;
                    }
                }
                shrinkLeewayOnRight += c.previousWidth - Math.max(c.minWidth || 0, absoluteColMinWidth);
            }
        }
    }
    var shrinkLeewayOnLeft = 0, stretchLeewayOnLeft = 0;
    for (j = 0; j <= cell; j++) {
        // columns on left only affect minPageX
        c = cols[j];
        if (c.resizable) {
            if (stretchLeewayOnLeft != null) {
                if (c.maxWidth) {
                    stretchLeewayOnLeft += c.maxWidth - c.previousWidth;
                } else {
                    stretchLeewayOnLeft = null;
                }
            }
            shrinkLeewayOnLeft += c.previousWidth - Math.max(c.minWidth || 0, absoluteColMinWidth);
        }
    }
    if (shrinkLeewayOnRight === null) {
        shrinkLeewayOnRight = 100000;
    }
    if (shrinkLeewayOnLeft === null) {
        shrinkLeewayOnLeft = 100000;
    }
    if (stretchLeewayOnRight === null) {
        stretchLeewayOnRight = 100000;
    }
    if (stretchLeewayOnLeft === null) {
        stretchLeewayOnLeft = 100000;
    }

    return {
        maxPageX: pageX + Math.min(shrinkLeewayOnRight, stretchLeewayOnLeft),
        minPageX: pageX - Math.min(shrinkLeewayOnLeft, stretchLeewayOnRight)
    }
}
