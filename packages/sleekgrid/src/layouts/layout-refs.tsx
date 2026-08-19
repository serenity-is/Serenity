import { computed, signal } from "@serenity-is/domwise";
import type { GridSignals } from "../core/grid-signals";

/** Logical horizontal band key. `start`/`end` are pinned side bands. */
export type BandKey = "start" | "main" | "end";
/** Logical vertical pane key within each band. */
export type PaneKey = "top" | "body" | "bottom";

/**
 * DOM and layout state for a single horizontal band (`start`/`main`/`end`).
 */
export interface GridBandRefs {
    /** Band identifier. */
    key: BandKey;
    /** Header column container for this band, if rendered. */
    headerCols?: HTMLElement;
    /** Header-row (filter row) column container, if rendered. */
    headerRowCols?: HTMLElement;
    /** Canvas elements per vertical pane. */
    canvas: {
        /** Top-frozen pane canvas, if enabled. */
        top?: HTMLElement;
        /** Main body viewport canvas. */
        body: HTMLElement;
        /** Bottom-frozen pane canvas, if enabled. */
        bottom?: HTMLElement;
    },
    /** Footer row column container, if rendered. */
    footerRowCols?: HTMLElement;
    /** Column index offset for cells inside this band (e.g. pinned count). */
    readonly cellOffset: number;
    /** Measured canvas width for this band in pixels. */
    canvasWidth: number;
}

/**
 * Aggregated refs for all bands and derived pinning/frozen indices.
 */
export type GridLayoutRefs = {
    /** Band refs for the pinned-start side. */
    readonly start: GridBandRefs;
    /** Band refs for the main (center, scrollable) band. */
    readonly main: GridBandRefs;
    /** Band refs for the pinned-end side. */
    readonly end: GridBandRefs;
    /** Top panel container element, if rendered. */
    topPanel?: HTMLElement;
    /** Number of columns pinned to the start (derived, bounded by `config`). */
    readonly pinnedStartCols: number;
    /** Last pinned-start column index or `-Infinity` when none. */
    readonly pinnedStartLast: number;
    /** Number of columns pinned to the end. */
    readonly pinnedEndCols: number;
    /** First pinned-end column index or `Infinity` when none. */
    readonly pinnedEndFirst: number;
    /** Number of top-frozen rows. */
    readonly frozenTopRows: number;
    /** Last top-frozen row index or `-Infinity` when none. */
    readonly frozenTopLast: number;
    /** Number of bottom-frozen rows. */
    readonly frozenBottomRows: number;
    /** First bottom-frozen row index or `Infinity` when none. */
    readonly frozenBottomFirst: number;
    /** Writable config inputs; setters trigger {@link createGridSignalsAndRefs} recalculation. */
    config: {
        /** Desired start-pinned column count. */
        pinnedStartCols?: number;
        /** Desired end-pinned column count. */
        pinnedEndCols?: number;
        /** Maximum total pinned columns, or `null` to allow all. */
        pinnedLimit?: number | null;
        /** Total column count driving index calculations. */
        colCount?: number;
        /** Desired top-frozen row count. */
        frozenTopRows?: number;
        /** Desired bottom-frozen row count. */
        frozenBottomRows?: number;
        /** Maximum total frozen rows, or `null` to allow all. */
        frozenLimit?: number | null;
        /** Total data row count driving frozen calculations. */
        dataLength?: number;
    }
}

/**
 * Iterates existing bands (`start`, `main`, `end`) and invokes `callback` for each.
 * @param refs - Aggregate layout refs.
 * @param callback - Action invoked for each band.
 */
export function forEachBand(refs: GridLayoutRefs, callback: (band: GridBandRefs) => void): void {
    if (!refs) return;
    refs.start && callback(refs.start);
    refs.main && callback(refs.main);
    refs.end && callback(refs.end);
};

/**
 * Maps each band to a value, optionally skipping `null`/`undefined` results.
 * @template T - Mapped value type.
 * @param refs - Aggregate layout refs.
 * @param callback - Mapper invoked for each band.
 * @param skipNullReturns - When `true` (default), nullish results are omitted.
 * @returns Collected mapped values.
 */
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

/**
 * Removes and nulls DOM nodes tracked by a single band.
 * @param refs - Band refs to dispose.
 * @param removeNode - Grid-provided removal helper (honors custom sanitizer).
 */
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

/**
 * Collects all existing canvas nodes across bands/panes.
 * @param refs - Aggregate layout refs.
 * @returns Array of non-null canvas elements.
 */
export function getAllCanvasNodes(refs: GridLayoutRefs): HTMLElement[] {
    const canvasNodes: HTMLElement[] = [];
    forEachBand(refs, (h) => paneKeys.forEach(pane => {
        const canvas = h.canvas[pane];
        if (canvas)
            canvasNodes.push(canvas);
    }));
    return canvasNodes;
}

/**
 * Collects viewport containers (`canvas.parentElement`) for all canvases.
 * @param refs - Aggregate layout refs.
 * @returns Array of non-null viewport elements.
 */
export function getAllViewportNodes(refs: GridLayoutRefs): HTMLElement[] {
    const viewportNodes: HTMLElement[] = [];
    forEachBand(refs, (h) => paneKeys.forEach(pane => {
        const viewport = h.canvas[pane]?.parentElement;
        if (viewport)
            viewportNodes.push(viewport);
    }));
    return viewportNodes;
}

/**
 * Collects containers that scroll horizontally (main header rows and main pane viewports).
 * @param refs - Aggregate layout refs.
 * @returns Array of scrollable elements.
 */
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

/**
 * Collects body viewport containers that scroll vertically across all bands.
 * @param refs - Aggregate layout refs.
 * @returns Array of vertically scrollable viewport elements.
 */
export function getAllVScrollContainers(refs: GridLayoutRefs): HTMLElement[] {
    const vScrollableNodes: HTMLElement[] = [];
    forEachBand(refs, (band) => {
        const viewport = band.canvas.body?.parentElement;
        if (viewport)
            vScrollableNodes.push(viewport);
    });
    return vScrollableNodes;
}


/**
 * Factory for the reactive signals and derived layout refs that drive pinning and frozen rows.
 * Generates computed visibility signals, bounded pinning/frozen counters and live getters/setters on `refs.config`.
 * @returns Object containing `signals` and `refs` wired for mutual recalculation.
 */
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
