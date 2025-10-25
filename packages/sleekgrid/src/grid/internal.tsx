import { computed, signal } from "@serenity-is/signals";
import { invokeDisposingListeners } from "@serenity-is/sleekdom";
import { Column, type GridSignals } from "../core";
import type { GridLayoutRefs } from "../layouts/layout-refs";

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

export const defaultRemoveNode = (node: HTMLElement) => {
    if (!node)
        return;
    invokeDisposingListeners(node, { descendants: true });
    node.remove();
}

export const defaultEmptyNode = (node: HTMLElement) => {
    if (!node)
        return;
    invokeDisposingListeners(node, { descendants: true, excludeSelf: true });
    node.innerHTML = "";
}

export function defaultJQueryEmptyNode(this: { (node: HTMLElement): { empty: () => void }, fn: any }, node: HTMLElement) {
    if (!node)
        return;
    if (!this || this.fn)
        defaultEmptyNode(node);
    else
        this(node).empty();
}

export function defaultJQueryRemoveNode(this: { (node: HTMLElement): { remove: () => void }, fn: any }, node: HTMLElement) {
    if (!node)
        return;
    if (!this || this.fn)
        defaultRemoveNode(node);
    else
        this(node).remove();
}

export function bindPrototypeMethods(instance: any, filter?: (key: string | symbol, func: Function) => boolean) {
    // adapted from https://github.com/sindresorhus/auto-bind
    let object = instance.constructor.prototype;
    do {
        for (const key of Reflect.ownKeys(object)) {
            if (key === 'constructor' || Reflect.hasOwnProperty(key))
                continue;

            const descriptor = Reflect.getOwnPropertyDescriptor(object, key);
            if (descriptor && typeof descriptor.value === 'function' &&
                !Reflect.getOwnPropertyDescriptor(instance, key)) {
                const func = instance[key];
                if (filter && !filter(key, func))
                    continue;
                instance[key] = func.bind(instance);
            }
        }
    } while ((object = Reflect.getPrototypeOf(object)) && object !== Object.prototype);
}

export function createGridSignalsAndRefs(): { signals: GridSignals; refs: GridLayoutRefs } {
    const showColumnHeader = signal<boolean>();
    const hideColumnHeader = computed(() => !showColumnHeader.value);
    const showHeaderRow = signal<boolean>();
    const hideHeaderRow = computed(() => !showHeaderRow.value);
    const showFooterRow = signal<boolean>();
    const hideFooterRow = computed(() => !showFooterRow.value);
    const showTopPanel = signal<boolean>();
    const hideTopPanel = computed(() => !showTopPanel.value);
    let pinnedStartFirst = -Infinity;
    let pinnedEndFirst = Infinity;
    let frozenTopLast = -Infinity;
    let frozenBottomFirst = Infinity;
    const signals: GridSignals = {
        showColumnHeader,
        hideColumnHeader,
        showTopPanel,
        hideTopPanel,
        showHeaderRow,
        hideHeaderRow,
        showFooterRow,
        hideFooterRow,
        pinnedStartLast: signal(pinnedStartFirst),
        pinnedEndFirst: signal(pinnedEndFirst),
        frozenTopLast: signal(frozenTopLast),
        frozenBottomFirst: signal(frozenBottomFirst),
    };
    const refs: GridLayoutRefs = {
        start: { body: {}, top: {}, bottom: {} },
        main: { body: {}, top: {}, bottom: {} },
        end: { body: {}, top: {}, bottom: {} },
        get pinnedEndFirst() { return pinnedEndFirst; },
        set pinnedEndFirst(value) { pinnedEndFirst = value; signals.pinnedEndFirst.value = value; },
        get pinnedStartLast() { return pinnedStartFirst; },
        set pinnedStartLast(value) { pinnedStartFirst = value; signals.pinnedStartLast.value = value; },
        get frozenTopLast() { return frozenTopLast; },
        set frozenTopLast(value) { frozenTopLast = value; signals.frozenTopLast.value = value; },
        get frozenBottomFirst() { return frozenBottomFirst; },
        set frozenBottomFirst(value) { frozenBottomFirst = value; signals.frozenBottomFirst.value = value; },
    };
    return { signals, refs };
}
