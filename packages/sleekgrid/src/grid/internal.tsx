import { invokeDisposingListeners } from "@serenity-is/domwise";

/**
 * Tests shallow array equality after sorting both inputs.
 * Used to compare selected-rows arrays ignoring order.
 * @param arr1 - First array.
 * @param arr2 - Second array.
 * @returns `true` when arrays contain the same numbers.
 */
export function simpleArrayEquals(arr1: number[], arr2: number[]): boolean {
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
 * Per-row DOM cache entry tracking row nodes and lazily populated cell nodes.
 */
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

/**
 * Navigation result from {@link CellNavigator.navigate}.
 */
export interface GoToResult {
    row: number;
    cell: number;
    posX: number;
}

/**
 * Deferred cleanup entry for async post-rendered rows or cells.
 */
export interface PostProcessCleanupEntry {
    groupId: number,
    cellNode?: HTMLElement,
    columnIdx?: number,
    rowNodeS?: HTMLElement;
    rowNodeC?: HTMLElement;
    rowNodeE?: HTMLElement;
    rowIdx?: number;
}

/**
 * Default DOM remover that invokes `domwise` disposing listeners before `remove()`.
 * @param node - Element to remove.
 */
export const defaultRemoveNode = (node: HTMLElement): void => {
    if (!node)
        return;
    invokeDisposingListeners(node, { descendants: true });
    node.remove();
}

/**
 * Default container emptier that disposes descendants and clears `innerHTML`.
 * @param node - Element to empty.
 */
export const defaultEmptyNode = (node: HTMLElement): void => {
    if (!node)
        return;
    invokeDisposingListeners(node, { descendants: true, excludeSelf: true });
    node.innerHTML = "";
}

/**
 * jQuery-aware empty helper; falls back to {@link defaultEmptyNode} when jQuery is unavailable.
 * @param node - Element to empty.
 */
export function defaultJQueryEmptyNode(this: { (node: HTMLElement): { empty: () => void }, fn: any }, node: HTMLElement): void {
    if (!node)
        return;
    if (!this || this.fn)
        defaultEmptyNode(node);
    else
        this(node).empty();
}

/**
 * jQuery-aware remover; falls back to {@link defaultRemoveNode} when jQuery is unavailable.
 * @param node - Element to remove.
 */
export function defaultJQueryRemoveNode(this: { (node: HTMLElement): { remove: () => void }, fn: any }, node: HTMLElement): void {
    if (!node)
        return;
    if (!this || this.fn)
        defaultRemoveNode(node);
    else
        this(node).remove();
}
