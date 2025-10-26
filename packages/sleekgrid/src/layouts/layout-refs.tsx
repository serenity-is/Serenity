export type BandKey = "start" | "main" | "end";
export type PaneKey = "top" | "body" | "bottom";

export interface DataPaneRefs {
    pane?: HTMLElement;
    viewport?: HTMLElement;
    canvas?: HTMLElement;
}

export interface GridBandRefs {
    headerCols?: HTMLElement;
    headerRowCols?: HTMLElement;
    readonly top: DataPaneRefs;
    readonly body: DataPaneRefs;
    readonly bottom: DataPaneRefs;
    footerRowCols?: HTMLElement;
}

export type GridLayoutRefs = {
    readonly start: GridBandRefs;
    readonly main: GridBandRefs;
    readonly end: GridBandRefs;
    topPanel?: HTMLElement;
    pinnedStartLast: number;
    pinnedEndFirst: number;
    frozenTopLast: number;
    frozenBottomFirst: number;
}

export function forEachBand(refs: GridLayoutRefs, callback: (band: GridBandRefs) => void): void {
    if (!refs) return;
    refs.start && callback(refs.start);
    refs.main && callback(refs.main);
    refs.end && callback(refs.end);
};

export function mapBands<T>(refs: GridLayoutRefs, callback: (band: GridBandRefs) => T, skipNullReturns = true): T[] {
    const result: T[] = [];
    forEachBand(refs, band => {
        const ret = callback(band);
        if (!skipNullReturns || ret != null)
            result.push(ret);
    });
    return result;
}

function disposeDataPaneRefs(refs: DataPaneRefs, removeNode: (node: HTMLElement) => void): void {
    if (!refs) return;
    removeNode(refs.canvas);
    removeNode(refs.viewport);
    removeNode(refs.pane);
    refs.canvas = refs.viewport = refs.pane = null;
}

export function disposeBandRefs(refs: GridBandRefs, removeNode: (node: HTMLElement) => void): void {
    if (!refs) return;
    refs.headerCols && (refs.headerCols.onselectstart = null);
    removeNode(refs.headerCols?.parentElement);
    removeNode(refs.headerRowCols?.parentElement);
    removeNode(refs.footerRowCols?.parentElement);
    refs.headerCols = refs.headerRowCols = refs.footerRowCols = null;
    disposeDataPaneRefs(refs.top, removeNode);
    disposeDataPaneRefs(refs.body, removeNode);
    disposeDataPaneRefs(refs.bottom, removeNode);
}

export function getAllCanvasNodes(refs: GridLayoutRefs): HTMLElement[] {
    const canvasNodes: HTMLElement[] = [];
    forEachBand(refs, (h) => {
        h.top.canvas && canvasNodes.push(h.top.canvas);
        h.body.canvas && canvasNodes.push(h.body.canvas);
        h.bottom.canvas && canvasNodes.push(h.bottom.canvas);
    });
    return canvasNodes;
}

export function getAllViewportNodes(refs: GridLayoutRefs): HTMLElement[] {
    const viewportNodes: HTMLElement[] = [];
    forEachBand(refs, (h) => {
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
    main.headerRowCols?.parentElement && hScrollableNodes.push(main.headerRowCols.parentElement);
    main.top.viewport && hScrollableNodes.push(main.top.viewport);
    main.bottom.viewport && hScrollableNodes.push(main.bottom.viewport);
    main.footerRowCols?.parentElement && hScrollableNodes.push(main.footerRowCols.parentElement);
    return hScrollableNodes;
}

export function getAllVScrollContainers(refs: GridLayoutRefs): HTMLElement[] {
    const vScrollableNodes: HTMLElement[] = [];
    forEachBand(refs, (h) => {
        h.body.viewport && vScrollableNodes.push(h.body.viewport);
    });
    return vScrollableNodes;
}

