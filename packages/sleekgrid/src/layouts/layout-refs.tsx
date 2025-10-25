
export interface ViewportPaneRefs {
    pane?: HTMLElement;
    viewport?: HTMLElement;
    canvas?: HTMLElement;
}

export interface GridLayoutHRefs {
    headerCols?: HTMLElement;
    topPanel?: HTMLElement;
    headerRowCols?: HTMLElement;
    readonly top: ViewportPaneRefs;
    readonly body: ViewportPaneRefs;
    readonly bottom: ViewportPaneRefs;
    footerRowCols?: HTMLElement;
}

export type GridLayoutRefs = {
    readonly main: GridLayoutHRefs;
    readonly start: GridLayoutHRefs;
    readonly end: GridLayoutHRefs;
    pinnedStartLast: number;
    pinnedEndFirst: number;
    frozenTopLast: number;
    frozenBottomFirst: number;
}

export function layoutRefsForEach(refs: GridLayoutRefs, callback: (hRefs: GridLayoutHRefs) => void): void {
    if (!refs) return;
    refs.start && callback(refs.start);
    refs.main && callback(refs.main);
    refs.end && callback(refs.end);
};

export function mapLayoutRefs<T>(refs: GridLayoutRefs, callback: (hRefs: GridLayoutHRefs) => T, skipNullReturns = true): T[] {
    const result: T[] = [];
    layoutRefsForEach(refs, (hRefs) => {
        const ret = callback(hRefs);
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
    layoutRefsForEach(refs, (h) => {
        h.top.canvas && canvasNodes.push(h.top.canvas);
        h.body.canvas && canvasNodes.push(h.body.canvas);
        h.bottom.canvas && canvasNodes.push(h.bottom.canvas);
    });
    return canvasNodes;
}

export function getAllViewportNodes(refs: GridLayoutRefs): HTMLElement[] {
    const viewportNodes: HTMLElement[] = [];
    layoutRefsForEach(refs, (h) => {
        h.top.viewport && viewportNodes.push(h.top.viewport);
        h.body.viewport && viewportNodes.push(h.body.viewport);
        h.bottom.viewport && viewportNodes.push(h.bottom.viewport);
    });
    return viewportNodes;
}

export function getAllHScrollContainers(refs: GridLayoutRefs): HTMLElement[] {
    const hScrollableNodes: HTMLElement[] = [];
    const main = refs.main;;
    main.body.viewport && hScrollableNodes.push(main.body.viewport);
    main.headerCols?.parentElement && hScrollableNodes.push(main.headerCols.parentElement);
    main.topPanel?.parentElement && hScrollableNodes.push(main.topPanel.parentElement);
    main.headerRowCols?.parentElement && hScrollableNodes.push(main.headerRowCols.parentElement);
    main.top.viewport && hScrollableNodes.push(main.top.viewport);
    main.bottom.viewport && hScrollableNodes.push(main.bottom.viewport);
    main.footerRowCols?.parentElement && hScrollableNodes.push(main.footerRowCols.parentElement);
    return hScrollableNodes;
}

export function getAllVScrollContainers(refs: GridLayoutRefs): HTMLElement[] {
    const vScrollableNodes: HTMLElement[] = [];
    layoutRefsForEach(refs, (h) => {
        h.body.viewport && vScrollableNodes.push(h.body.viewport);
    });
    return vScrollableNodes;
}

