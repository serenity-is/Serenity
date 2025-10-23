import type { HBand, } from "./layout-consts";

export interface ViewportPaneRefs {
    pane?: HTMLElement;
    viewport?: HTMLElement;
    canvas?: HTMLElement;
}

export interface GridLayoutHRefs {
    headerCols?: HTMLElement;
    topPanel?: HTMLElement;
    headerRowCols?: HTMLElement;
    top?: ViewportPaneRefs;
    body: ViewportPaneRefs;
    bottom?: ViewportPaneRefs;
    footerRowCols?: HTMLElement;
}

export type GridLayoutRefs = {
    main: GridLayoutHRefs;
    start?: GridLayoutHRefs;
    end?: GridLayoutHRefs;
}

export function layoutRefsForEach(refs: GridLayoutRefs, callback: (hRefs: GridLayoutHRefs, hband: HBand) => void): void {
    if (!refs) return;
    refs.start && callback(refs.start, "start");
    refs.main && callback(refs.main, "main");
    refs.end && callback(refs.end, "end");
};

export function mapLayoutRefs<T>(refs: GridLayoutRefs, callback: (hRefs: GridLayoutHRefs, hband: HBand) => T, skipNullReturns = true): T[] {
    const result: T[] = [];
    layoutRefsForEach(refs, (hRefs, hband) => {
        const ret = callback(hRefs, hband);
        if (!skipNullReturns || ret != null)
            result.push(ret);
    });
    return result;
}

function disposeViewportPaneRefs(refs: ViewportPaneRefs, removeNode: (node: HTMLElement) => void): void {
    if (!refs) return;
    removeNode(refs.canvas);
    removeNode(refs.viewport);
    removeNode(refs.pane);
    refs.canvas = refs.viewport = refs.pane = null;
}

export function disposeLayoutHRefs(refs: GridLayoutHRefs, removeNode: (node: HTMLElement) => void): void {
    if (!refs) return;
    refs.headerCols && (refs.headerCols.onselectstart = null);
    removeNode(refs.headerCols?.parentElement);
    removeNode(refs.topPanel?.parentElement);
    removeNode(refs.headerRowCols?.parentElement);
    removeNode(refs.footerRowCols?.parentElement);
    refs.headerCols = refs.topPanel = refs.headerRowCols = refs.footerRowCols = null;
    disposeViewportPaneRefs(refs.top, removeNode);
    disposeViewportPaneRefs(refs.body, removeNode);
    disposeViewportPaneRefs(refs.bottom, removeNode);
}

export function getAllCanvasNodes(refs: GridLayoutRefs): HTMLElement[] {
    const canvasNodes: HTMLElement[] = [];
    layoutRefsForEach(refs, (hRefs) => {
        hRefs.top?.canvas && canvasNodes.push(hRefs.top.canvas);
        hRefs.body?.canvas && canvasNodes.push(hRefs.body.canvas);
        hRefs.bottom?.canvas && canvasNodes.push(hRefs.bottom.canvas);
    });
    return canvasNodes;
}
