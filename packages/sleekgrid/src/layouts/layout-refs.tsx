import { computed, signal } from "@serenity-is/domwise";
import type { GridSignals } from "../core/grid-signals";

export type BandKey = "start" | "main" | "end";
export type PaneKey = "top" | "body" | "bottom";

export interface GridBandRefs {
    key: BandKey;
    headerCols?: HTMLElement;
    headerRowCols?: HTMLElement;
    canvas: {
        top?: HTMLElement;
        body: HTMLElement;
        bottom?: HTMLElement;
    },
    footerRowCols?: HTMLElement;
    readonly firstCol: number;
    canvasWidth: number;
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

const paneKeys: PaneKey[] = ["top", "body", "bottom"];

export function disposeBandRefs(refs: GridBandRefs, removeNode: (node: HTMLElement) => void): void {
    if (!refs) return;
    refs.headerCols && (refs.headerCols.onselectstart = null);
    removeNode(refs.headerCols?.parentElement);
    removeNode(refs.headerRowCols?.parentElement);
    removeNode(refs.footerRowCols?.parentElement);
    refs.headerCols = refs.headerRowCols = refs.footerRowCols = null;
    for (const paneKey of paneKeys) {
        const canvas = refs.canvas[paneKey];
        if (canvas) {
            const viewport = canvas.parentElement;
            removeNode(canvas);
            removeNode(viewport);
            refs.canvas[paneKey] = null;
        }
    }
}

export function getAllCanvasNodes(refs: GridLayoutRefs): HTMLElement[] {
    const canvasNodes: HTMLElement[] = [];
    forEachBand(refs, (h) => paneKeys.forEach(pane => {
        const canvas = h.canvas[pane];
        if (canvas)
            canvasNodes.push(canvas);
    }));
    return canvasNodes;
}

export function getAllViewportNodes(refs: GridLayoutRefs): HTMLElement[] {
    const viewportNodes: HTMLElement[] = [];
    forEachBand(refs, (h) => paneKeys.forEach(pane => {
        const viewport = h.canvas[pane]?.parentElement;
        if (viewport)
            viewportNodes.push(viewport);
    }));
    return viewportNodes;
}

export function getAllHScrollContainers(refs: GridLayoutRefs): HTMLElement[] {
    const hScrollableNodes: HTMLElement[] = [];
    const main = refs.main;
    main.headerCols?.parentElement && hScrollableNodes.push(main.headerCols.parentElement);
    main.headerRowCols?.parentElement && hScrollableNodes.push(main.headerRowCols.parentElement);
    paneKeys.forEach(pane => {
        const viewport = main.canvas[pane]?.parentElement;
        if (viewport)
            hScrollableNodes.push(viewport);
    });
    main.footerRowCols?.parentElement && hScrollableNodes.push(main.footerRowCols.parentElement);
    return hScrollableNodes;
}

export function getAllVScrollContainers(refs: GridLayoutRefs): HTMLElement[] {
    const vScrollableNodes: HTMLElement[] = [];
    forEachBand(refs, (band) => {
        const viewport = band.canvas.body?.parentElement;
        if (viewport)
            vScrollableNodes.push(viewport);
    });
    return vScrollableNodes;
}


export function createGridSignalsAndRefs(): { signals: GridSignals; refs: GridLayoutRefs } {
    const showColumnHeader = signal();
    const hideColumnHeader = computed(() => !showColumnHeader.value);
    const showHeaderRow = signal();
    const hideHeaderRow = computed(() => !showHeaderRow.value);
    const showFooterRow = signal();
    const hideFooterRow = computed(() => !showFooterRow.value);
    const showTopPanel = signal();
    const hideTopPanel = computed(() => !showTopPanel.value);
    let pinnedStartLast = -Infinity;
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
        pinnedStartLast: signal(pinnedStartLast),
        pinnedEndFirst: signal(pinnedEndFirst),
        frozenTopLast: signal(frozenTopLast),
        frozenBottomFirst: signal(frozenBottomFirst),
    };
    const refs: GridLayoutRefs = {
        start: {
            key: "start",
            canvas: {
                body: null
            },
            firstCol: -Infinity,
            canvasWidth: 0
        },
        main: {
            key: "main",
            canvas: { body: null },
            firstCol: 0,
            canvasWidth: 0
        },
        end: {
            key: "end",
            canvas: {
                body: null
            },
            firstCol: Infinity,
            canvasWidth: 0
        },
        get pinnedStartLast() {
            return pinnedStartLast;
        },
        set pinnedStartLast(value) {
            if (pinnedStartLast !== value) {
                pinnedStartLast = value;
                (refs.start as any).firstCol = value >= 0 ? 0 : -Infinity;
                (refs.main as any).firstCol = value >= 0 ? value + 1 : 0;
                signals.pinnedStartLast.value = value;
            }
        },
        get pinnedEndFirst() {
            return pinnedEndFirst;
        },
        set pinnedEndFirst(value) {
            if (pinnedEndFirst !== value) {
                pinnedEndFirst = value;
                (refs.end as any).firstCol = value;
            }
            signals.pinnedEndFirst.value = value;
        },
        get frozenTopLast() {
            return frozenTopLast;
        },
        set frozenTopLast(value) {
            frozenTopLast = value;
            signals.frozenTopLast.value = value;
        },
        get frozenBottomFirst() {
            return frozenBottomFirst;
        },
        set frozenBottomFirst(value) {
            frozenBottomFirst = value;
            signals.frozenBottomFirst.value = value;
        }
    };
    return { signals, refs };
}
