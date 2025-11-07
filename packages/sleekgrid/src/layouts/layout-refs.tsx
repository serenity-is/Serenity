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
    readonly cellOffset: number;
    canvasWidth: number;
}

export type GridLayoutRefs = {
    readonly start: GridBandRefs;
    readonly main: GridBandRefs;
    readonly end: GridBandRefs;
    topPanel?: HTMLElement;
    readonly pinnedStartCols: number;
    readonly pinnedStartLast: number;
    readonly pinnedEndCols: number;
    readonly pinnedEndFirst: number;
    readonly frozenTopRows: number;
    readonly frozenTopLast: number;
    readonly frozenBottomRows: number;
    readonly frozenBottomFirst: number;
    config: {
        pinnedStartCols?: number;
        pinnedEndCols?: number;
        pinnedLimit?: number | null;
        colCount?: number;
        frozenTopRows?: number;
        frozenBottomRows?: number;
        frozenLimit?: number | null;
        dataLength?: number;
    }
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
    const config = {
        pinnedStartCols: 0,
        pinnedEndCols: 0,
        pinnedLimit: null as number | null,
        frozenTopRows: 0,
        frozenBottomRows: 0,
        frozenLimit: 0,
        colCount: 0,
        dataLength: 0
    };
    const calculated = {
        pinnedStartLast: -Infinity,
        startFirstCol: -Infinity,
        pinnedStartCols: 0,
        pinnedEndFirst: Infinity,
        pinnedEndCols: 0,
        frozenTopLast: -Infinity,
        frozenTopRows: 0,
        frozenBottomFirst: Infinity,
        frozenBottomRows: 0
    };

    function recalc() {
        const colCount = Math.max(config.colCount ?? 0, 0);
        const rowCount = Math.max(config.dataLength ?? 0, 0);
        let pinnedAvail = Math.min(Math.max(config.pinnedLimit ?? colCount, 0), colCount);
        calculated.pinnedStartCols = config.pinnedStartCols > 0 ? Math.min(config.pinnedStartCols, pinnedAvail) : 0;
        pinnedAvail -= calculated.pinnedStartCols;
        calculated.pinnedEndCols = config.pinnedEndCols > 0 ? Math.min(config.pinnedEndCols, pinnedAvail) : 0;

        let frozenAvail = Math.min(Math.max(config.frozenLimit ?? rowCount, 0), rowCount);
        calculated.frozenTopRows = config.frozenTopRows > 0 ? Math.min(config.frozenTopRows, frozenAvail) : 0;
        frozenAvail -= calculated.frozenTopRows;
        calculated.frozenBottomRows = config.frozenBottomRows > 0 ? Math.min(config.frozenBottomRows, frozenAvail) : 0;

        calculated.pinnedStartLast = calculated.pinnedStartCols > 0 ? calculated.pinnedStartCols - 1 : -Infinity;
        calculated.pinnedEndFirst = calculated.pinnedEndCols > 0 ? colCount - calculated.pinnedEndCols : Infinity;
        calculated.frozenTopLast = calculated.frozenTopRows > 0 ? calculated.frozenTopRows - 1 : -Infinity;
        calculated.frozenBottomFirst = calculated.frozenBottomRows > 0 ? rowCount - calculated.frozenBottomRows : Infinity;

        signals.pinnedStartCols.value = calculated.pinnedStartCols;
        signals.pinnedEndCols.value = calculated.pinnedEndCols;
        signals.frozenTopRows.value = calculated.frozenTopRows;
        signals.frozenBottomRows.value = calculated.frozenBottomRows;
    }

    const signals: GridSignals = {
        showColumnHeader,
        hideColumnHeader,
        showTopPanel,
        hideTopPanel,
        showHeaderRow,
        hideHeaderRow,
        showFooterRow,
        hideFooterRow,
        pinnedStartCols: signal(0),
        pinnedEndCols: signal(0),
        frozenTopRows: signal(0),
        frozenBottomRows: signal(0),
    };
    const refs: GridLayoutRefs = {
        start: {
            key: "start",
            canvas: {
                body: null
            },
            cellOffset: 0,
            canvasWidth: 0
        },
        main: {
            key: "main",
            canvas: { body: null },
            get cellOffset() { return calculated.pinnedStartCols; },
            canvasWidth: 0
        },
        end: {
            key: "end",
            canvas: {
                body: null
            },
            get cellOffset() { return calculated.pinnedEndFirst >= 0 ? calculated.pinnedEndFirst : 0; },
            canvasWidth: 0
        },
        get pinnedStartCols() {
            return calculated.pinnedStartCols;
        },
        get pinnedStartLast() {
            return calculated.pinnedStartLast;
        },
        get pinnedEndFirst() {
            return calculated.pinnedEndFirst
        },
        get pinnedEndCols() {
            return calculated.pinnedEndCols;
        },
        get frozenTopRows() {
            return calculated.frozenTopRows;
        },
        get frozenTopLast() {
            return calculated.frozenTopLast;
        },
        get frozenBottomRows() {
            return calculated.frozenBottomRows;
        },
        get frozenBottomFirst() {
            return calculated.frozenBottomFirst;
        },
        config: {
            get dataLength() { return config.dataLength; },
            get colCount() { return config.colCount; },
            get pinnedStartCols() { return config.pinnedStartCols; },
            get pinnedEndCols() { return config.pinnedEndCols; },
            get pinnedLimit() { return config.pinnedLimit; },
            get frozenTopRows() { return config.frozenTopRows; },
            get frozenBottomRows() { return config.frozenBottomRows; },
            get frozenLimit() { return config.frozenLimit; },
            set dataLength(v: number) {
                if (config.dataLength !== v) {
                    config.dataLength = v;
                    recalc();
                }
            },
            set colCount(v: number) {
                if (config.colCount !== v) {
                    config.colCount = v;
                    recalc();
                }
            },
            set pinnedStartCols(v: number) {
                if (config.pinnedStartCols !== v) {
                    config.pinnedStartCols = v;
                    recalc();
                }
            },
            set pinnedEndCols(v: number) {
                if (config.pinnedEndCols !== v) {
                    config.pinnedEndCols = v;
                    recalc();
                }
            },
            set pinnedLimit(v: number | null) {
                if (config.pinnedLimit !== v) {
                    config.pinnedLimit = v;
                    recalc();
                }
            },
            set frozenTopRows(v: number) {
                if (config.frozenTopRows !== v) {
                    config.frozenTopRows = v;
                    recalc();
                }
            },
            set frozenBottomRows(v: number) {
                if (config.frozenBottomRows !== v) {
                    config.frozenBottomRows = v;
                    recalc();
                }
            },
            set frozenLimit(v: number) {
                if (config.frozenLimit !== v) {
                    config.frozenLimit = v;
                    recalc();
                }
            }
        }
    };
    return { signals, refs };
}
