import { Column, parsePx, Position } from "../core";

// shared across all grids on the page
let maxSupportedCssHeight: number;  // browser's breaking point
let scrollbarDimensions: { width: number, height: number };

export function absBox(elem: HTMLElement): Position {
    var box: Position = {
        top: elem.offsetTop,
        left: elem.offsetLeft,
        bottom: 0,
        right: 0,
        width: elem.offsetWidth,
        height: elem.offsetHeight,
        visible: true
    };

    box.bottom = box.top + box.height;
    box.right = box.left + box.width;

    // walk up the tree
    var offsetParent = elem.offsetParent;
    while ((elem = elem.parentNode as HTMLElement) != document.body && elem != null) {
        if (box.visible && elem.scrollHeight != elem.offsetHeight && getComputedStyle(elem).overflowY !== "visible") {
            box.visible = box.bottom > elem.scrollTop && box.top < elem.scrollTop + elem.clientHeight;
        }

        if (box.visible && elem.scrollWidth != elem.offsetWidth && getComputedStyle(elem).overflowX != "visible") {
            box.visible = box.right > elem.scrollLeft && box.left < elem.scrollLeft + elem.clientWidth;
        }

        box.left -= elem.scrollLeft;
        box.top -= elem.scrollTop;

        if (elem === offsetParent) {
            box.left += elem.offsetLeft;
            box.top += elem.offsetTop;
            offsetParent = elem.offsetParent;
        }

        box.bottom = box.top + box.height;
        box.right = box.left + box.width;
    }

    return box;
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

export function getMaxSupportedCssHeight(recalc?: boolean): number {
    if (!recalc && maxSupportedCssHeight != null)
        return maxSupportedCssHeight;
    return (maxSupportedCssHeight = ((navigator.userAgent.toLowerCase().match(/gecko\//) ? 4000000 : 32000000)));
}

export function getScrollBarDimensions(recalc?: boolean): { width: number; height: number; } {
    if (!scrollbarDimensions || recalc) {
        const c = document.body.appendChild(<div style="position:absolute;top:-10000px;left:-10000px;width:100px;height:100px;overflow: scroll;border:0" /> as HTMLElement);
        scrollbarDimensions = {
            width: Math.round(c.offsetWidth - c.clientWidth),
            height: Math.round(c.offsetWidth - c.clientHeight)
        };
        c.remove();
    }
    return scrollbarDimensions;
}


export function simpleArrayEquals(arr1: number[], arr2: number[]) {
    if (!Array.isArray(arr1) || !Array.isArray(arr2) || arr1.length !== arr2.length)
        return false;
    arr1 = arr1.slice().sort();
    arr2 = arr2.slice().sort();
    for (var i = 0; i < arr1.length; i++) {
        if (arr1[i] !== arr2[i])
            return false;
    }
    return true;
}

/**
 * Helper to sort visible cols, while keeping invisible cols sticky to
 * the previous visible col. For example, if columns are currently in order
 * A, B, C, D, E, F, G, H and desired order is G, D, F (assuming A, B, C, E
 * were invisible) the result is A, B, G, H, D, E, F.
 */
export function sortToDesiredOrderAndKeepRest(columns: Column[], idOrder: string[]): Column[] {
    if (idOrder.length == 0)
        return columns;

    var orderById: { [key: string]: number } = {},
        colIdxById: { [key: string]: number } = {},
        result: Column[] = [];

    for (var i = 0; i < idOrder.length; i++)
        orderById[idOrder[i]] = i;

    for (i = 0; i < columns.length; i++)
        colIdxById[columns[i].id] = i;

    function takeFrom(i: number) {
        for (var j = i; j < columns.length; j++) {
            var c = columns[j];
            if (i != j && orderById[c.id] != null)
                break;
            result.push(c);
            colIdxById[c.id] = null;
        }
    }

    if (orderById[columns[0].id] == null)
        takeFrom(0);

    for (var id of idOrder) {
        i = colIdxById[id];
        if (i != null)
            takeFrom(i);
    }

    for (i = 0; i < columns.length; i++) {
        var c = columns[i];
        if (colIdxById[c.id] != null) {
            result.push(c);
            colIdxById[c.id] = null;
        }
    }

    return result;
}

export function calcMinMaxPageXOnDragStart(cols: Column[], colIdx: number, pageX: number, forceFit: boolean, absoluteColMinWidth: number): { maxPageX: number; minPageX: number; } {
    var shrinkLeewayOnRight = null, stretchLeewayOnRight = null, j: number, c: Column;
    if (forceFit) {
        shrinkLeewayOnRight = 0;
        stretchLeewayOnRight = 0;
        // colums on right affect maxPageX/minPageX
        for (j = colIdx + 1; j < cols.length; j++) {
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
    for (j = 0; j <= colIdx; j++) {
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

export function addUiStateHover() {
    (this as HTMLElement)?.classList.add("ui-state-hover");
}

export function removeUiStateHover() {
    (this as HTMLElement)?.classList.remove("ui-state-hover");
}

export function getVBoxDelta(el: HTMLElement): number {
    if (!el)
        return 0;

    var style = getComputedStyle(el);
    if (style.boxSizing === 'border-box')
        return 0;

    var p = ["border-top-width", "border-bottom-width", "padding-top", "padding-bottom"];
    var delta = 0;
    for (var val of p)
        delta += parsePx(style.getPropertyValue(val)) || 0;
    return delta;
}

export function getInnerWidth(el: HTMLElement): number {
    var style = getComputedStyle(el);
    var width = parsePx(style.width) ?? 0;
    if (style.boxSizing != 'border-box')
        return Math.max(0, width);

    var p = ["border-left-width", "border-right-width", "padding-left", "padding-right"];
    for (var val of p)
        width -= parsePx(style.getPropertyValue(val)) || 0;

    return Math.max(width, 0);
}

export interface CachedRow {
    rowNodeS: HTMLElement,
    rowNodeC: HTMLElement,
    rowNodeE: HTMLElement,
    // ColSpans of rendered cells (by column idx).
    // Can also be used for checking whether a cell has been rendered.
    cellColSpans: number[],

    // Cell nodes (by column idx).  Lazy-populated by ensureCellNodesInRowsCache().
    cellNodesByColumnIdx: { [key: number]: HTMLElement },

    // Column indices of cell nodes that have been rendered, but not yet indexed in
    // cellNodesByColumnIdx.  These are in the same order as cell nodes added at the
    // end of the row.
    cellRenderQueue: number[];

    // Elements returned from formatters for cells in cellRenderQueue.
    cellRenderContent: (Element | DocumentFragment)[];
}

export interface GoToResult {
    row: number;
    cell: number;
    posX: number;
}

export interface PostProcessCleanupEntry {
    groupId: number,
    cellNode?: HTMLElement,
    columnIdx?: number,
    rowNodeS?: HTMLElement;
    rowNodeC?: HTMLElement;
    rowNodeE?: HTMLElement;
    rowIdx?: number;
}

